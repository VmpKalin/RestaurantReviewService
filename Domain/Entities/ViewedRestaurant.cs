namespace ToptalFinialSolution.Domain.Entities;

public class ViewedRestaurant
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public DateTime ViewedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Restaurant Restaurant { get; set; } = null!;
}
