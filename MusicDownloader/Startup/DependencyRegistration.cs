using MediatR;
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
        // Add YoutubeReExplode
        AddYoutubeReExplode(serviceCollection);
        
        // Automapper
        serviceCollection.AddAutoMapper(typeof(Program));
        
        // MediatR
        serviceCollection.AddMediatR(typeof(Program));
    }

    private static void AddYoutubeReExplode(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<YoutubeClient>();
    }
}