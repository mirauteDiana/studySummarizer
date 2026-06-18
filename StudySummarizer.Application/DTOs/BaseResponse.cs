namespace StudySummarizer.Application.DTOs;

public abstract class BaseResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
