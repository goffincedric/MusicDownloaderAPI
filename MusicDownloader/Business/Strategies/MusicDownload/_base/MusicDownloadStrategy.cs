using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Music.Metadata;
using MusicDownloader.Business.Requests.Music.Transcoding;
using MusicDownloader.Business.Requests.Youtube.Download;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.MusicDownload._base;

public abstract class MusicDownloadStrategy : IMusicDownloadStrategy
{
    protected readonly IMediator Mediator;

    protected MusicDownloadStrategy(IMediator mediator)
    {
        Mediator = mediator;
    }

    /// <summary>
    /// Executes the general flow that is followed when downloading music from a supported provider.
    /// </summary>
    /// <param name="url">Url linking to the music to download</param>
    /// <param name="metadataMapperStrategy">Desired tag system</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A wrapper containing streamed music, along with some file metadata</returns>
    public async Task<MusicStream> Execute(
        string url,
        // TODO: Change to desired output container and then chose mapper strategy depending on chosen container
        IMetadataMapperStrategy metadataMapperStrategy,
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
        var audioStreamTask = GetAudioStream(url, cancellationToken);
        var coverArtStreamTask = GetCoverArtStream(
            trackDetails.Thumbnails, playlistDetailsExtended?.Thumbnails, cancellationToken
        );
        var metadataTask = GetMetadata(trackDetails, playlistDetailsExtended, cancellationToken);

        // Transcode audio assets
        return await Transcode(
            audioStreamTask, coverArtStreamTask, metadataTask, metadataMapperStrategy, cancellationToken
        );
    }

    #region Overridable methods for strategies

    protected abstract void Validate(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    );

    protected abstract Task<Stream> GetAudioStream(string url, CancellationToken cancellationToken = default);

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

    private Task<MusicStream> Transcode(
        Task<Stream> audioStreamTask,
        Task<Stream?> coverArtStreamTask,
        Task<TrackMetadata> trackMetadataTask,
        IMetadataMapperStrategy metadataMapperStrategy,
        CancellationToken cancellationToken = default
    ) => Mediator.Send(new TranscodeAudioRequest
    {
        AudioStreamTask = audioStreamTask,
        CoverArtTask = coverArtStreamTask,
        TrackMetadataTask = trackMetadataTask,
        MetadataMapperStrategy = metadataMapperStrategy
    }, cancellationToken);
}