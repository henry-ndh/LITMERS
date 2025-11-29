using Base.API;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Main.API.Controllers
{
    [ApiController]
    public class UploadController : BaseAPIController
    {
        private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
        }

        [RequestSizeLimit(100 * 1024 * 1024)]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return SaveError("File không hợp lệ hoặc trống.");
                }

                using (var httpClient = new HttpClient())
                {
                    using (var formData = new MultipartFormDataContent())
                    {
                        var stream = file.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType =
                            new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                        formData.Add(fileContent, "file", file.FileName);

                        var response = await httpClient.PostAsync("https://cdn.webradarcs2.com/upload", formData);

                        var responseBody = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError("Upload CDN thất bại: {0}", responseBody);
                            return SaveError($"Upload CDN thất bại. Lỗi: {responseBody}");
                        }

                        var innerResponse = JsonSerializer.Deserialize<InnerResponse>(responseBody);

                        _logger.LogInformation("Upload CDN thành công, URL = {0}", innerResponse.data.downloadUrl);

                        return SaveSuccess(innerResponse.data);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi upload file");
                return SaveError(ex.Message);
            }
        }

        public class InnerResponse
        {
            public string message { get; set; }
            public bool success { get; set; }
            public int statusCode { get; set; }
            public InnerData data { get; set; }
        }

        public class InnerData
        {
            public string downloadUrl { get; set; }
            public string fileName { get; set; }
            public double fileSizeMb { get; set; }
            public string fileType { get; set; }
            public DateTime uploadTime { get; set; }
        }
    }
}
