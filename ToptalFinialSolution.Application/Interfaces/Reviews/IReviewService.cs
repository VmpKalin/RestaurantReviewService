using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IReviewService
{
    Task<PagedResult<ReviewDto>> GetReviewsAsync(ReviewListQuery query, CancellationToken cancellationToken = default);
    Task<ReviewDto> CreateReviewAsync(CreateReviewRequest request, Guid reviewerId, CancellationToken cancellationToken = default);
}
