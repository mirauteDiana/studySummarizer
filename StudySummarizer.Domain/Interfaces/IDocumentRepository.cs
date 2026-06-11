using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Domain.Interfaces;

public interface IDocumentRepository
{
    Task<IEnumerable<Document>> GetAllAsync();
    Task<Document?> GetByIdAsync(Guid id);
    Task AddAsync(Document doc);
    Task DeleteAsync(Document doc);
    Task SaveChangesAsync();
}
