using MusicDownloader.Shared.Constants;
using Serilog;

namespace MusicDownloader.Startup;

public static class WebApplicationExtension
{
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        #region Logging

        // Add request logging using serilog
        app.UseSerilogRequestLogging();

        #endregion

        #region Swagger

        // Only use swagger in development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(setup => setup.EnableTryItOutByDefault());
        }

        #endregion

        #region Middleware

        // Use cors policy
        app.UseCors(ApplicationConstants.Cors.AllowAllOrigins);

        // Redirect http to https
        app.UseHttpsRedirection();

        // Enable authentication & authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        // Register custom middlewares
        app.RegisterCustomMiddlewares();

        #endregion

        // Map controller endpoints
        app.MapControllers();

        return app;
    }
}
