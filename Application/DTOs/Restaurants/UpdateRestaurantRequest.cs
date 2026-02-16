using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record UpdateRestaurantRequest
{
    [MinLength(1)]
    [MaxLength(200)]
    public string? Title { get; init; }
    public string? PreviewImage { get; init; }
    [MinLength(5)]
    public string? Description { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Address { get; init; }
}
