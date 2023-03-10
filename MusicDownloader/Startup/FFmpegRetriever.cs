using System.Diagnostics;
using System.Runtime.InteropServices;
using FFMpegCore;
using MusicDownloader.Shared.Constants;
using Serilog;
using Xabe.FFmpeg.Downloader;

namespace MusicDownloader.Startup;

public static class FFmpegConfigurator
{
    private static async Task RetrieveFFmpegBinariesAsync()
    {
        Log.Logger.Information("Downloading FFmpeg binaries...");
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ApplicationConstants.FFmpegPath);
        Log.Logger.Information("Downloaded FFmpeg binaries.");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Log.Logger.Information("Detected Linux OS, making downloaded binary executable...");
            var success = Chmod(ApplicationConstants.FFmpegPath, "777", true);
            if (!success) Log.Logger.Warning("WARNING !!!Couldn't make binaries executable!!!");
        }
    }
    
    private static bool Chmod(string filePath, string permissions = "700", bool recursive = false)
    {
        var cmd = recursive ? $"chmod -R {permissions} {filePath}" : $"chmod {permissions} {filePath}";
        try
        {
            using var process = Process.Start("/bin/bash", $"-c \"{cmd}\"");
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public static async Task ConfigureFFmpeg()
    {
        await RetrieveFFmpegBinariesAsync();
        
        // Configure FFmpegCore
        GlobalFFOptions.Configure(options => options.BinaryFolder = ApplicationConstants.FFmpegPath);
    }
}