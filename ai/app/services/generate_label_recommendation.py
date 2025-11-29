from fastapi import HTTPException, status
import aiomysql

from app.generation.generate_label_recommendation import label_recommendation_prompt
from app.schemas.ai_label import LabelRecommendationResult
from app.ingest.llm import DynamicChatModel

async def generate_label_recommendation(
        title: str,
        description: str,
        label_text: str,
):
    llm = DynamicChatModel().get_underlying_model().with_structured_output(LabelRecommendationResult)
    llm_chain = label_recommendation_prompt | llm

    result = await llm_chain.ainvoke(
        {
            "title": title,
            "description": description,
            "label_options": label_text,
        }
    )

    try:
        result_final = result
    except Exception as e:
        result_final = []

    return result_final