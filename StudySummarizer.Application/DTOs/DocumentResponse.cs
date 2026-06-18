using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Application.DTOs;

public class DocumentResponse : BaseResponse
{
    public string Title { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public DocumentStatus Status { get; set; }
}
