using ErrorOr;
using Microsoft.AspNetCore.Identity;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Application.Interfaces;
using StudySummarizer.Domain.Entities;
using StudySummarizer.Domain.Interfaces;

namespace StudySummarizer.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<Guid>> RegisterAsync(RegisterUserRequest request)
    {
        var email = request.Email.ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(email))
            return Error.Conflict("User.EmailTaken", $"A user with email '{request.Email}' already exists.");

        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            return Error.Conflict("User.UsernameTaken", $"Username '{request.Username}' is already taken.");

        var user = new User
        {
            Username = request.Username,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user.Id;
    }

    public async Task<ErrorOr<string>> LoginAsync(LoginUserRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant());
        if (user is null)
            return Error.Unauthorized("User.InvalidCredentials", "Invalid email or password.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return Error.Unauthorized("User.InvalidCredentials", "Invalid email or password.");

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _userRepository.SaveChangesAsync();
        }

        return _jwtTokenGenerator.GenerateToken(user);
    }

    public async Task<ErrorOr<UserProfileResponse>> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Error.NotFound("User.NotFound", $"User {userId} was not found.");

        return new UserProfileResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            RegisteredAt = user.CreatedAt
        };
    }
}
