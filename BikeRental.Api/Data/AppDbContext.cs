using BikeRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Bike> Bikes => Set<Bike>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bike>(bike =>
        {
            bike.Property(b => b.Model).IsRequired().HasMaxLength(100);
            bike.Property(b => b.Type).HasConversion<string>().HasMaxLength(20);
            bike.Property(b => b.PricePerHour).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Bike>().HasData(
            new Bike { Id = 1, Model = "Helkama Jopo", Type = BikeType.City, PricePerHour = 3.50m },
            new Bike { Id = 2, Model = "Trek Marlin 7", Type = BikeType.Mountain, PricePerHour = 5.00m },
            new Bike { Id = 3, Model = "Tunturi E-Bike", Type = BikeType.Electric, PricePerHour = 8.00m },
            new Bike { Id = 4, Model = "Puky ZL 16", Type = BikeType.Kids, PricePerHour = 2.00m },
            new Bike { Id = 5, Model = "Cannondale Quick", Type = BikeType.City, PricePerHour = 4.00m, InMaintenance = true }
        );
    }
}