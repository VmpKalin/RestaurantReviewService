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
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetReviews([FromQuery] ReviewListQuery query)
    {
        var result = await reviewService.GetReviewsAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new review (Reviewer only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Reviewer")]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var review = await reviewService.CreateReviewAsync(request, userId);
            return CreatedAtAction(nameof(GetReviews), new { restaurantId = review.RestaurantId }, review);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
