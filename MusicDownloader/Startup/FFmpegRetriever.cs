using System.Diagnostics;
using System.Runtime.InteropServices;
using FFMpegCore;
using MusicDownloader.Shared.Constants;
using Xabe.FFmpeg.Downloader;

namespace MusicDownloader.Startup;

public static class FFmpegConfigurator
{
    private static async Task RetrieveFFmpegBinariesAsync()
    {
        Console.WriteLine("Downloading FFmpeg binaries...");
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ApplicationConstants.FFmpegPath);
        Console.WriteLine("Downloaded FFmpeg binaries.");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Console.WriteLine("Detected Linux OS, making downloaded binary executable...");
            var success = Chmod(ApplicationConstants.FFmpegPath, "777", true);
            if (!success) Console.WriteLine("WARNING !!!Couldn't make binaries executable!!!");
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