using ErrorOr;
using StudySummarizer.Application.DTOs;

namespace StudySummarizer.Application.Interfaces;

public interface IUserService
{
    Task<ErrorOr<Guid>> RegisterAsync(RegisterUserRequest request);
    Task<ErrorOr<string>> LoginAsync(LoginUserRequest request);
    Task<ErrorOr<UserProfileResponse>> GetProfileAsync(Guid userId);
}
