using DotNetEnv;
using MusicDownloader.Shared.Constants;
using Serilog;

namespace MusicDownloader.Startup;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder ConfigureWebApplicationBuilder(
        this WebApplicationBuilder builder
    )
    {
        // Load .env file when developing locally
        if (builder.Environment.IsDevelopment())
            Env.Load();
        // Load settings
        builder.Services.AddSettings();

        // Add serilog functionality
        builder.Host.UseSerilog();

        // Cors domains
        builder.Services.AddCors(
            options =>
                options.AddPolicy(
                    ApplicationConstants.Cors.AllowAllOrigins,
                    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                )
        );

        // Add services to the container.
        builder.Services.AddHttpClient();
        builder.Services.AddLibraries();
        builder.Services.AddBusiness();
        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.ConfigureSwagger();
        // Add authentication
        builder.Services.ConfigureAuthentication();

        return builder;
    }
}
