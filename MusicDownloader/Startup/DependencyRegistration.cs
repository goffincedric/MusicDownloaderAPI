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
        // Add YoutubeExplode
        AddYoutubeExplode(serviceCollection);
    }

    private static void AddYoutubeExplode(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<YoutubeClient>();
    }
}