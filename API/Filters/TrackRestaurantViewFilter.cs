using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.API.Filters;

/// <summary>
/// Action filter that tracks restaurant views for authenticated reviewers
/// Executes after the action and only records view if the response is successful
/// </summary>
public class TrackRestaurantViewFilter : IAsyncActionFilter
{
    private readonly IViewedRestaurantService _viewedRestaurantService;
    private readonly ILogger<TrackRestaurantViewFilter> _logger;

    public TrackRestaurantViewFilter(
        IViewedRestaurantService viewedRestaurantService,
        ILogger<TrackRestaurantViewFilter> logger)
    {
        _viewedRestaurantService = viewedRestaurantService;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Execute the action first
        var executedContext = await next();

        // Only track if action succeeded with OK result
        if (executedContext.Result is OkObjectResult)
        {
            var httpContext = executedContext.HttpContext;
            var user = httpContext.User;

            // Check if user is authenticated and is a reviewer
            if (user.Identity?.IsAuthenticated == true && user.IsInRole("Reviewer"))
            {
                // Extract restaurant ID from action arguments
                if (executedContext.ActionDescriptor.Parameters
                    .Any(p => p.Name == "id" && p.ParameterType == typeof(Guid)))
                {
                    if (context.ActionArguments.TryGetValue("id", out var idValue) && 
                        idValue is Guid restaurantId)
                    {
                        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
                        {
                            try
                            {
                                // Record the view
                                await _viewedRestaurantService.RecordViewAsync(userId, restaurantId);
                            }
                            catch (Exception ex)
                            {
                                // Swallow failures - viewing tracking should not break the response
                                _logger.LogWarning(ex,
                                    "Failed to record restaurant view for user {UserId} and restaurant {RestaurantId}",
                                    userId, restaurantId);
                            }
                        }
                    }
                }
            }
        }
    }
}
