using Microsoft.EntityFrameworkCore;
using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Summary> Summaries => Set<Summary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<Summary>().HasQueryFilter(s => !s.IsDeleted);

        modelBuilder.Entity<Summary>()
            .HasOne(s => s.Document)
            .WithOne()
            .HasForeignKey<Summary>(s => s.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
