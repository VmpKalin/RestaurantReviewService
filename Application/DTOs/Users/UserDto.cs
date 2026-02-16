using ToptalFinialSolution.Domain.Enums;

namespace ToptalFinialSolution.Application.DTOs;

public record UserDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required UserType UserType { get; set; }
}
