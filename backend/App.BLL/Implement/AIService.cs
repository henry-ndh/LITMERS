using App.BLL.Interface;
using App.Entity.DTO.Response.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace App.BLL.Implement
{
    public class AIService : IAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIService> _logger;

        public AIService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AISummaryResponseDTO> GetIssueSummary(long issueId, long userId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            
            var baseUrl = _configuration["AIService:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("AIService:BaseUrl is not configured in appsettings");
            }

            try
            {
                var url = $"{baseUrl}/api/{issueId}/ai/summary";
                var payload = new { id = userId };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                httpClient.Timeout = TimeSpan.FromSeconds(60);
                _logger.LogInformation($"[AIService] Calling summary API for issue {issueId}, user {userId}");

                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"[AIService] Summary response received for issue {issueId}");

                // Parse JSON response
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<AISummaryResponseDTO>(responseBody, options);
                
                if (result == null)
                {
                    throw new Exception("Failed to parse AI summary response");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"[AIService] Error calling summary API for issue {issueId}: {ex.Message}");
                throw new Exception($"Failed to get AI summary: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"[AIService] Error parsing summary response for issue {issueId}: {ex.Message}");
                throw new Exception($"Failed to parse AI summary response: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AIService] Unexpected error in GetIssueSummary: {ex.Message}");
                throw;
            }
        }

        public async Task<AISuggestionResponseDTO> GetIssueSuggestion(long issueId, long userId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            
            var baseUrl = _configuration["AIService:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("AIService:BaseUrl is not configured in appsettings");
            }

            try
            {
                var url = $"{baseUrl}/api/{issueId}/ai/suggestion";
                var payload = new { id = userId };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                httpClient.Timeout = TimeSpan.FromSeconds(60);
                _logger.LogInformation($"[AIService] Calling suggestion API for issue {issueId}, user {userId}");

                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"[AIService] Suggestion response received for issue {issueId}");

                // Parse JSON response
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<AISuggestionResponseDTO>(responseBody, options);
                
                if (result == null)
                {
                    throw new Exception("Failed to parse AI suggestion response");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"[AIService] Error calling suggestion API for issue {issueId}: {ex.Message}");
                throw new Exception($"Failed to get AI suggestion: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"[AIService] Error parsing suggestion response for issue {issueId}: {ex.Message}");
                throw new Exception($"Failed to parse AI suggestion response: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AIService] Unexpected error in GetIssueSuggestion: {ex.Message}");
                throw;
            }
        }
    }
}

