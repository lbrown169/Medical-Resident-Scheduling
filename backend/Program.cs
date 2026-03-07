using DotNetEnv;
using MedicalDemo;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using Microsoft.EntityFrameworkCore;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args)
    .AddStandardConfiguration(args)
    .AddLoggingConfiguration()
    .AddApplicationServices()
    .AddApiConfiguration();

// Add services to the container

WebApplication app = builder.Build();
app.ConfigureControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    MedicalContext db =
        scope.ServiceProvider.GetRequiredService<MedicalContext>();

    int retries = 10;
    while (retries > 0)
    {
        try
        {
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                db.Database.Migrate();
                DatabaseSeeder seeder =
                    scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                await seeder.Seed(db);
            }
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Database connection failed. Retrying... ({retries} retries left)");

            if (retries == 0)
            {
                throw;
            }

            await Task.Delay(5000);
        }
    }
}

app.Run();