using System.ComponentModel.DataAnnotations;
using ToptalFinialSolution.Domain.Enums;

namespace ToptalFinialSolution.Application.DTOs;

public record SignUpRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    [MinLength(8)]
    public required string Password { get; set; }
    [MinLength(2)]
    public required string FullName { get; set; }
    public required UserType UserType { get; set; }
}
