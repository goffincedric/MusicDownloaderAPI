using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.Transcoding._base;

public abstract class TranscoderStrategy : ITranscoderStrategy
{
    private readonly bool _requiresTrackMetadata;

    private readonly bool _requiresCoverArtStream;

    protected readonly string TargetContainer;

    protected TrackMetadata TrackMetadata;
    protected Task<Stream?> CoverArtStreamTask;

    protected TranscoderStrategy(
        bool requiresTrackMetadata,
        bool requiresCoverArtStream,
        string targetContainer
    )
    {
        _requiresTrackMetadata = requiresTrackMetadata;
        _requiresCoverArtStream = requiresCoverArtStream;
        TargetContainer = targetContainer;
    }

    public bool RequiresTrackMetadata() => _requiresTrackMetadata;

    public bool RequiresCoverArtStream() => _requiresCoverArtStream;

    public string GetTargetContainer() => TargetContainer;

    public Task Execute(string audioUrl, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <param name="trackMetadata">An object with metadata about the audio, which will be included in the transcoded audio</param>
    public void SetTrackMetadata(TrackMetadata trackMetadata) => TrackMetadata = trackMetadata;

    /// <param name="coverArtStreamTask">Task that contains an optional stream of the cover art to include</param>
    public void SetCoverArtStream(Task<Stream?> coverArtStreamTask) =>
        CoverArtStreamTask = coverArtStreamTask;

    #region Overridable methods

    public abstract Task Execute(
        string audioUrl,
        Stream targetStream,
        CancellationToken cancellationToken
    );

    #endregion
}
