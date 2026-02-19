using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record LoginRequest
{
    [EmailAddress]
    public required string Email { get; init; }
    public required string Password { get; init; }
}
