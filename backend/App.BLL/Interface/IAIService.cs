using App.Entity.DTO.Response.AI;

namespace App.BLL.Interface
{
    public interface IAIService
    {
        Task<AISummaryResponseDTO> GetIssueSummary(long issueId, long userId);
        Task<AISuggestionResponseDTO> GetIssueSuggestion(long issueId, long userId);
    }
}

