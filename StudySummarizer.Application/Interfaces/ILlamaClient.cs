using StudySummarizer.Domain.Enums;

namespace StudySummarizer.Application.Interfaces;

public interface ILlamaClient
{
    Task<string> SummarizeAsync(string text, SummaryType summaryType, CancellationToken cancellationToken = default);
}
