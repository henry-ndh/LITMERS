import aiomysql, json
from fastapi import APIRouter, Depends, HTTPException, status
from typing import Optional
from datetime import datetime

from app.schemas.ai_label import AILabelRecommendationResponse, AILabelRecommendationRequest
from app.schemas.ai_suggestion import AISuggestionResponse
from app.services.generate_issue_solution import generate_issue_solution
from app.services.generate_label_recommendation import generate_label_recommendation
from app.utils.db_connection import mysql_pool, get_db_conn
from app.constants import MINIMUM_LENGTH_DES_ISSUE
from app.services.check_and_consume import check_and_consume, RateLimitExceeded
from app.schemas.ai_summary import AISummaryResponse, CurrentUser
from app.services.generate_issue_summary import ensure_user_can_view_issue, generate_issue_summary

router = APIRouter(prefix="/api", tags=["ai"])

@router.post("/test-ai")
async def test_ai(
    conn: aiomysql.Connection = Depends(get_db_conn),
    # current_user: CurrentUser = Depends(get_current_user)  # thực tế bạn dùng auth riêng
):
    user_id = 4  # demo, sau dùng current_user.id

    try:
        # 1. Check & consume quota (1 điểm)
        info = await check_and_consume(conn, user_id=user_id, cost=1)

        # 2. Tới đây là đủ quota → bạn gọi OpenAI / logic AI khác
        # ... gọi model ...

        await conn.commit()
        return {
            "ok": True,
            "remainingDaily": info.remaining_daily,
            "remainingMinutely": info.remaining_minutely,
        }

    except RateLimitExceeded as e:
        await conn.rollback()
        info = e.info
        raise HTTPException(
            status_code=status.HTTP_429_TOO_MANY_REQUESTS,
            detail={
                "code": "AI_RATE_LIMIT_EXCEEDED",
                "message": "AI rate limit exceeded",
                "remainingDaily": info.remaining_daily,
                "remainingMinutely": info.remaining_minutely,
                "resetDay": info.reset_day.isoformat(),
                "resetMinute": info.reset_minute.isoformat(),
            },
        )
    except Exception as e:
        await conn.rollback()
        print(str(e))
        raise HTTPException(status_code=500, detail=f"Internal server error: {str(e)}")



# =========================
#  FR-040: AI Summary Generation
# =========================

