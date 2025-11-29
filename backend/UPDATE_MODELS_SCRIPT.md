# Script để update còn lại các models

Tôi đã update xong các models chính. Do số lượng models còn lại khá nhiều, dưới đây là danh sách các file cần update và các columns cần thêm `[Column("snake_case")]`:

## IssueStatusModel
- ProjectId -> project_id
- Name -> name
- Color -> color
- Position -> position
- IsDefault -> is_default
- WipLimit -> wip_limit
- DeletedAt -> deleted_at

## IssueModel
- ProjectId -> project_id
- StatusId -> status_id
- Title -> title
- Description -> description
- OwnerId -> owner_id
- AssigneeId -> assignee_id
- DueDate -> due_date
- Priority -> priority
- Position -> position
- DeletedAt -> deleted_at
- AiSummary -> ai_summary
- AiSummaryGeneratedAt -> ai_summary_generated_at
- AiSuggestion -> ai_suggestion
- AiSuggestionGeneratedAt -> ai_suggestion_generated_at
- AiCommentSummary -> ai_comment_summary
- AiCommentSummaryGeneratedAt -> ai_comment_summary_generated_at
- AiCommentSummaryCommentCount -> ai_comment_summary_comment_count

## ProjectLabelModel
- ProjectId -> project_id
- Name -> name
- Color -> color

## IssueLabelModel (không kế thừa BaseEntity)
- IssueId -> issue_id
- LabelId -> label_id

## IssueSubtaskModel
- IssueId -> issue_id
- Title -> title
- IsDone -> is_done
- Position -> position

## IssueHistoryModel
- IssueId -> issue_id
- Field -> field
- OldValue -> old_value
- NewValue -> new_value
- ActorId -> actor_id

## CommentModel
- IssueId -> issue_id
- UserId -> user_id
- Content -> content
- DeletedAt -> deleted_at

## AIDailyUsageModel
- UserId -> user_id
- Date -> date
- Count -> count

## AIMinuteUsageModel
- UserId -> user_id
- MinuteBucket -> minute_bucket
- Count -> count

## IssueEmbeddingModel
- IssueId -> issue_id
- Embedding -> embedding

## NotificationModel
- UserId -> user_id
- Type -> type
- Title -> title
- Message -> message
- Payload -> payload
- IsRead -> is_read
