namespace ToptalFinialSolution.Application.DTOs;

public record AuthResponse
{
    public required string Token { get; set; }
    public required UserDto User { get; set; }
}
