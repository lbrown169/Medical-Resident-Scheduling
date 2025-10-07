using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedicalDemo.Models;

// Adjust the namespace to match your actual structure
public class MedicalContextFactory : IDesignTimeDbContextFactory<MedicalContext>
{
    public MedicalContext CreateDbContext(string[] args)
    {
        // Get the environment
        string environment
            = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
              "Development";

        // Build configuration
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", true)
            .Build();

        // Get connection string
        string? connectionString
            = configuration.GetConnectionString("MySqlConn");

        // Create options builder
        DbContextOptionsBuilder<MedicalContext> builder = new();
        builder.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString));

        // 3. Return a new instance of your DbContext
        return new MedicalContext(builder.Options);
    }
}