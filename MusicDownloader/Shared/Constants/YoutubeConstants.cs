using FFMpegCore.Enums;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Shared.Constants;

public abstract class YoutubeConstants
{
    public static readonly TimeSpan MaxAllowedDownloadDuration = new(1, 0, 0);
    public static readonly int MinRequiredCoverResolution = 600 * 600;

    public const AudioQuality AudioQuality = FFMpegCore.Enums.AudioQuality.VeryHigh;
    public const int SamplingRate = 48000;
    public const int CoverFramerate = 1;

    public static readonly Container[] SupportedContainers =
    {
        new(ContainerConstants.Containers.Ogg),
        new(ContainerConstants.Containers.Mp3),
        new(ContainerConstants.Containers.Opus),
        new(ContainerConstants.Containers.Aac)
    };
}