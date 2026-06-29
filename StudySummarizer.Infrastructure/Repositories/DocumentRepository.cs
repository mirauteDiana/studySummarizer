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

    public async Task<IEnumerable<Document>> GetAllByUserIdAsync(Guid userId) =>
        await _context.Documents.Where(d => d.UserId == userId).ToListAsync();

    public async Task<Document?> GetByIdAndUserIdAsync(Guid id, Guid userId) =>
        await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

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