@router.post(
    "/{issue_id}/ai/summary",
    response_model=AISummaryResponse,
)
async def generate_ai_summary_for_issue(
    issue_id: int,
    current_user: CurrentUser,
    conn: aiomysql.Connection = Depends(get_db_conn),
):
    """
    FR-040 – AI Summary Generation

    - Auth + check quyền xem issue.
    - Nếu description length <= 10 -> 400 { code: "DESCRIPTION_TOO_SHORT" }.
    - Gọi AiRateLimiter.checkAndConsume(userId).
    - Cache:
        + Nếu issue.aiSummary đã có -> dùng cache, cost=0.
        + Nếu chưa -> gọi AI, lưu aiSummary + aiSummaryGeneratedAt.
    """

    try:
        # 1. Load issue
        async with conn.cursor() as cur:
            await cur.execute(
                """
                SELECT
                    id,
                    project_id,
                    description,
                    ai_summary,
                    ai_summary_generated_at
                FROM issues
                WHERE id = %s
                """,
                (issue_id,),
            )
            issue = await cur.fetchone()

        if not issue:
            raise HTTPException(status_code=404, detail="Issue not found")

        # 2. Check quyền xem issue
        await ensure_user_can_view_issue(conn, current_user, issue)

        # 3. Validate description length
        description: str = issue.get("description") or ""
        if len(description.strip()) <= MINIMUM_LENGTH_DES_ISSUE:
            # Không cần DB commit/rollback vì chưa đụng gì
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail={
                    "code": "DESCRIPTION_TOO_SHORT",
                    "message": f"Issue description is too short for AI summary. Minimum length of description length: {MINIMUM_LENGTH_DES_ISSUE}.",
                },
            )

        # 4. Cache branch
        ai_summary: Optional[str] = issue.get("ai_summary")

        if ai_summary:
            # Đã có cache -> chỉ check quota với cost = 0
            info = await check_and_consume(conn, user_id=current_user.id, cost=0)
            await conn.commit()  # lưu record usage nếu có insert
            return AISummaryResponse(
                summary=ai_summary,
                cached=True,
                remainingDaily=info.remaining_daily,
                remainingMinutely=info.remaining_minutely,
            )

        # 5. Chưa có cache -> trừ quota + gọi AI
        info = await check_and_consume(conn, user_id=current_user.id, cost=1)

        try:
            summary = await generate_issue_summary(description)
        except Exception as e:
            print(str(e))
            await conn.rollback()
            raise HTTPException(
                status_code=status.HTTP_502_BAD_GATEWAY,
                detail={
                    "code": "AI_SUMMARY_FAILED",
                    "message": "Failed to generate AI summary.",
                },
            )

        now = datetime.utcnow()

        # 6. Lưu summary vào issue
        async with conn.cursor() as cur:
            await cur.execute(
                """
                UPDATE issues
                SET ai_summary = %s,
                    ai_summary_generated_at = %s
                WHERE id = %s
                """,
                (summary, now, issue_id),
            )

        await conn.commit()

        return AISummaryResponse(
            summary=summary,
            cached=False,
            remainingDaily=info.remaining_daily,
            remainingMinutely=info.remaining_minutely,
        )

    except RateLimitExceeded as e:
        await conn.rollback()
        info = e.info
        # FE có thể đọc code + remaining để show message
        raise HTTPException(
            status_code=status.HTTP_429_TOO_MANY_REQUESTS,
            detail={
                "code": "AI_RATE_LIMIT_EXCEEDED",
                "message": "AI rate limit exceeded",
                "remainingDaily": info.remaining_daily,
                "remainingMinutely": info.remaining_minutely,
                "resetDay": info.reset_day.isoformat(),
                "resetMinute": info.reset_minute.isoformat(),
            },
        )
    except HTTPException:
        # Các HTTPException ở trên đã có status, chỉ việc throw tiếp
        raise
    except Exception as e:
        await conn.rollback()
        print(str(e))
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "code": "AI_SUMMARY_FAILED",
                "message": "Unexpected error while generating AI summary.",
            },
        )


