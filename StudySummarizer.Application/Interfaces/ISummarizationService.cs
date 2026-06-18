using ErrorOr;
using StudySummarizer.Application.DTOs;

namespace StudySummarizer.Application.Interfaces;

public interface ISummarizationService
{
    Task<ErrorOr<Success>> StartAsync(Guid documentId, SummarizeRequest request);
    Task<ErrorOr<SummaryResponse>> GetAsync(Guid documentId);
    Task<ErrorOr<Success>> RegenerateAsync(Guid documentId, SummarizeRequest request);
}
