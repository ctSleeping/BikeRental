using BikeRental.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BikeRental.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Bike> Bikes => Set<Bike>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Rental> Rentals => Set<Rental>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Bike ---
        modelBuilder.Entity<Bike>(bike =>
        {
            bike.Property(b => b.Model).IsRequired().HasMaxLength(100);
            bike.Property(b => b.Type).HasConversion<string>().HasMaxLength(20);
            bike.Property(b => b.PricePerHour).HasPrecision(10, 2);
        });

        // --- User ---
        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            user.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            user.Property(u => u.Email).IsRequired().HasMaxLength(200);
            user.HasIndex(u => u.Email).IsUnique();          // same email can't exist twice
            user.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });

        // --- Rental + relationships ---
        modelBuilder.Entity<Rental>(rental =>
        {
            rental.Property(r => r.HourlyPrice).HasPrecision(10, 2);
            rental.Property(r => r.TotalPrice).HasPrecision(10, 2);

            // A rental has one bike; a bike has many rentals.
            rental.HasOne(r => r.Bike)
                .WithMany(b => b.Rentals)
                .HasForeignKey(r => r.BikeId)
                .OnDelete(DeleteBehavior.Restrict);

            // A rental has one user; a user has many rentals.
            rental.HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Seed data ---
        modelBuilder.Entity<Bike>().HasData(
            new Bike { Id = 1, Model = "Helkama Jopo", Type = BikeType.City, PricePerHour = 3.50m },
            new Bike { Id = 2, Model = "Trek Marlin 7", Type = BikeType.Mountain, PricePerHour = 5.00m },
            new Bike { Id = 3, Model = "Tunturi E-Bike", Type = BikeType.Electric, PricePerHour = 8.00m },
            new Bike { Id = 4, Model = "Puky ZL 16", Type = BikeType.Kids, PricePerHour = 2.00m },
            new Bike { Id = 5, Model = "Cannondale Quick", Type = BikeType.City, PricePerHour = 4.00m, InMaintenance = true }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, FirstName = "Matti", LastName = "Meikäläinen", Email = "matti@example.com", PasswordHash = "not-set", Role = UserRole.Customer },
            new User { Id = 2, FirstName = "Liisa", LastName = "Virtanen", Email = "liisa@example.com", PasswordHash = "not-set", Role = UserRole.Customer },
            new User { Id = 3, FirstName = "Anna", LastName = "Admin", Email = "admin@example.com", PasswordHash = "not-set", Role = UserRole.Admin }
        );

        // --- UTC fix: keep this LAST ---
        // SQLite forgets that a date was UTC. This marks every date read
        // from the database as UTC, so JSON times end in "Z".
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            toDb => toDb,
            fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));

        var nullableUtcConverter = new ValueConverter<DateTime?, DateTime?>(
            toDb => toDb,
            fromDb => fromDb.HasValue
                ? DateTime.SpecifyKind(fromDb.Value, DateTimeKind.Utc)
                : null);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(utcConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableUtcConverter);
            }
        }
    }
}