# =========================
#  FR-041: AI Solution Suggestion
# =========================
@router.post(
    "/{issue_id}/ai/suggestion",
    response_model=AISuggestionResponse,
)
async def generate_ai_suggestion_for_issue(
    issue_id: int,
    current_user: CurrentUser,
    conn: aiomysql.Connection = Depends(get_db_conn),
):
    """
    FR-041 – AI Solution Suggestion

    - Auth + check quyền xem issue.
    - Nếu description length <= 10 -> 400 { code: "DESCRIPTION_TOO_SHORT" }.
    - Rate limit với AiRateLimiter.checkAndConsume.
    - Cache:
        + Nếu issue.ai_suggestion có rồi -> trả cached, cost=0.
        + Nếu chưa -> gọi AI, lưu ai_suggestion + ai_suggestion_generated_at.
    """

    try:
        # 1. Load issue
        async with conn.cursor() as cur:
            await cur.execute(
                """
                SELECT
                    id,
                    project_id,
                    title,
                    description,
                    ai_suggestion,
                    ai_suggestion_generated_at
                FROM issues
                WHERE id = %s
                """,
                (issue_id,),
            )
            issue = await cur.fetchone()

        if not issue:
            raise HTTPException(status_code=404, detail="Issue not found")

        # 2. Check quyền xem issue
        await ensure_user_can_view_issue(conn, current_user, issue)

        # 3. Validate description length
        description: str = issue.get("description") or ""
        title: str = issue.get("title") or ""
        if len(description.strip()) <= MINIMUM_LENGTH_DES_ISSUE:
            # Không cần DB commit/rollback vì chưa đụng gì
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail={
                    "code": "DESCRIPTION_TOO_SHORT",
                    "message": f"Issue description is too short for AI suggestion. Minimum length of description length: {MINIMUM_LENGTH_DES_ISSUE}.",
                },
            )

        # 4. Cache branch
        ai_suggestion: Optional[str] = issue.get("ai_suggestion")

        if ai_suggestion:
            # Có cache -> không trừ quota (cost=0)
            info = await check_and_consume(conn, user_id=current_user.id, cost=0)
            await conn.commit()
            return AISuggestionResponse(
                suggestion=ai_suggestion,
                cached=True,
                remainingDaily=info.remaining_daily,
                remainingMinutely=info.remaining_minutely,
            )

        # 5. Chưa có cache -> trừ quota + gọi AI
        info = await check_and_consume(conn, user_id=current_user.id, cost=1)

        try:
            suggestion = await generate_issue_solution(title, description)
        except Exception:
            await conn.rollback()
            raise HTTPException(
                status_code=status.HTTP_502_BAD_GATEWAY,
                detail={
                    "code": "AI_SUGGESTION_FAILED",
                    "message": "Failed to generate AI suggestion.",
                },
            )

        now = datetime.utcnow()

        # 6. Lưu suggestion vào issue
        async with conn.cursor() as cur:
            await cur.execute(
                """
                UPDATE issues
                SET ai_suggestion = %s,
                    ai_suggestion_generated_at = %s
                WHERE id = %s
                """,
                (suggestion, now, issue_id),
            )

        await conn.commit()

        return AISuggestionResponse(
            suggestion=suggestion,
            cached=False,
            remainingDaily=info.remaining_daily,
            remainingMinutely=info.remaining_minutely,
        )

    except RateLimitExceeded as e:
        await conn.rollback()
        info = e.info
        raise HTTPException(
            status_code=status.HTTP_429_TOO_MANY_REQUESTS,
            detail={
                "code": "AI_RATE_LIMIT_EXCEEDED",
                "message": "AI rate limit exceeded",
                "remainingDaily": info.remaining_daily,
                "remainingMinutely": info.remaining_minutely,
                "resetDay": info.reset_day.isoformat(),
                "resetMinute": info.reset_minute.isoformat(),
            },
        )
    except HTTPException:
        raise
    except Exception:
        await conn.rollback()
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "code": "AI_SUGGESTION_FAILED",
                "message": "Unexpected error while generating AI suggestion.",
            },
        )


