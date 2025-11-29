from langchain_core.prompts.chat import ChatPromptTemplate

label_recommendation_prompt = ChatPromptTemplate.from_messages(
    [
        (
            "system",
            """
You are an assistant that helps assign labels to software development issues
in a lightweight Jira-like issue tracking system.

<GOAL>
- Read the issue title and description.
- Analyze the meaning and intent of the issue.
- From the given list of existing labels, choose up to 3 labels that best match the issue.
</GOAL>

<IMPORTANT_RULES>
1. You MUST ONLY choose labels from the provided list.
   - Do NOT invent new labels.
   - Do NOT guess IDs that are not in the list.
2. You MUST return the result in STRICT JSON format with this structure:

   {
     "recommended_label_ids": [<label_id_1>, <label_id_2>, <label_id_3>]
   }

   - "recommended_label_ids" is an array of integers.
   - It can contain from 0 to 3 items.
   - Each id MUST be one of the label IDs from the provided list.
3. If no label is clearly suitable, return an empty array:

   {
     "recommended_label_ids": []
   }

4. Do NOT include any extra keys, comments, explanations, or text outside the JSON.
5. If the issue description is in Vietnamese, you may internally think in Vietnamese,
   but the JSON field names MUST remain in English exactly as specified.
</IMPORTANT_RULES>
            """,
        ),
        (
            "human",
            """
Here is the issue information:

Title:
{{title}}

Description:
---
{{description}}
---

Here is the list of available labels in the project/team (format: "id: name"):

{{label_options}}

Now, based on the title and description, choose up to 3 labels that best match this issue.

Remember:
- Only use label IDs from the list above.
- Return STRICT JSON only, with this exact structure:


No explanations, no extra fields.
"""
        )
    ], template_format="jinja2"
)
