using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySummarizer.Application.DTOs;
using StudySummarizer.Application.Interfaces;

namespace StudySummarizer.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        return result.Match(
            userId => Ok(new { message = "User registered successfully", userId }),
            errors => Problem(errors));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var result = await _userService.LoginAsync(request);
        return result.Match(
            token => Ok(new { message = "Login successful", token }),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetProfileAsync(CurrentUserId);
        return result.Match(Ok, errors => Problem(errors));
    }
}
