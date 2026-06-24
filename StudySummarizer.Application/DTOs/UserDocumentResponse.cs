using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Application.DTOs;

public class UserDocumentResponse
{
    public Guid DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
}
