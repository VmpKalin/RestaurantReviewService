using ToptalFinialSolution.Domain.Enums;

namespace ToptalFinialSolution.Application.DTOs;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required UserType UserType { get; init; }
}
