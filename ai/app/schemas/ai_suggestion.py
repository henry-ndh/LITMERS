from pydantic import BaseModel, Field
from typing import List, Optional

class AISuggestionResponse(BaseModel):
    suggestion: str
    cached: bool
    remainingDaily: int
    remainingMinutely: int

class CurrentUser(BaseModel):
    id: int

class LLMSuggestionResponse(BaseModel):
    """
AI-generated suggestion of an approach to solving an issue.
Used with with_structured_output(IssueSolutionSuggestion).
    """

    overview: str = Field(
        ...,
        description=(
            "A brief summary (1–3 sentences) of the main approach to solving the issue, "
            "written in the same language as the issue description."
        ),
    )
    steps: List[str] = Field(
        ...,
        description=(
            "A list of specific action steps, in logical order, "
            "each element is a concise step."
        ),
    )
    risks: Optional[List[str]] = Field(
        default=None,
        description=(
            "Risks, points to note, edge cases or important dependencies (if any). "
            "Can be left blank if nothing stands out."
        ),
    )

    @property
    def to_str(self) -> str:
        """
        Returns the content as Markdown, used to render the UI.
        """
        parts: List[str] = []

        # Phần overview
        parts.append("### Summary\n")
        parts.append(self.overview.strip())

        # Phần steps (nếu có)
        if self.steps:
            parts.append("\n\n### Recommended steps\n")
            for idx, step in enumerate(self.steps, start=1):
                step_text = step.strip()
                if not step_text:
                    continue
                parts.append(f"{idx}. {step_text}")

        # Phần risks (nếu có và không rỗng)
        if self.risks:
            cleaned_risks = [r.strip() for r in self.risks if r.strip()]
            if cleaned_risks:
                parts.append("\n\n### Risks & Cautions\n")
                for risk in cleaned_risks:
                    parts.append(f"- {risk}")

        return "\n".join(parts)