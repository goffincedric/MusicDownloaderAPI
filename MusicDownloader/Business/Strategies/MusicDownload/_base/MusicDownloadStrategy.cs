using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Music.Metadata;
using MusicDownloader.Business.Requests.Youtube.Download;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.MusicDownload._base;

public abstract class MusicDownloadStrategy : IMusicDownloadStrategy
{
    protected readonly IMediator Mediator;

    protected MusicDownloadStrategy(IMediator mediator)
    {
        Mediator = mediator;
    }
    
    public async Task<MusicStream> Execute(
        string url,
        ITranscoderStrategy transcoderStrategy,
        CancellationToken cancellationToken = default
    )
    {
        // Get music details and validate them
        var downloadDetails = await Mediator.Send(new GetDownloadRequestDetailsRequest
        {
            Url = url
        }, cancellationToken);
        var trackDetails = downloadDetails.TrackDetails;
        var playlistDetailsExtended = downloadDetails.PlaylistDetailsExtended;
        Validate(trackDetails, playlistDetailsExtended, cancellationToken);

        // Get the required assets for transcoding
        var audioUrl = await GetAudioUrl(url, cancellationToken);
        var coverArtStreamTask = GetCoverArtStream(
            trackDetails.Thumbnails, playlistDetailsExtended?.Thumbnails, cancellationToken
        );
        var metadataTask = GetMetadata(trackDetails, playlistDetailsExtended, cancellationToken);

        // Transcode audio assets
        return await transcoderStrategy.Execute(audioUrl, coverArtStreamTask, metadataTask);
    }

    #region Overridable methods for strategies

    protected abstract void Validate(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    );

    protected abstract Task<Stream> GetAudioStream(string url, CancellationToken cancellationToken = default);
    protected abstract Task<string> GetAudioUrl(string url, CancellationToken cancellationToken = default);

    #endregion

    private Task<TrackMetadata> GetMetadata(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    ) => Mediator.Send(new ResolveMusicMetadataRequest
    {
        TrackDetails = trackDetails,
        PlaylistDetails = playlistDetailsExtended
    }, cancellationToken);

    private Task<Stream?> GetCoverArtStream(
        List<ThumbnailDetails> trackThumbnails,
        List<ThumbnailDetails>? playlistThumbnails = null,
        CancellationToken cancellationToken = default
    ) => Mediator.Send(new ResolveMusicCoverImageRequest
    {
        TrackThumbnails = trackThumbnails,
        PlaylistThumbnails = playlistThumbnails
    }, cancellationToken);
}