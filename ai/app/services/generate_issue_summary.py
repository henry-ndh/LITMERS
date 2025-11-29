from fastapi import HTTPException, status
import aiomysql

from app.generation.generate_issue_summary import issue_summary_prompt
from app.schemas.ai_summary import CurrentUser, AISummaryResponse, LLMSummaryResponse
from app.ingest.llm import DynamicChatModel

async def ensure_user_can_view_issue(
    conn: aiomysql.Connection,
    user: CurrentUser,
    issue_row: dict,
) -> None:
    """
    Cho phép xem issue nếu:
    - User là owner của project, HOẶC
    - User là owner của team chứa project, HOẶC
    - User là member của team đó (team_members).
    """

    user_id = int(user.id)
    project_id = issue_row["project_id"]

    # Lấy thông tin project + team
    async with conn.cursor() as cur:
        await cur.execute(
            """
            SELECT
                p.id                AS project_id,
                p.team_id           AS project_team_id,
                p.owner_id          AS project_owner_id,
                p.is_archived       AS project_is_archived,
                p.deleted_at        AS project_deleted_at,
                t.owner_id          AS team_owner_id,
                t.deleted_at        AS team_deleted_at
            FROM projects p
            LEFT JOIN teams t ON t.id = p.team_id
            WHERE p.id = %s
            """,
            (project_id,),
        )
        project = await cur.fetchone()

    # Không tìm thấy project hoặc đã bị xóa/archived → coi như 404
    if (
        not project
        or project["project_deleted_at"] is not None
        or project["team_deleted_at"] is not None
        or bool(project["project_is_archived"])
    ):
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Project not found",
        )

    project_owner_id = int(project["project_owner_id"])
    team_owner_id = int(project["team_owner_id"]) if project["team_owner_id"] is not None else None
    team_id = project["project_team_id"]

    # 1) Owner project
    if user_id == project_owner_id:
        return

    # 2) Owner team
    if team_owner_id is not None and user_id == team_owner_id:
        return

    print(project)
    # 3) Member team
    if team_id is not None:
        async with conn.cursor() as cur:
            await cur.execute(
                """
                SELECT 1
                FROM team_members
                WHERE team_id = %s AND user_id = %s
                LIMIT 1
                """,
                (team_id, user_id),
            )
            member_row = await cur.fetchone()

        if member_row:
            return

    # Không thuộc bất kỳ case nào ở trên → cấm
    raise HTTPException(
        status_code=status.HTTP_403_FORBIDDEN,
        detail="Forbidden",
    )

async def generate_issue_summary(
        description: str,
):
    llm = DynamicChatModel().get_underlying_model().with_structured_output(LLMSummaryResponse)
    llm_chain = issue_summary_prompt | llm

    result = await llm_chain.ainvoke(
        {
            "description": description,
        }
    )

    try:
        result_final = result.summary
    except Exception as e:
        result_final = description

    return result_final