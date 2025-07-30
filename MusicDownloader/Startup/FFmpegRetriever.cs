using FFMpegCore;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Startup;

public static class FFMpegConfigurator
{
    public static void ConfigureFFMpeg()
    {
        // Configure FFMpegCore
        GlobalFFOptions.Configure(options =>
            options.BinaryFolder = ApplicationConstants.FFMpegPath
        );
    }
}
