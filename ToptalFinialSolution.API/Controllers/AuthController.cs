using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Sign up a new user
    /// </summary>
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> SignUp([FromBody] SignUpRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.SignUpAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userType = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new { userId, email, userType });
    }
}
