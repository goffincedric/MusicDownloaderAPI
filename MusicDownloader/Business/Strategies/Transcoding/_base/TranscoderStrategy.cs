using MusicDownloader.Business.Models;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.Transcoding._base;

public abstract class TranscoderStrategy : ITranscoderStrategy
{
    private readonly bool _requiresTrackMetadata;

    private readonly bool _requiresCoverArtStream;

    protected TrackMetadata TrackMetadata;
    protected Task<Stream?> CoverArtStreamTask;

    protected TranscoderStrategy(bool requiresTrackMetadata, bool requiresCoverArtStream)
    {
        _requiresTrackMetadata = requiresTrackMetadata;
        _requiresCoverArtStream = requiresCoverArtStream;
    }

    public bool RequiresTrackMetadata() => _requiresTrackMetadata;

    public bool RequiresCoverArtStream() => _requiresCoverArtStream;

    /// <param name="trackMetadata">An object with metadata about the audio, which will be included in the transcoded audio</param>
    public void SetTrackMetadata(TrackMetadata trackMetadata) => TrackMetadata = trackMetadata;

    /// <param name="coverArtStreamTask">Task that contains an optional stream of the cover art to include</param>
    public void SetCoverArtStream(Task<Stream?> coverArtStreamTask) =>
        CoverArtStreamTask = coverArtStreamTask;

    // TODO: Remove?

    public abstract Task<DownloadStreamInfo> Execute(
        string audioUrl,
        CancellationToken cancellationToken
    );
}
