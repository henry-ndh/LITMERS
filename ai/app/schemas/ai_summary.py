from pydantic import BaseModel, Field

class AISummaryResponse(BaseModel):
    summary: str
    cached: bool
    remainingDaily: int
    remainingMinutely: int

class CurrentUser(BaseModel):
    id: int

class LLMSummaryResponse(BaseModel):
    summary: str = Field(
        ..., description="The summary of description"
    )
    reason: str = Field(
        ..., description="The reason <= 20 words for summary"
    )