using BikeRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Bike> Bikes => Set<Bike>();
}