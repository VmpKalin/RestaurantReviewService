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
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    private readonly IViewedRestaurantService _viewedRestaurantService;

    public RestaurantsController(
        IRestaurantService restaurantService,
        IViewedRestaurantService viewedRestaurantService)
    {
        _restaurantService = restaurantService;
        _viewedRestaurantService = viewedRestaurantService;
    }

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
        var result = await _restaurantService.GetRestaurantsAsync(query);
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
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
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
        try
        {
            var userId = GetCurrentUserId();
            var restaurant = await _restaurantService.CreateRestaurantAsync(request, userId);
            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, restaurant);
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

    /// <summary>
    /// Update a restaurant (Owner only, own restaurants)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<RestaurantDto>> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var restaurant = await _restaurantService.UpdateRestaurantAsync(id, request, userId);
            return Ok(restaurant);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a restaurant (Owner only, own restaurants)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult> DeleteRestaurant(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _restaurantService.DeleteRestaurantAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Get recently viewed restaurants (Reviewer only)
    /// </summary>
    [HttpGet("recently-viewed")]
    [Authorize(Roles = "Reviewer")]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRecentlyViewed()
    {
        try
        {
            var userId = GetCurrentUserId();
            var restaurants = await _viewedRestaurantService.GetRecentlyViewedAsync(userId);
            return Ok(restaurants);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
