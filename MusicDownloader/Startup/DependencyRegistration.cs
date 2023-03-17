using Serilog;
using YoutubeReExplode;

namespace MusicDownloader.Startup;

public static class DependencyInjection
{
    public static void AddBusiness(this IServiceCollection serviceCollection)
    {
        // Add business services here
    }

    public static void AddLibraries(this IServiceCollection serviceCollection)
    {
        // Add serilog
        serviceCollection.AddSingleton(Log.Logger);

        // Add YoutubeReExplode
        AddYoutubeReExplode(serviceCollection);

        // Automapper
        serviceCollection.AddAutoMapper(typeof(Program));

        // MediatR
        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
    }

    private static void AddYoutubeReExplode(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<YoutubeClient>();
    }
}