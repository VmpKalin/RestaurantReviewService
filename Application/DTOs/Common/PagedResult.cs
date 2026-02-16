namespace ToptalFinialSolution.Application.DTOs;

public record PagedResult<T>
{
    public required IEnumerable<T> Items { get; set; }
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public required int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
