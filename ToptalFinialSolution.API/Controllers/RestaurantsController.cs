using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToptalFinialSolution.API.Extensions;
using ToptalFinialSolution.API.Filters;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RestaurantsController(
    IRestaurantService restaurantService,
    IViewedRestaurantService viewedRestaurantService)
    : ControllerBase
{

    /// <summary>
    /// Get all restaurants with pagination and filtering
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<RestaurantDto>>> GetRestaurants([FromQuery] RestaurantListQuery query, CancellationToken cancellationToken)
    {
        var result = await restaurantService.GetRestaurantsAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific restaurant by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [TrackRestaurantView]
    public async Task<ActionResult<RestaurantDto>> GetRestaurant(Guid id, CancellationToken cancellationToken)
    {
        var restaurant = await restaurantService.GetRestaurantByIdAsync(id, cancellationToken);
        if (restaurant is null)
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
    public async Task<ActionResult<RestaurantDto>> CreateRestaurant([FromBody] CreateRestaurantRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var restaurant = await restaurantService.CreateRestaurantAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, restaurant);
    }

    /// <summary>
    /// Update a restaurant (Owner only, own restaurants)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<RestaurantDto>> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var restaurant = await restaurantService.UpdateRestaurantAsync(id, request, userId, cancellationToken);
        return Ok(restaurant);
    }

    /// <summary>
    /// Delete a restaurant (Owner only, own restaurants)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult> DeleteRestaurant(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await restaurantService.DeleteRestaurantAsync(id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get recently viewed restaurants (Reviewer only)
    /// </summary>
    [HttpGet("recently-viewed")]
    [Authorize(Roles = "Reviewer")]
    public async Task<ActionResult<IReadOnlyList<RestaurantDto>>> GetRecentlyViewed(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var restaurants = await viewedRestaurantService.GetRecentlyViewedAsync(userId, cancellationToken);
        return Ok(restaurants);
    }
}
