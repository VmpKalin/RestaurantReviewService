using NetTopologySuite.Geometries;

namespace ToptalFinialSolution.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PreviewImage { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>
    /// PostGIS geography point (SRID 4326) used for spatial queries.
    /// Kept in sync with Latitude/Longitude properties.
    /// </summary>
    public Point? Location { get; set; }

    public string Description { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public Guid OwnerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>
    /// Sets both lat/lng properties and the PostGIS Location point.
    /// </summary>
    public void SetCoordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Location = new Point(longitude, latitude) { SRID = 4326 };
    }

    /// <summary>
    /// Incrementally updates AverageRating and ReviewCount when a new review is added.
    /// Avoids extra DB queries by computing from the stored denormalized values.
    /// </summary>
    public void AddReviewRating(int rating)
    {
        AverageRating = Math.Round(((AverageRating * ReviewCount) + rating) / (ReviewCount + 1), 2);
        ReviewCount++;
    }

    /// <summary>
    /// Recalculates AverageRating and ReviewCount from the loaded Reviews collection.
    /// Requires Reviews navigation property to be loaded.
    /// </summary>
    public void RecalculateRating()
    {
        if (Reviews.Count == 0)
        {
            AverageRating = 0;
            ReviewCount = 0;
            return;
        }

        ReviewCount = Reviews.Count;
        AverageRating = Math.Round(Reviews.Average(r => r.Rating), 2);
    }
}
