using MedicalDemo.Services;

namespace MedicalDemo.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void ConfigureControllers()
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.ApplyCorsPolicy();
            app.MapControllers();
        }

        public void ApplyCorsPolicy()
        {
            CorsPolicyConfigurationService.ApplyDefaultCorsPolicy(app);
        }
    }
}