@router.post(
    "/projects/{project_id}/ai/label-recommendation",
    response_model=AILabelRecommendationResponse,
)
async def ai_label_recommendation(
    project_id: int,
    payload: AILabelRecommendationRequest,
    current_user: CurrentUser,
    conn: aiomysql.Connection = Depends(get_db_conn),
):
    """
    FR-043 – AI Auto-Label

    Body: AILabelRecommendationRequest
        - title
        - description
        - label_ids (optional): nếu FE muốn chỉ recommend trong tập này.

    Logic label:
    - Lấy project theo project_id -> lấy team_id (để check quyền nếu cần).
    - Lấy labels từ project_labels theo project_id.
    - (Nếu payload.label_ids có) -> chỉ giữ những label id đó.
    - Gọi AI recommend -> trả tối đa 3 id hợp lệ.
    """

    title = (payload.title or "").strip()
    description = (payload.description or "").strip()
    label_ids_filter = payload.label_ids

    if not title:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail={
                "code": "TITLE_REQUIRED",
                "message": "Issue title is required for AI label recommendation.",
            },
        )

    try:
        # 1. Load project + team để check quyền (giữ logic cũ nếu projects có team_id/owner_id)
        async with conn.cursor() as cur:
            await cur.execute(
                """
                SELECT
                    p.id          AS project_id,
                    p.team_id     AS project_team_id,
                    p.owner_id    AS project_owner_id,
                    p.is_archived AS project_is_archived,
                    p.deleted_at  AS project_deleted_at,
                    t.owner_id    AS team_owner_id,
                    t.deleted_at  AS team_deleted_at
                FROM projects p
                LEFT JOIN teams t ON t.id = p.team_id
                WHERE p.id = %s
                """,
                (project_id,),
            )
            project = await cur.fetchone()

        if (
            not project
            or project["project_deleted_at"] is not None
            or project["team_deleted_at"] is not None
            or bool(project["project_is_archived"])
        ):
            raise HTTPException(status_code=404, detail="Project not found")

        user_id = int(current_user.id)
        project_owner_id = int(project["project_owner_id"])
        team_owner_id = (
            int(project["team_owner_id"])
            if project["team_owner_id"] is not None
            else None
        )
        team_id = project["project_team_id"]

        # 1.1. Check quyền giống ensure_user_can_view_issue
        if user_id != project_owner_id:
            allowed = False

            # Owner team
            if team_owner_id is not None and user_id == team_owner_id:
                allowed = True
            # Member team
            elif team_id is not None:
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
                    allowed = True

            if not allowed:
                raise HTTPException(
                    status_code=status.HTTP_403_FORBIDDEN,
                    detail="Forbidden",
                )

        # 2. Lấy labels từ project_labels theo project_id (KHÔNG phải team_id)
        async with conn.cursor() as cur:
            if label_ids_filter and len(label_ids_filter) > 0:
                placeholders = ",".join(["%s"] * len(label_ids_filter))
                params = [project_id] + label_ids_filter
                await cur.execute(
                    f"""
                    SELECT id, name
                    FROM project_labels
                    WHERE project_id = %s
                      AND id IN ({placeholders})
                    """,
                    params,
                )
            else:
                await cur.execute(
                    """
                    SELECT id, name
                    FROM project_labels
                    WHERE project_id = %s
                    """,
                    (project_id,),
                )

            labels = await cur.fetchall()

        if not labels:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail={
                    "code": "NO_LABELS_IN_PROJECT",
                    "message": "No labels defined for this project.",
                },
            )

        # Ghép text labels cho prompt (id: name)
        label_lines = [f"- {row['id']}: {row['name']}" for row in labels]
        label_text = "\n".join(label_lines)

        # 3. Rate limit: mỗi lần click nút là 1 quota
        info = await check_and_consume(conn, user_id=current_user.id, cost=1)

        # 4. Gọi OpenAI để recommend
        try:
            # result_text là JSON string dạng:
            # {"recommended_label_ids": [1, 2, 3]}
            result_text = await generate_label_recommendation(
                title=title,
                description=description,
                label_text=label_text,
            )
        except Exception as e:
            print("AI label error:", str(e))
            await conn.rollback()
            raise HTTPException(
                status_code=status.HTTP_502_BAD_GATEWAY,
                detail={
                    "code": "AI_LABEL_RECOMMENDATION_FAILED",
                    "message": "Failed to generate AI label recommendation.",
                },
            )

        try:
            raw_ids = result_text.recommended_label_ids
        except Exception as e:
            raw_ids = []
        if not isinstance(raw_ids, list):
            raw_ids = []

        ids = []
        for item in raw_ids:
            try:
                ids.append(int(item))
            except (TypeError, ValueError):
                continue

        # unique + max 3
        ids = list(dict.fromkeys(ids))[:3]

        # chỉ giữ những id tồn tại trong project_labels của project này
        existing_ids = {row["id"] for row in labels}
        ids = [i for i in ids if i in existing_ids]

        await conn.commit()

        return AILabelRecommendationResponse(
            recommendedLabelIds=ids,
            remainingDaily=info.remaining_daily,
            remainingMinutely=info.remaining_minutely,
        )

    except RateLimitExceeded as e:
        await conn.rollback()
        info = e.info
        raise HTTPException(
            status_code=status.HTTP_429_TOO_MANY_REQUESTS,
            detail={
                "code": "AI_RATE_LIMIT_EXCEEDED",
                "message": "AI rate limit exceeded",
                "remainingDaily": info.remaining_daily,
                "remainingMinutely": info.remaining_minutely,
                "resetDay": info.reset_day.isoformat(),
                "resetMinute": info.reset_minute.isoformat(),
            },
        )
    except HTTPException:
        raise
    except Exception as e:
        await conn.rollback()
        print("Unexpected error in label recommendation:", str(e))
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "code": "AI_LABEL_RECOMMENDATION_FAILED",
                "message": "Unexpected error while generating AI label recommendation.",
            },
        )