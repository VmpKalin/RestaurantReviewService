namespace ToptalFinialSolution.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PreviewImage { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ViewedRestaurant> ViewedRestaurants { get; set; } = new List<ViewedRestaurant>();
}
