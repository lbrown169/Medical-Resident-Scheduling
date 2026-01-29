using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MedicalDemo.Algorithm;
using MedicalDemo.Converters;
using MedicalDemo.Interfaces;
using MedicalDemo.Models;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using MedicalDemo.Services.EmailSendServices;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MedicalDemo;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddStandardConfiguration(
        this WebApplicationBuilder builder, string[] args)
    {
        builder.Configuration
            .AddJsonFile(@"Configuration/appsettings.json", false, true)
            .AddJsonFile(
                @$"Configuration/appsettings.{builder.Environment.EnvironmentName}.json",
                true)
            .AddCommandLine(args);

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<SchedulingMapperService>();
        builder.Services.AddScoped<SchedulerService>();
        builder.Services.AddScoped<AlgorithmService>();
        builder.Services.AddScoped<DatabaseSeeder>();

        // Converters
        builder.Services.AddScoped<AdminConverter>();
        builder.Services.AddScoped<AnnouncementConverter>();
        builder.Services.AddScoped<BlackoutConverter>();
        builder.Services.AddScoped<DateConverter>();
        builder.Services.AddScoped<ResidentConverter>();
        builder.Services.AddScoped<ScheduleConverter>();

        // Email
        if (builder.Environment.IsDevelopment())
        {
            builder.Services
                .AddScoped<IEmailSendService, DevelopmentEmailSendService>();
        }
        else
        {
            builder.Services
                .AddScoped<IEmailSendService, MailgunEmailSendService>();
        }

        builder.Services.AddHttpClient(nameof(MailgunEmailSendService),
            client =>
            {
                string? apiKey
                    = builder.Configuration.GetValue<string>("Mailgun:ApiKey");
                string? domain
                    = builder.Configuration.GetValue<string>("Mailgun:Domain");
                if (apiKey == null || domain == null)
                {
                    apiKey = Environment
                        .GetEnvironmentVariable("MailgunApiKey");
                    domain = Environment
                        .GetEnvironmentVariable("MailgunDomain");

                    if (apiKey == null || domain == null)
                    {
                        throw new Exception(
                            "Mailgun API key/domain was not set, but HTTP client was created"
                        );
                    }
                }

                string base64Auth
                    = Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($"api:{apiKey}"));

                client.BaseAddress
                    = new Uri($"https://api.mailgun.net/v3/{domain}/messages");
                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Basic", base64Auth);
            });

        builder.Services.AddDbContext<MedicalContext>((sp, options) =>
        {
            string? mySqlConnectionString = Environment.GetEnvironmentVariable(
                    "DB_CONNECTION_STRING");
            ILogger<MedicalContext> logger
                = sp.GetRequiredService<ILogger<MedicalContext>>();

            if (string.IsNullOrEmpty(mySqlConnectionString))
            {
                IConfiguration configuration
                    = sp.GetRequiredService<IConfiguration>();
                mySqlConnectionString
                    = configuration.GetConnectionString("MySqlConn");
            }

            if (string.IsNullOrEmpty(mySqlConnectionString))
            {
                throw new Exception(
                    "Database connection string is not configured. Please set ConnectionStrings.MySqlConn in appsettings.json");
            }

            logger.LogDebug("Connecting to {mySqlConnectionString}",
                mySqlConnectionString);
            options.UseMySql(mySqlConnectionString,
                ServerVersion.AutoDetect(mySqlConnectionString));
        });

        return builder;
    }

    public static WebApplicationBuilder AddLoggingConfiguration(
        this WebApplicationBuilder builder)
    {
        // Nlog
        builder.Configuration.AddJsonFile(@"Configuration/nlog.json", false, true);
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        GlobalDiagnosticsContext.Set("ApplicationName", GetApplicationName());
        GlobalDiagnosticsContext.Set("ConfigurationName",
            GetConfigurationName());

        // Sentry logs
        builder.WebHost.UseSentry(options =>
        {
            IConfigurationSection sentrySection = builder.Configuration.GetSection("Sentry");
            string? sentryDsn =
                sentrySection.GetValue<string>("Dsn") ??
                Environment.GetEnvironmentVariable("SentryDsn");

            if (string.IsNullOrEmpty(sentryDsn))
            {
                Console.WriteLine(
                    "Sentry DSN not set. Errors will not be reported.");
                options.Dsn = "";
                return;
            }

            options.Dsn = sentryDsn;
            options.MinimumBreadcrumbLevel = LogLevel.Information;
            options.MinimumEventLevel = LogLevel.Error;
            options.EnableLogs = sentrySection.GetValue("EnableLogs", true);
        });

        return builder;
    }

    public static WebApplicationBuilder AddApiConfiguration(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        string port = Environment.GetEnvironmentVariable("BACKEND_PORT") ??
                      "5109";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        return builder;
    }

    private static string? GetConfigurationName()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        Regex regex = new(@$"{GetApplicationName()}\\(.*)\\v");
        Match match = regex.Match(baseDirectory);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }

    private static string GetApplicationName()
    {
        Assembly? entryAssembly = Assembly.GetEntryAssembly();
        return entryAssembly?.GetName().Name!;
    }
}