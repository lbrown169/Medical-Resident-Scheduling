using DotNetEnv;
using MedicalDemo.Models;
using MedicalDemo.Services;
using Microsoft.EntityFrameworkCore;

// Load .env file
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SchedulingMapperService>();
builder.Services.AddScoped<SchedulerService>();
builder.Services.AddScoped<PostmarkService>();
builder.Services.AddScoped<MiscService>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            origin.StartsWith("https://psycall.net") ||
            origin.StartsWith("https://www.psycall.net") ||
            origin.StartsWith("https://backend.psycall.net") ||
            origin.StartsWith("http://localhost"))
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Connect to DB
string? MySqlConnectString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
Console.WriteLine($"Loaded DB_CONNECTION_STRING: {MySqlConnectString}");
if (string.IsNullOrEmpty(MySqlConnectString))
{
    throw new Exception("Database connection string is not configured. Please set DB_CONNECTION_STRING environment variable.");
}

builder.Services.AddDbContext<MedicalContext>(options =>
{
    Console.WriteLine("Attempting to connect to database...");
    options.UseMySql(MySqlConnectString, ServerVersion.AutoDetect(MySqlConnectString));
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// 1) Redirect HTTP → HTTPS
app.UseHttpsRedirection();

// 2) Routing must come before CORS
app.UseRouting();

// 3) Apply CORS policy
app.UseCors("AllowFrontend");

// 4) Map your controllers
app.MapControllers();

// 5) Configure host port (from env or default)
string port = Environment.GetEnvironmentVariable("BACKEND_PORT") ?? "5109";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    MedicalContext db = scope.ServiceProvider.GetRequiredService<MedicalContext>();

    string? seedEnv = Environment.GetEnvironmentVariable("SEED");
    if (seedEnv == "true")
    {
        DatabaseSeeder.Seed(db);
    }
}

app.Run();