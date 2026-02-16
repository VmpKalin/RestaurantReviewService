using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record UpdateRestaurantRequest
{
    [MinLength(1)]
    [MaxLength(200)]
    public string? Title { get; set; }
    public string? PreviewImage { get; set; }
    [MinLength(5)]
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Address { get; set; }
}
