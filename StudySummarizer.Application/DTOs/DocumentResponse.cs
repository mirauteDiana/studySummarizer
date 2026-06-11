using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Application.DTOs;

public class DocumentResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public DocumentStatus Status { get; set; }
    public DateTime UploadDate { get; set; }
}
