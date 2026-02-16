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
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
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
}
