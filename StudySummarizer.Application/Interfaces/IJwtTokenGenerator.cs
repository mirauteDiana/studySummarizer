using StudySummarizer.Domain.Entities;

namespace StudySummarizer.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
