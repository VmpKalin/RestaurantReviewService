using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

// Restaurant DTOs
public class CreateRestaurantRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public string? PreviewImage { get; set; }
    
    [Required]
    [MinLength(5)]
    public string Description { get; set; } = string.Empty;
    
    // Either provide coordinates directly or an address
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public string? Address { get; set; }
}

public class UpdateRestaurantRequest
{
    [MinLength(1)]
    [MaxLength(200)]
    public string? Title { get; set; }
    
    public string? PreviewImage { get; set; }
    
    [MinLength(5)]
    public string? Description { get; set; }
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public string? Address { get; set; }
}

public class RestaurantDto
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
    public string OwnerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RestaurantListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? TitleFilter { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusInMiles { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
