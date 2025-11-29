from pydantic import BaseModel, Field
from typing import Optional, List


class AILabelRecommendationRequest(BaseModel):
    title: str
    description: Optional[str] = ""
    label_ids: List[int] = []


class AILabelRecommendationResponse(BaseModel):
    recommendedLabelIds: List[int]
    remainingDaily: int
    remainingMinutely: int

class LabelRecommendationResult(BaseModel):
    recommended_label_ids: List[int] = Field(
        ...,
        description="The recommended label IDs associated with the label."
    )
    reasoning: str = Field(
        ...,
        description="The reasoning associated with the label."
    )