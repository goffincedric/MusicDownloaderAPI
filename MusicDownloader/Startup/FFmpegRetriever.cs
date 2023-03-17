using System.Diagnostics;
using System.Runtime.InteropServices;
using FFMpegCore;
using MusicDownloader.Shared.Constants;
using Serilog;
using Xabe.FFmpeg.Downloader;

namespace MusicDownloader.Startup;

public static class FFMpegConfigurator
{
    private static async Task RetrieveFFMpegBinariesAsync()
    {
        Log.Logger.Information("Downloading FFMpeg binaries...");
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ApplicationConstants.FFMpegPath);
        Log.Logger.Information("Downloaded FFMpeg binaries.");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Log.Logger.Information("Detected Linux OS, making downloaded binary executable...");
            var success = Chmod(ApplicationConstants.FFMpegPath, "777", true);
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

    public static async Task ConfigureFFMpeg()
    {
        await RetrieveFFMpegBinariesAsync();
        
        // Configure FFMpegCore
        GlobalFFOptions.Configure(options => options.BinaryFolder = ApplicationConstants.FFMpegPath);
    }
}