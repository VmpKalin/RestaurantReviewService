using Microsoft.AspNetCore.Mvc;

namespace ToptalFinialSolution.API.Filters;

/// <summary>
/// Attribute to mark actions that should track restaurant views
/// Uses TypeFilterAttribute to enable dependency injection in the filter
/// </summary>
public class TrackRestaurantViewAttribute() : TypeFilterAttribute(typeof(TrackRestaurantViewFilter));
