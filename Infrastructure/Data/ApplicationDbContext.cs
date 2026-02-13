using Microsoft.EntityFrameworkCore;
using ToptalFinialSolution.Domain.Entities;

namespace ToptalFinialSolution.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ViewedRestaurant> ViewedRestaurants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UserType).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Restaurant configuration
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Latitude).IsRequired();
            entity.Property(e => e.Longitude).IsRequired();
            entity.Property(e => e.AverageRating).HasDefaultValue(0);
            entity.Property(e => e.ReviewCount).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Owner)
                .WithMany(u => u.Restaurants)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReviewText).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasOne(e => e.Restaurant)
                .WithMany(r => r.Reviews)
                .HasForeignKey(e => e.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Reviewer)
                .WithMany(u => u.Reviews)
                .HasForeignKey(e => e.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.RestaurantId);
            entity.HasIndex(e => e.ReviewerId);
        });

        // ViewedRestaurant configuration
        modelBuilder.Entity<ViewedRestaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ViewedAt).IsRequired();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.ViewedRestaurants)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Restaurant)
                .WithMany(r => r.ViewedRestaurants)
                .HasForeignKey(e => e.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.UserId, e.RestaurantId, e.ViewedAt });
        });
    }
}
