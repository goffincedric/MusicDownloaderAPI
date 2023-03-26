using MusicDownloader.Api.Middleware;

namespace MusicDownloader.Startup;

public static class MiddlewareRegistration
{
    public static void RegisterCustomMiddlewares(this IApplicationBuilder app)
    {
        // Add global error handling
        app.UseGlobalExceptionHandlingMiddleware();
    }
}