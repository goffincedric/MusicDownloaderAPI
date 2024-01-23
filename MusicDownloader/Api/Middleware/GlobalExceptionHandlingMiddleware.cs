using System.Text.Json;
using AutoMapper;
using MusicDownloader.Api.Models;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Middleware;

public class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger logger,
    IHostEnvironment hostEnvironment,
    IMapper mapper
)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.Error(exception.ToString());
        // Map error details
        var mappedErrorDetails = mapper.Map<ErrorDetails>(exception);
        mappedErrorDetails.ExtraInfo = hostEnvironment.IsDevelopment()
            ? [exception.StackTrace]
            : Array.Empty<string>();

        // Set response details and send
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = mappedErrorDetails.StatusCode;
        await context
            .Response
            .WriteAsync(
                JsonSerializer.Serialize(
                    mappedErrorDetails,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
    }
}

public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandlingMiddleware(
        this IApplicationBuilder app
    )
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
