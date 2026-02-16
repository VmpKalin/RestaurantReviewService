using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using ToptalFinialSolution.Application.Interfaces;

namespace ToptalFinialSolution.API.Filters;

/// <summary>
/// Action filter that tracks restaurant views for authenticated reviewers.
/// Executes after the action and only records view if the response is successful.
/// </summary>
public class TrackRestaurantViewFilter(
    IViewedRestaurantService viewedRestaurantService,
    ILogger<TrackRestaurantViewFilter> logger)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Result is not OkObjectResult)
        {
            return;
        }

        var user = executedContext.HttpContext.User;

        if (user.Identity?.IsAuthenticated is not true || !user.IsInRole("Reviewer"))
        {
            return;
        }

        if (!context.ActionArguments.TryGetValue("id", out var idValue) || idValue is not Guid restaurantId)
        {
            return;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        try
        {
            var cancellationToken = executedContext.HttpContext.RequestAborted;
            await viewedRestaurantService.RecordViewAsync(userId, restaurantId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to record restaurant view for user {UserId} and restaurant {RestaurantId}",
                userId, restaurantId);
        }
    }
}
