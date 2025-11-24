using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

/// <summary>
/// We use this for the local migrations. Reason being is that our DI layer will not start the app
/// if certain configs are missing (For obvious reasons). EF Core cannot access container env vars
/// so we have to do this "abstraction" for local migrations
/// </summary>

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string connectionString = "";
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (environment == "Development")
        {
            connectionString = "Host=localhost;Port=55432;Database=postgres;User ID=postgres;Password=postgres";
        }

        else if (environment == "Testing")
        {
            connectionString = "Host=localhost;Port=65432;Database=postgres;User ID=postgres;Password=postgres";
        }

        else
        {
            Environment.GetEnvironmentVariable("ProdDbConnectionString");
        }

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}