using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.Transcoding._base;

public interface ITranscoderStrategy
{
    /// <summary>
    /// Executes transcoding processes for supported a supported container.
    /// </summary>
    /// <param name="audioStreamTask">Task that contains a stream of the audio to transcode</param>
    /// <param name="coverArtStreamTask">Task that contains an optional stream of the cover art to include</param>
    /// <param name="trackMetadataTask">Task that contains an object with metadata about the audio, which will be included in the transcoded audio</param>
    /// <returns>An object containing the transcoded audio stream, along with the filename and container</returns>
    internal Task<MusicStream> Execute(
        Task<Stream> audioStreamTask,
        Task<Stream?> coverArtStreamTask,
        Task<TrackMetadata> trackMetadataTask
    );
}