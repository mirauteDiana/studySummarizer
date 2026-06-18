using Microsoft.EntityFrameworkCore;
using StudySummarizer.Domain.Entities;
using StudySummarizer.Domain.Interfaces;
using StudySummarizer.Infrastructure.Persistence;

namespace StudySummarizer.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Document>> GetAllAsync() =>
        await _context.Documents.ToListAsync();

    public async Task<Document?> GetByIdAsync(Guid id) =>
        await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

    public async Task AddAsync(Document doc) =>
        await _context.Documents.AddAsync(doc);

    public Task DeleteAsync(Document doc)
    {
        doc.IsDeleted = true;
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
