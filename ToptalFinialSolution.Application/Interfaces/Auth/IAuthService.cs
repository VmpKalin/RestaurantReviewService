using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    string GenerateJwtToken(Guid userId, string email, string userType);
}
