using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToptalFinialSolution.API.Filters;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RestaurantsController(
    IRestaurantService restaurantService,
    IViewedRestaurantService viewedRestaurantService)
    : ControllerBase
{
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User not authenticated"));
    }

    /// <summary>
    /// Get all restaurants with pagination and filtering
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<RestaurantDto>>> GetRestaurants([FromQuery] RestaurantListQuery query)
    {
        var result = await restaurantService.GetRestaurantsAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific restaurant by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [TrackRestaurantView]
    public async Task<ActionResult<RestaurantDto>> GetRestaurant(Guid id)
    {
        var restaurant = await restaurantService.GetRestaurantByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound(new { message = "Restaurant not found" });
        }

        return Ok(restaurant);
    }

    /// <summary>
    /// Create a new restaurant (Owner only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<RestaurantDto>> CreateRestaurant([FromBody] CreateRestaurantRequest request)
    {
        var userId = GetCurrentUserId();
        var restaurant = await restaurantService.CreateRestaurantAsync(request, userId);
        return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, restaurant);
    }

    /// <summary>
    /// Update a restaurant (Owner only, own restaurants)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<RestaurantDto>> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantRequest request)
    {
        var userId = GetCurrentUserId();
        var restaurant = await restaurantService.UpdateRestaurantAsync(id, request, userId);
        return Ok(restaurant);
    }

    /// <summary>
    /// Delete a restaurant (Owner only, own restaurants)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult> DeleteRestaurant(Guid id)
    {
        var userId = GetCurrentUserId();
        await restaurantService.DeleteRestaurantAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Get recently viewed restaurants (Reviewer only)
    /// </summary>
    [HttpGet("recently-viewed")]
    [Authorize(Roles = "Reviewer")]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRecentlyViewed()
    {
        var userId = GetCurrentUserId();
        var restaurants = await viewedRestaurantService.GetRecentlyViewedAsync(userId);
        return Ok(restaurants);
    }
}
