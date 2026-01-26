using DotNetEnv;
using MedicalDemo;
using MedicalDemo.Models;
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
    MedicalContext db
        = scope.ServiceProvider.GetRequiredService<MedicalContext>();

    if (app.Environment.IsDevelopment())
    {
        db.Database.Migrate();
        DatabaseSeeder.Seed(db);
    }
}

app.Run();