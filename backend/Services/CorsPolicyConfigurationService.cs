using Microsoft.AspNetCore.Cors.Infrastructure;

namespace MedicalDemo.Services;

public static class CorsPolicyConfigurationService
{
    private const string CORS_CONFIGURATION_SECTION_NAME = "Cors";
    private const string ALLOWED_ORIGINS_CONFIGURATION_SECTION_NAME = "AllowedOrigins";

    private const string DEFAULT_CORS_POLICY_NAME = "DefaultCorsPolicy";

    public static void AddDefaultCorsPolicy(CorsOptions options, IConfiguration configuration)
    {
        IConfigurationSection corsSection = configuration.GetSection(CORS_CONFIGURATION_SECTION_NAME);
        IConfiguration originSection = corsSection.GetSection(ALLOWED_ORIGINS_CONFIGURATION_SECTION_NAME);
        List<string> defaultOrigins = originSection.Get<List<string>>() ?? [];

        AddCorsPolicy(options, DEFAULT_CORS_POLICY_NAME, defaultOrigins);
    }

    public static void ApplyDefaultCorsPolicy(WebApplication app)
    {
        app.UseCors(DEFAULT_CORS_POLICY_NAME);
    }

    private static void AddCorsPolicy(CorsOptions options, string policyName, IEnumerable<string> origins)
    {
        options.AddPolicy(policyName, policy =>
        {
            policy
                .SetIsOriginAllowed(origin => origins.Any(o => origin.StartsWith(o, StringComparison.OrdinalIgnoreCase)))
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    }
}