using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record CreateRestaurantRequest
{
    [MinLength(1)]
    [MaxLength(200)]
    public required string Title { get; init; }
    public string? PreviewImage { get; init; }
    [MinLength(5)]
    public required string Description { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Address { get; init; }
}
