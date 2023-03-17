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
    var builder = WebApplication.CreateBuilder(args);

    // Add serilog functionality
    builder.Host.UseSerilog();

    // Cors domains
    const string allowAllOrigins = "AllowAllOrigins";
    builder.Services.AddCors(options => options.AddPolicy(allowAllOrigins, policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    ));

    // Add services to the container.
    builder.Services.AddHttpClient();
    builder.Services.AddLibraries();
    builder.Services.AddBusiness();

    builder.Services.AddRouting(options => { options.LowercaseUrls = true; });
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Build web application
    var app = builder.Build();

    // Add request logging using serilog
    app.UseSerilogRequestLogging();

    // Only use swagger in development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Use cors policy
    app.UseCors(allowAllOrigins);
    // Redirect http to https
    app.UseHttpsRedirection();
    // Enable authorization middleware
    app.UseAuthorization();

    // Register custom middlewares
    app.RegisterCustomMiddlewares();

    // Map controller endpoints
    app.MapControllers();

    // Run the applications
    app.Run();
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
 *  - Filter out livestreams from playlists
 *  - API Key auth
 *  - Filter out 'artist name -' and '- artist name' from song title + trim song title
 *  - Make docker container pull latest changes to stay up-to-date
 */
// Linux: sudo apt-get install -y ffmpeg libgdiplus