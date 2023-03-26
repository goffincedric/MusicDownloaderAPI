using System.Text.Json;
using AutoMapper;
using MusicDownloader.Api.Models;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMapper _mapper;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next, ILogger logger, IHostEnvironment hostEnvironment, IMapper mapper
    )
    {
        _logger = logger;
        _next = next;
        _hostEnvironment = hostEnvironment;
        _mapper = mapper;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.Error(exception.ToString());
        // Map error details
        var mappedErrorDetails = _mapper.Map<ErrorDetails>(exception);
        mappedErrorDetails.ExtraInfo =
            _hostEnvironment.IsDevelopment() ? new[] { exception.StackTrace } : Array.Empty<string>();

        // Set response details and send
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = mappedErrorDetails.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(mappedErrorDetails));
    }
}

public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}