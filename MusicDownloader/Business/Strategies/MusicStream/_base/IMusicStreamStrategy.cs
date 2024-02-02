using MusicDownloader.Business.Strategies.Transcoding._base;

namespace MusicDownloader.Business.Strategies.MusicStream._base;

public interface IMusicStreamStrategy
{
    /// <summary>
    /// Executes the general flow that is followed when downloading music from a supported provider and streaming it to the client.
    /// </summary>
    /// <param name="url">Url linking to the music to download</param>
    /// <param name="transcoderStrategy">Optional desired transcoding strategy</param>
    /// <param name="response">Response to the stream the media to</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    Task Execute(
        string url,
        ITranscoderStrategy? transcoderStrategy,
        HttpResponse response,
        CancellationToken cancellationToken = default
    );
}
