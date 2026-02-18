using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToptalFinialSolution.API.Extensions;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{

    /// <summary>
    /// Get all reviews with pagination and filtering
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetReviews([FromQuery] ReviewListQuery query, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetReviewsAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new review (Reviewer only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Reviewer")]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var review = await reviewService.CreateReviewAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetReviews), new { restaurantId = review.RestaurantId }, review);
    }
}
