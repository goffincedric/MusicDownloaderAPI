﻿using FFMpegCore.Enums;

namespace MusicDownloader.Shared.Constants;

public static class YoutubeConstants
{
    public static readonly string Container = "ogg";
    public static readonly Codec AudioCodec = FFMpegCore.Enums.AudioCodec.LibVorbis;
    public static readonly AudioQuality AudioQuality = AudioQuality.VeryHigh;
    public static readonly int SamplingRate = 48000;
    public static readonly Codec VideoCodec = FFMpegCore.Enums.VideoCodec.LibTheora; // Codec needed for cover art video
}