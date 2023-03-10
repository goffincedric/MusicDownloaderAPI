using MusicDownloader.Business.Requests.Youtube.Metadata;
using MusicDownloader.Startup;
using Serilog;

// Create basic logging for application startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    // Retrieve FFmpeg binaries for local use
    await FFmpegConfigurator.ConfigureFFmpeg();

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
    builder.Services.AddHttpClient<ResolveVideoCoverImageRequestHandler>();
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
 *  - Logging
 *  - Error handling
 *  - API Key auth
 *  - Filter out 'artist name -' and '- artist name' from song title + trim song title
 *  - add restriction to max 15 mins of video download + no livestreams
 */
// Linux: sudo apt-get install -y ffmpeg libgdiplus