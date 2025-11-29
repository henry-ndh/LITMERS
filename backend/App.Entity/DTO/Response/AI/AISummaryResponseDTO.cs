namespace App.Entity.DTO.Response.AI
{
    public class AISummaryResponseDTO
    {
        public string Summary { get; set; }
        public bool Cached { get; set; }
        public int RemainingDaily { get; set; }
        public int RemainingMinutely { get; set; }
    }
}

