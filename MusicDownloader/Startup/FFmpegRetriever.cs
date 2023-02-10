using FFMpegCore;
using MusicDownloader.Shared.Constants;
using Xabe.FFmpeg.Downloader;

namespace MusicDownloader.Startup;

public static class FFmpegConfigurator
{
    public static async Task RetrieveFFmpegBinariesAsync()
    {
        Console.WriteLine("Downloading FFmpeg binaries...");
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ApplicationConstants.FFmpegPath);
        Console.WriteLine("Downloaded FFmpeg binaries.");
    }

    public static async Task ConfigureFFmpeg()
    {
        await RetrieveFFmpegBinariesAsync();
        
        // Configure FFmpegCore
        GlobalFFOptions.Configure(options => options.BinaryFolder = ApplicationConstants.FFmpegPath);
    }
}