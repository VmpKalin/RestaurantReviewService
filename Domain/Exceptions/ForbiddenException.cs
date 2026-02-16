namespace ToptalFinialSolution.Domain.Exceptions;

/// <summary>
/// Thrown when an authenticated user attempts an action they are not authorized to perform (HTTP 403).
/// </summary>
public class ForbiddenException(string message) : Exception(message);
