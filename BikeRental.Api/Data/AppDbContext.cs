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
    public DbSet<Station> Stations => Set<Station>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Station>(station =>
        {
            station.Property(s => s.Name).IsRequired().HasMaxLength(100);
            station.Property(s => s.Address).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Bike>(bike =>
        {
            bike.Property(b => b.Model).IsRequired().HasMaxLength(100);
            bike.Property(b => b.Type).HasConversion<string>().HasMaxLength(20);
            bike.Property(b => b.PricePerHour).HasPrecision(10, 2);

            // One station has many bikes. Deleting a station clears StationId.
            bike.HasOne(b => b.Station)
                .WithMany(s => s.Bikes)
                .HasForeignKey(b => b.StationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            user.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            user.Property(u => u.Email).IsRequired().HasMaxLength(200);
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Rental>(rental =>
        {
            rental.Property(r => r.HourlyPrice).HasPrecision(10, 2);
            rental.Property(r => r.TotalPrice).HasPrecision(10, 2);

            // One bike has many rentals.
            rental.HasOne(r => r.Bike)
                .WithMany(b => b.Rentals)
                .HasForeignKey(r => r.BikeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One user has many rentals.
            rental.HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pickup and return stations. Deleting a station clears these ids.
            rental.HasOne(r => r.PickupStation)
                .WithMany()
                .HasForeignKey(r => r.PickupStationId)
                .OnDelete(DeleteBehavior.SetNull);

            rental.HasOne(r => r.ReturnStation)
                .WithMany()
                .HasForeignKey(r => r.ReturnStationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Station>().HasData(
            new Station { Id = 1, Name = "Kauppatori", Address = "Kauppapuistikko, 65100 Vaasa", Latitude = 63.0960, Longitude = 21.6158 },
            new Station { Id = 2, Name = "VAMK Palosaari", Address = "Wolffintie 30, 65200 Vaasa", Latitude = 63.1046, Longitude = 21.5935 },
            new Station { Id = 3, Name = "Rautatieasema", Address = "Hovioikeudenpuistikko 26, 65100 Vaasa", Latitude = 63.0946, Longitude = 21.6209 }
        );

        modelBuilder.Entity<Bike>().HasData(
            new Bike { Id = 1, Model = "Helkama Jopo", Type = BikeType.City, PricePerHour = 3.50m, StationId = 1 },
            new Bike { Id = 2, Model = "Trek Marlin 7", Type = BikeType.Mountain, PricePerHour = 5.00m, StationId = 1 },
            new Bike { Id = 3, Model = "Tunturi E-Bike", Type = BikeType.Electric, PricePerHour = 8.00m, StationId = 2 },
            new Bike { Id = 4, Model = "Puky ZL 16", Type = BikeType.Kids, PricePerHour = 2.00m, StationId = 2 },
            new Bike { Id = 5, Model = "Cannondale Quick", Type = BikeType.City, PricePerHour = 4.00m, StationId = 3, InMaintenance = true }
        );

        // Seed users. Password for all: Password123!
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "Matti",
                LastName = "Meikäläinen",
                Email = "matti@example.com",
                Role = UserRole.Customer,
                PasswordHash = "AQAAAAIAAYagAAAAEDO7IAsxiuY3CK48XjKrNBPFYgo9owSCt0Zi9jDJqjCf6tHhVeQkQS2TX9nbUBOD+w=="
            },
            new User
            {
                Id = 2,
                FirstName = "Liisa",
                LastName = "Virtanen",
                Email = "liisa@example.com",
                Role = UserRole.Customer,
                PasswordHash = "AQAAAAIAAYagAAAAEP8YpShRA5TmKFSjYIC8/4iyD2DaB/Y3iVq82EXvujZEku8U9+T71ly4qqw+XDz4DA=="
            },
            new User
            {
                Id = 3,
                FirstName = "Anna",
                LastName = "Admin",
                Email = "admin@example.com",
                Role = UserRole.Admin,
                PasswordHash = "AQAAAAIAAYagAAAAEFJB5Jvd7gioD9l0/n06z0vb+Q5cO3AMi+wrva+7ZyEd6uiv9STbc+T2uP+5DGOwnQ=="
            }
        );

        // SQLite does not store DateTimeKind. These converters mark every
        // date read from the database as UTC, so JSON times end in "Z".
        // Kept last so they apply to every DateTime property.
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