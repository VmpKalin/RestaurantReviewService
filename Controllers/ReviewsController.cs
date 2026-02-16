using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User not authenticated"));
    }

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
        var userId = GetCurrentUserId();
        var review = await reviewService.CreateReviewAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetReviews), new { restaurantId = review.RestaurantId }, review);
    }
}
