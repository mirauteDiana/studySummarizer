using Microsoft.EntityFrameworkCore;
using StudySummarizer.Domain.Entities;
using StudySummarizer.Domain.Interfaces;
using StudySummarizer.Infrastructure.Persistence;

namespace StudySummarizer.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    // Bypasses the soft-delete query filter: the email uniqueness check must match the
    // physical UNIQUE index, which still includes soft-deleted rows. Otherwise a re-register
    // with a soft-deleted user's email passes this check and fails on SaveChanges.
    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == email);

    public async Task<bool> ExistsByUsernameAsync(string username) =>
        await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Username == username);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
