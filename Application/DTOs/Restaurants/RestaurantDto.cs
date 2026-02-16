namespace ToptalFinialSolution.Application.DTOs;

public record RestaurantDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? PreviewImage { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required string Description { get; set; }
    public required double AverageRating { get; set; }
    public required int ReviewCount { get; set; }
    public required Guid OwnerId { get; set; }
    public required string OwnerName { get; set; }
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// Distance from the search point in kilometers. Only populated when searching by location.
    /// </summary>
    public double? DistanceKm { get; set; }
}
