﻿using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.Transcoding._base;

public interface ITranscoderStrategy
{
    /// <summary>
    /// Executes transcoding processes for supported a supported container.
    /// Codec performance: libopus > libvorbis >= libfdk_aac > libmp3lame >= eac3/ac3 > aac > libtwolame > vorbis > mp2 > wmav2/wmav1
    /// Source: https://trac.ffmpeg.org/wiki/Encode/HighQualityAudio#AudioencodersFFmpegcanuse
    /// </summary>
    /// <param name="audioUrl">String that contains a url to the audio to transcode</param>
    /// <param name="coverArtStreamTask">Task that contains an optional stream of the cover art to include</param>
    /// <param name="trackMetadataTask">Task that contains an object with metadata about the audio, which will be included in the transcoded audio</param>
    /// <returns>An object containing the transcoded audio stream, along with the filename and container</returns>
    internal Task<MusicStream> Execute(
        string audioUrl,
        Task<Stream?> coverArtStreamTask,
        Task<TrackMetadata> trackMetadataTask
    );
}