using MusicDownloader.Business.Models;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.Transcoding._base;

public interface ITranscoderStrategy
{
    internal bool RequiresTrackMetadata();
    internal bool RequiresCoverArtStream();
    
    internal void SetTrackMetadata(TrackMetadata trackMetadata);
    internal void SetCoverArtStream(Task<Stream?> coverArtStreamTask);
    
    
    /// <summary>
    /// Executes transcoding processes for supported a supported container.
    /// Codec performance: libopus > libvorbis >= libfdk_aac > libmp3lame >= eac3/ac3 > aac > libtwolame > vorbis > mp2 > wmav2/wmav1
    /// Source: https://trac.ffmpeg.org/wiki/Encode/HighQualityAudio#AudioencodersFFmpegcanuse
    /// </summary>
    /// <param name="audioUrl">String that contains a url to the audio to transcode</param>
    /// <returns>An object containing the transcoded audio stream along with the container name</returns>
    internal Task<DownloadStreamInfo> Execute(
        string audioUrl,
        CancellationToken cancellationToken
    );
}