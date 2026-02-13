using ToptalFinialSolution.Domain.Enums;

namespace ToptalFinialSolution.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ViewedRestaurant> ViewedRestaurants { get; set; } = new List<ViewedRestaurant>();
}
