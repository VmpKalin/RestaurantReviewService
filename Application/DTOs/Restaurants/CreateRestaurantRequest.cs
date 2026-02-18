using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record CreateRestaurantRequest : IValidatableObject
{
    [MinLength(1)]
    [MaxLength(200)]
    public required string Title { get; init; }

    [Url]
    public string? PreviewImage { get; init; }

    [MinLength(5)]
    public required string Description { get; init; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
    public double? Latitude { get; init; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
    public double? Longitude { get; init; }

    public string? Address { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasCoordinates = Latitude.HasValue && Longitude.HasValue;
        var hasAddress = !string.IsNullOrWhiteSpace(Address);

        if (!hasCoordinates && !hasAddress)
        {
            yield return new ValidationResult(
                "Either coordinates (Latitude and Longitude) or Address must be provided.",
                [nameof(Latitude), nameof(Longitude), nameof(Address)]);
        }

        if (Latitude.HasValue != Longitude.HasValue)
        {
            yield return new ValidationResult(
                "Both Latitude and Longitude must be provided together.",
                [nameof(Latitude), nameof(Longitude)]);
        }
    }
}
