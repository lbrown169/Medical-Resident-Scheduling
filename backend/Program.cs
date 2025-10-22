using DotNetEnv;
using MedicalDemo.Models;
using MedicalDemo.Services;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SchedulingMapperService>();
builder.Services.AddScoped<SchedulerService>();
builder.Services.AddScoped<PostmarkService>();
builder.Services.AddScoped<MiscService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost") ||
                origin.StartsWith("https://psycall.net") ||
                origin.StartsWith("https://www.psycall.net") ||
                origin.StartsWith("https://backend.psycall.net"))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Missing DB connection string.");
}

builder.Services.AddDbContext<MedicalContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{

    app.UseSwagger();
    app.UseSwaggerUI();
}


if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MedicalContext>();
    var seedEnv = Environment.GetEnvironmentVariable("SEED");
    if (seedEnv == "true")
    {
        DatabaseSeeder.Seed(db);
    }
}

app.Run();