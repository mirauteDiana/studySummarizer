using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Domain.Entities;

public class Document : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
}
