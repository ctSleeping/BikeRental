using System.Text.Json.Serialization;
using BikeRental.Api.Common;
using BikeRental.Api.Data;
using BikeRental.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace BikeRental.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddScoped<IBikeService, BikeService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRentalService, RentalService>();

            var app = builder.Build();

            // apply any pending migrations at startup.
            // A fresh clone gets its database (with seed data) automatically,
            // so "dotnet run" alone is enough. If the database is already
            // up to date, this does nothing.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // Must be first, so it catches errors from everything below it
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}