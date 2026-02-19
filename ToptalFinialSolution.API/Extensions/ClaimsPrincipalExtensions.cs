using System.Security.Claims;

namespace ToptalFinialSolution.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(value ?? throw new UnauthorizedAccessException("User not authenticated"));
    }
}
