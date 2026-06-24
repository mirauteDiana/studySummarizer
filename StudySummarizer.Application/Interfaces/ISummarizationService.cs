using ErrorOr;
using StudySummarizer.Application.DTOs;

namespace StudySummarizer.Application.Interfaces;

public interface ISummarizationService
{
    Task<ErrorOr<Success>> StartAsync(Guid documentId, SummarizeRequest request, Guid userId);
    Task<ErrorOr<SummaryResponse>> GetAsync(Guid documentId, Guid userId);
    Task<ErrorOr<Success>> RegenerateAsync(Guid documentId, SummarizeRequest request, Guid userId);
}
