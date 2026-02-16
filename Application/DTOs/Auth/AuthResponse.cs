namespace ToptalFinialSolution.Application.DTOs;

public record AuthResponse
{
    public required string Token { get; init; }
    public required UserDto User { get; init; }
}
