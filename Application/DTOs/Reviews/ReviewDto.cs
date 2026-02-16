namespace ToptalFinialSolution.Application.DTOs;

public record ReviewDto
{
    public required Guid Id { get; set; }
    public required string ReviewText { get; set; }
    public required int Rating { get; set; }
    public required Guid RestaurantId { get; set; }
    public required string RestaurantTitle { get; set; }
    public required Guid ReviewerId { get; set; }
    public required string ReviewerName { get; set; }
    public required DateTime CreatedAt { get; set; }
}
