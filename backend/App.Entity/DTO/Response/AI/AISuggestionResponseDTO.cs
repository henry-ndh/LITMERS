namespace App.Entity.DTO.Response.AI
{
    public class AISuggestionResponseDTO
    {
        public string Suggestion { get; set; }
        public bool Cached { get; set; }
        public int RemainingDaily { get; set; }
        public int RemainingMinutely { get; set; }
    }
}

