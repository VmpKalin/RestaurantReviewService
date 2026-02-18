using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record RestaurantListQuery : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;

    public string? TitleFilter { get; init; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public double? Latitude { get; init; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public double? Longitude { get; init; }

    [Range(0.1, 500, ErrorMessage = "RadiusKm must be between 0.1 and 500.")]
    public double? RadiusKm { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasAnyCoordinate = Latitude.HasValue || Longitude.HasValue || RadiusKm.HasValue;

        if (hasAnyCoordinate && !(Latitude.HasValue && Longitude.HasValue && RadiusKm.HasValue))
        {
            yield return new ValidationResult(
                "Latitude, Longitude, and RadiusKm must all be provided together for proximity search.",
                [nameof(Latitude), nameof(Longitude), nameof(RadiusKm)]);
        }
    }
}
