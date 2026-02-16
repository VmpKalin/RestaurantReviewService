using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record CreateReviewRequest
{
    public required Guid RestaurantId { get; init; }
    [MinLength(10)]
    [MaxLength(500)]
    public required string ReviewText { get; init; }
    [Range(1, 5)]
    public required int Rating { get; init; }
}
