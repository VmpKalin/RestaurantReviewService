namespace ToptalFinialSolution.Application.DTOs;

public record RestaurantDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? PreviewImage { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required string Description { get; init; }
    public required double AverageRating { get; init; }
    public required int ReviewCount { get; init; }
    public required Guid OwnerId { get; init; }
    public required string OwnerName { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Distance from the search point in kilometers. Only populated when searching by location.
    /// </summary>
    public double? DistanceKm { get; init; }
}
