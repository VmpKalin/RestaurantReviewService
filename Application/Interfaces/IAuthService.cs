using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> SignUpAsync(SignUpRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    string GenerateJwtToken(Guid userId, string email, string userType);
}
