using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Domain.Interfaces;

public interface ISummaryRepository
{
    Task<Summary?> GetByDocumentIdAsync(Guid documentId);
    Task AddAsync(Summary summary);
    Task SaveChangesAsync();
}
