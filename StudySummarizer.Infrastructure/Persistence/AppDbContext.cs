using Microsoft.EntityFrameworkCore;
using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Document> Documents => Set<Document>();
}
