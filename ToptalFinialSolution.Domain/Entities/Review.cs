namespace ToptalFinialSolution.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public int Rating { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid ReviewerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Navigation properties
    public Restaurant Restaurant { get; set; } = null!;
    public User Reviewer { get; set; } = null!;
}
