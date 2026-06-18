using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Domain.Entities;

public class Summary : BaseEntity
{
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;
    public SummaryType SummaryType { get; set; }
    public string Content { get; set; } = string.Empty;
}
