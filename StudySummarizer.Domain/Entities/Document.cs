using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public DateTime UploadDate { get; set; }
}
