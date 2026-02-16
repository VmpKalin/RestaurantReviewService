Issues Found
1. Recently Viewed Order Not Preserved (Bug)
In ViewedRestaurantService.GetRecentlyViewedAsync, the restaurant IDs come from Redis in "most recently viewed first" order, but the DB query doesn't preserve that order:
ViewedRestaurantService.cs
Lines 49-53
        var restaurants = await context.Restaurants            .Where(r => restaurantIds.Contains(r.Id))            .Include(r => r.Owner)            .ToListAsync(cancellationToken);
The WHERE IN clause returns rows in arbitrary database order, so the "last 10 viewed" won't actually be sorted chronologically. The fix is to reorder results client-side to match the Redis ordering.
2. Password Hashing Is Insecure (Security)
The AuthService uses plain SHA256 for password hashing:
AuthService.cs
Lines 109-113
    private static string HashPassword(string password)    {        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));        return Convert.ToBase64String(hashedBytes);    }
SHA256 is a general-purpose hash, not a password hashing algorithm. It has no salting and is extremely fast (vulnerable to brute-force). The industry standard is BCrypt, PBKDF2, or Argon2.
3. Missing Input Validation on Coordinates/Radius
CreateRestaurantRequest and RestaurantListQuery don't validate coordinate ranges or radius values:
Latitude should be [-90, 90]
Longitude should be [-180, 180]
RadiusKm should be positive (and ideally capped at a reasonable max)
4. No DTO-Level Enforcement of "Coordinates OR Address Required"
The spec says coordinates are required but can be provided via address. Currently the "either coordinates or address" rule is only enforced in the service layer. A IValidatableObject implementation on CreateRestaurantRequest would give better API error responses (400 with model state errors) instead of a generic InvalidOperationException.
Want me to fix these issues? The recently-viewed ordering bug and the password hashing are the most impactful.