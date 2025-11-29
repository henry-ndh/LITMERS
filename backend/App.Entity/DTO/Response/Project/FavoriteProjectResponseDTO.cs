namespace App.Entity.DTO.Response.Project
{
    public class FavoriteProjectResponseDTO
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

