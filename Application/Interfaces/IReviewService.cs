using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IReviewService
{
    Task<PagedResult<ReviewDto>> GetReviewsAsync(ReviewListQuery query);
    Task<ReviewDto> CreateReviewAsync(CreateReviewRequest request, Guid reviewerId);
}
