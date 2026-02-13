namespace ToptalFinialSolution.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public Guid RestaurantId { get; set; }
    public Guid ReviewerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Restaurant Restaurant { get; set; } = null!;
    public User Reviewer { get; set; } = null!;
}
