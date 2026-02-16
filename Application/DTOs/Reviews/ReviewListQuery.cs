namespace ToptalFinialSolution.Application.DTOs;

public record ReviewListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? RestaurantId { get; set; }
}
