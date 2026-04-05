using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MedicalDemo.Algorithms.OnCallScheduleGenerator;
using MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;
using MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator;
using MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;
using MedicalDemo.Converters;
using MedicalDemo.Interfaces;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using MedicalDemo.Services.EmailSendServices;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MedicalDemo.Extensions;

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
        builder.Services.AddScoped<RuleViolationService>();
        builder.Services.AddScoped<Pgy4RotationScheduleService>();
        builder.Services.AddTransient<Pgy4RotationScheduleGenerator>();

        // Converters
        builder.Services.AddScoped<AdminConverter>();
        builder.Services.AddScoped<AnnouncementConverter>();
        builder.Services.AddScoped<BlackoutConverter>();
        builder.Services.AddScoped<DateConverter>();
        builder.Services.AddScoped<ResidentConverter>();
        builder.Services.AddScoped<ScheduleConverter>();
        builder.Services.AddScoped<SwapRequestConverter>();
        builder.Services.AddScoped<VacationConverter>();
        builder.Services.AddScoped<Pgy4RotationScheduleConverter>();
        builder.Services.AddScoped<RotationPrefRequestConverter>();
        builder.Services.AddScoped<RotationTypeConverter>();
        builder.Services.AddScoped<RotationConverter>();
        builder.Services.AddScoped<RotationPrefSubmissionWindowConverter>();
        builder.Services.AddScoped<Pgy4RotationScheduleOverrideConverter>();

        // Pgy4 schedule constraints
        builder.Services.AddScoped<HasChiefRotationConstraint>();
        builder.Services.AddScoped<InpatientConsultInJulyAndJanConstraint>();
        builder.Services.AddScoped<Min2ConsultsInpatientConstraint>();
        builder.Services.AddScoped<OneIopForenCommAddictPerMonthConstraint>();

        // Constraints
        builder.Services.AddScoped<ICallShiftConstraint, NoConsecutiveShiftConstraint>();
        builder.Services.AddScoped<ICallShiftConstraint, NotOnVacationConstraint>();
        builder.Services.AddScoped<ICallShiftConstraint, OneShiftADayConstraint>();
        builder.Services.AddScoped<ICallShiftConstraint, ShiftMatchesPgyDateConstraint>();
        builder.Services.AddScoped<ICallShiftConstraint, ShiftMatchesRotationConstraint>();

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
                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(domain))
                {
                    apiKey = Environment
                        .GetEnvironmentVariable("MailgunApiKey");
                    domain = Environment
                        .GetEnvironmentVariable("MailgunDomain");

                    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(domain))
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

        bool hasLoggedConnectionString = false;
        builder.Services.AddDbContextFactory<MedicalContext>((sp, options) =>
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

            if (!hasLoggedConnectionString)
            {
                logger.LogInformation("Connecting to database");
            }

            options.UseMySql(
                mySqlConnectionString,
                new MySqlServerVersion(new Version(8, 0, 36)),
                mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null
                    );
                }
            );
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
                Environment.GetEnvironmentVariable("SENTRY_DSN");

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
        builder.Services.AddCors(CorsPolicyConfigurationService.AddAllCorsPolicy);

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