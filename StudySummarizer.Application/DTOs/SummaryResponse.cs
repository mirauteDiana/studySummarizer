namespace StudySummarizer.Application.DTOs;

public class SummaryResponse : BaseResponse
{
    public Guid DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
