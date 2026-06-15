using Microsoft.AspNetCore.Http;

namespace StudySummarizer.Application.DTOs;

public class UploadDocumentRequest
{
    public string Title { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
}
