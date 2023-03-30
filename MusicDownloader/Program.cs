using MusicDownloader.Startup;
using Serilog;

// Create basic logging for application startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    // Retrieve FFMpeg binaries for local use
    await FFMpegConfigurator.ConfigureFFMpeg();

    // Create web builder
    var builder = WebApplication
        .CreateBuilder(args)
        .ConfigureWebApplicationBuilder();

    // Build, configure and run web application
    builder
        .Build()
        .ConfigureWebApplication()
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

/*
 * TODO:
 *  - Filter out 'artist name -' and '- artist name' from song title + trim song title
 *  - Add option to choose output containers: mp3, opus, aac
 *  - DOTNET 8: Add TimeProvider.System as singleton to DI Container and use it everywhere instead of DateTime class
 */