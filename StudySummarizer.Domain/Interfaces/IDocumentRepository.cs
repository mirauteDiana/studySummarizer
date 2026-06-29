using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Domain.Interfaces;

public interface IDocumentRepository
{
    Task<IEnumerable<Document>> GetAllByUserIdAsync(Guid userId);
    Task<Document?> GetByIdAndUserIdAsync(Guid id, Guid userId);
    Task AddAsync(Document doc);
    Task DeleteAsync(Document doc);
    Task SaveChangesAsync();
}
