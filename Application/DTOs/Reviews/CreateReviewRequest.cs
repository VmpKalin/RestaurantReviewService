using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

public record CreateReviewRequest
{
    public required Guid RestaurantId { get; set; }
    [MinLength(10)]
    [MaxLength(500)]
    public required string ReviewText { get; set; }
    [Range(1, 5)]
    public required int Rating { get; set; }
}
