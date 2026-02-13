using System.ComponentModel.DataAnnotations;

namespace ToptalFinialSolution.Application.DTOs;

// Review DTOs
public class CreateReviewRequest
{
    [Required]
    public Guid RestaurantId { get; set; }
    
    [Required]
    [MinLength(10)]
    [MaxLength(2000)]
    public string ReviewText { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public int Rating { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantTitle { get; set; } = string.Empty;
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ReviewListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? RestaurantId { get; set; }
}
