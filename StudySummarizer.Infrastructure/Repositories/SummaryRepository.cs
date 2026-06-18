using Microsoft.EntityFrameworkCore;
using StudySummarizer.Domain.Entities;
using StudySummarizer.Domain.Interfaces;
using StudySummarizer.Infrastructure.Persistence;

namespace StudySummarizer.Infrastructure.Repositories;

public class SummaryRepository : ISummaryRepository
{
    private readonly AppDbContext _context;

    public SummaryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Summary?> GetByDocumentIdAsync(Guid documentId) =>
        _context.Summaries.FirstOrDefaultAsync(s => s.DocumentId == documentId);

    public async Task AddAsync(Summary summary) =>
        await _context.Summaries.AddAsync(summary);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
