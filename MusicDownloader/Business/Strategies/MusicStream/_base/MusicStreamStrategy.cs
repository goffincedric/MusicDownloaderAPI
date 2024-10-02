using MediatR;
using Microsoft.Net.Http.Headers;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Music.Metadata;
using MusicDownloader.Business.Requests.Youtube.Download;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Extensions;
using MusicDownloader.Shared.Utils;

namespace MusicDownloader.Business.Strategies.MusicStream._base;

public abstract class MusicStreamStrategy : IMusicStreamStrategy
{
    protected readonly IMediator Mediator;

    public MusicStreamStrategy(IMediator mediator)
    {
        Mediator = mediator;
    }

    public async Task Execute(
        string url,
        ITranscoderStrategy? transcoderStrategy,
        HttpResponse response,
        CancellationToken cancellationToken = default
    )
    {
        // Get music details and validate them
        var (trackDetails, playlistDetailsExtended) = await Mediator.Send(
            new GetDownloadRequestDetailsRequest(url),
            cancellationToken
        );
        Validate(trackDetails, playlistDetailsExtended, cancellationToken);

        // Get track metadata
        var trackMetadata = await GetMetadata(
            trackDetails,
            playlistDetailsExtended,
            cancellationToken
        );

        // Select audio source to transcode from and set response headers
        var selectedMediaSource = await SelectAudioSource(url, cancellationToken);
        var targetContainer =
            transcoderStrategy?.GetTargetContainer() ?? selectedMediaSource.Container;
        SetResponseHeaders(response, trackMetadata, targetContainer);

        // Configure the transcoder strategy and start transcoding
        ConfigureTranscoderTargetMetadata(
            transcoderStrategy,
            trackMetadata,
            trackDetails,
            playlistDetailsExtended,
            cancellationToken
        );
        await Transcode(transcoderStrategy, response.Body, cancellationToken);
    }

    #region Overridable methods for strategies

    protected abstract void Validate(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sets the audio source for the strategy
    /// </summary>
    /// <returns>Info about the selected audio source</returns>
    protected abstract Task<SelectedMediaSource> SelectAudioSource(
        string url,
        CancellationToken cancellationToken
    );

    protected abstract Task Transcode(
        ITranscoderStrategy? transcoderStrategy,
        Stream targetStream,
        CancellationToken cancellationToken
    );

    #endregion

    #region Transcoder utility methods

    private void ConfigureTranscoderTargetMetadata(
        ITranscoderStrategy? transcoderStrategy,
        TrackMetadata trackMetadata,
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken
    )
    {
        // Don't configure transcoder if there is none
        if (transcoderStrategy is null)
            return;

        // Set track metadata
        if (transcoderStrategy.RequiresTrackMetadata())
            transcoderStrategy.SetTrackMetadata(trackMetadata);

        // Optionally, set cover art metadata if required by transcoder strategy
        if (!transcoderStrategy.RequiresCoverArtStream())
            return;
        var coverArtStreamTask = Mediator.Send(
            new ResolveMusicCoverImageRequest(
                trackDetails.Thumbnails,
                playlistDetailsExtended?.Thumbnails
            ),
            cancellationToken
        );
        transcoderStrategy.SetCoverArtStream(coverArtStreamTask);
    }

    #endregion

    #region Metadata utility methods

    private async Task<TrackMetadata> GetMetadata(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    ) =>
        await Mediator.Send(
            new ResolveMusicMetadataRequest(trackDetails, playlistDetailsExtended),
            cancellationToken
        );

    private static string GetFileName(TrackMetadata trackMetadata, string fileExtension) =>
        $"{trackMetadata.Title.ToSafeFilename()}.{fileExtension}";

    private static string GetMimeType(string fileExtension) =>
        MimeTypeUtils.MapExtensionToMimeType(fileExtension);

    #endregion

    #region Response utility headers

    private void SetResponseHeaders(
        HttpResponse response,
        TrackMetadata trackMetadata,
        string targetContainer
    )
    {
        var filename = GetFileName(trackMetadata, targetContainer);
        var mimeType = GetMimeType(targetContainer);

        // Set response headers to correctly reflect stream contents and return stream as file
        response.Headers.AccessControlExposeHeaders = "Content-Disposition";
        response.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = filename,
            FileNameStar = filename,
        }.ToString();
        response.ContentType = mimeType;
    }

    #endregion
}
