using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.Transcoding._base;

public abstract class TranscoderStrategy : ITranscoderStrategy
{
    private readonly bool _requiresTrackMetadata;
    public bool RequiresTrackMetadata => _requiresTrackMetadata;
    
    private readonly bool _requiresCoverArtStream;
    public bool RequiresCoverArtStream => _requiresCoverArtStream;
    
    protected Task<TrackMetadata> TrackMetadataTask;
    protected Task<Stream?> CoverArtStreamTask;

    protected TranscoderStrategy(bool requiresTrackMetadata, bool requiresCoverArtStream)
    {
        _requiresTrackMetadata = requiresTrackMetadata;
        _requiresCoverArtStream = requiresCoverArtStream;
    }
    
    
    /// <param name="trackMetadataTask">Task that contains an object with metadata about the audio, which will be included in the transcoded audio</param>
    public void SetTrackMetadataStream(Task<TrackMetadata> trackMetadataTask) => TrackMetadataTask = trackMetadataTask;

    /// <param name="coverArtStreamTask">Task that contains an optional stream of the cover art to include</param>
    public void SetCoverArtStream(Task<Stream?> coverArtStreamTask) => CoverArtStreamTask = coverArtStreamTask;

    public abstract Task<MusicStream> Execute(string audioUrl, CancellationToken cancellationToken);
}