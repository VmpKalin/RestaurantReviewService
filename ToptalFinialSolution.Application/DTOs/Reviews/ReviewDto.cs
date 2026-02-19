namespace ToptalFinialSolution.Application.DTOs;

public record ReviewDto
{
    public required Guid Id { get; init; }
    public required string ReviewText { get; init; }
    public required int Rating { get; init; }
    public required Guid RestaurantId { get; init; }
    public required string RestaurantTitle { get; init; }
    public required Guid ReviewerId { get; init; }
    public required string ReviewerName { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
