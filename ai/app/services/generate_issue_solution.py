from fastapi import HTTPException, status
import aiomysql

from app.generation.generate_issue_solution import issue_solution_prompt
from app.schemas.ai_suggestion import CurrentUser, LLMSuggestionResponse
from app.ingest.llm import DynamicChatModel



async def generate_issue_solution(
        title: str,
        description: str,
):
    llm = DynamicChatModel().get_underlying_model().with_structured_output(LLMSuggestionResponse)
    llm_chain = issue_solution_prompt | llm

    result = await llm_chain.ainvoke(
        {
            "title": title,
            "description": description,
        }
    )

    try:
        result_final = result.to_str
    except Exception as e:
        result_final = "Something went wrong. Please try again later."

    return result_final
