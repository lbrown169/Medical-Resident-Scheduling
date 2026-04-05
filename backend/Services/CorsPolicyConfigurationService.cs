using Microsoft.AspNetCore.Cors.Infrastructure;

namespace MedicalDemo.Services;

public static class CorsPolicyConfigurationService
{
    private const string DEVELOPMENT_CORS_POLICY_NAME = "AllowFrontendDevelopment";
    private const string STAGING_CORS_POLICY_NAME = "AllowFrontendStaging";
    private const string PRODUCTION_CORS_POLICY_NAME = "AllowFrontendProduction";

    private static readonly string[] DevelopmentOrigins = ["http://localhost"];
    private static readonly string[] StagingOrigins = ["https://staging.psycall.net", "https://backend.staging.psycall.net"];
    private static readonly string[] ProductionOrigins = ["https://psycall.net", "https://backend.psycall.net"];

    public static void AddAllCorsPolicy(CorsOptions options)
    {
        AddDevelopmentCorsPolicy(options);
        AddStagingCorsPolicy(options);
        AddProductionCorsPolicy(options);
    }

    public static void AddDevelopmentCorsPolicy(CorsOptions options)
    {
        AddCorsPolicy(options, DEVELOPMENT_CORS_POLICY_NAME, DevelopmentOrigins);
    }

    public static void AddStagingCorsPolicy(CorsOptions options)
    {
        AddCorsPolicy(options, STAGING_CORS_POLICY_NAME, StagingOrigins);
    }

    public static void AddProductionCorsPolicy(CorsOptions options)
    {
        AddCorsPolicy(options, PRODUCTION_CORS_POLICY_NAME, ProductionOrigins);
    }

    public static void ApplyCorsPolicy(WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.UseCors(PRODUCTION_CORS_POLICY_NAME);
        }
        else if (app.Environment.IsStaging())
        {
            app.UseCors(STAGING_CORS_POLICY_NAME);
        }
        else
        {
            app.UseCors(DEVELOPMENT_CORS_POLICY_NAME);
        }
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