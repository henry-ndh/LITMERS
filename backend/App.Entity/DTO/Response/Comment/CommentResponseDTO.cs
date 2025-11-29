namespace App.Entity.DTO.Response.Comment
{
    public class CommentResponseDTO
    {
        public long Id { get; set; }
        public long IssueId { get; set; }
        public string IssueTitle { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserAvatar { get; set; }
        public string Content { get; set; }
        public bool IsOwner { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

