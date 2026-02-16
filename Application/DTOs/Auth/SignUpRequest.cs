using System.ComponentModel.DataAnnotations;
using ToptalFinialSolution.Domain.Enums;

namespace ToptalFinialSolution.Application.DTOs;

public record SignUpRequest
{
    [EmailAddress]
    public required string Email { get; init; }
    [MinLength(8)]
    public required string Password { get; init; }
    [MinLength(2)]
    public required string FullName { get; init; }
    public required UserType UserType { get; init; }
}
