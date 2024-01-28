using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Music.Metadata;
using MusicDownloader.Business.Requests.Youtube.Download;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Extensions;
using MusicDownloader.Shared.Utils;

namespace MusicDownloader.Business.Strategies.MusicDownload._base;

public abstract class MusicDownloadStrategy(IMediator mediator) : IMusicDownloadStrategy
{
    public async Task<MusicStream> Execute(
        string url,
        ITranscoderStrategy? transcoderStrategy = null,
        CancellationToken cancellationToken = default
    )
    {
        // Get music details and validate them
        var (trackDetails, playlistDetailsExtended) = await mediator.Send(
            new GetDownloadRequestDetailsRequest(url),
            cancellationToken
        );
        Validate(trackDetails, playlistDetailsExtended, cancellationToken);

        // Get the required assets for transcoding
        var trackMetadata = await GetMetadata(
            trackDetails,
            playlistDetailsExtended,
            cancellationToken
        );
        if (transcoderStrategy?.RequiresTrackMetadata() == true)
            transcoderStrategy.SetTrackMetadata(trackMetadata);
        if (transcoderStrategy?.RequiresCoverArtStream() == true)
        {
            var coverArtStreamTask = GetCoverArtStream(
                trackDetails.Thumbnails,
                playlistDetailsExtended?.Thumbnails,
                cancellationToken
            );
            transcoderStrategy.SetCoverArtStream(coverArtStreamTask);
        }

        // Get audio assets and transcode if necessary
        DownloadStreamInfo downloadStreamInfo;
        if (transcoderStrategy is null)
        {
            downloadStreamInfo = await GetAudioStream(url, cancellationToken);
        }
        else
        {
            var audioUrl = await GetAudioUrl(url, cancellationToken);
            downloadStreamInfo = await transcoderStrategy.Execute(audioUrl, cancellationToken);
        }

        // Return stream along with container info
        return new MusicStream(
            downloadStreamInfo.Stream,
            GetFileName(trackMetadata, downloadStreamInfo.Container),
            GetMimeType(downloadStreamInfo.Container)
        );
    }

    #region Overridable methods for strategies

    protected abstract void Validate(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    );

    protected abstract Task<DownloadStreamInfo> GetAudioStream(
        string url,
        CancellationToken cancellationToken = default
    );

    protected abstract Task<string> GetAudioUrl(
        string url,
        CancellationToken cancellationToken = default
    );

    #endregion

    #region Private utility methods

    private Task<TrackMetadata> GetMetadata(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    ) =>
        mediator.Send(
            new ResolveMusicMetadataRequest(trackDetails, playlistDetailsExtended),
            cancellationToken
        );

    private Task<Stream?> GetCoverArtStream(
        List<ThumbnailDetails> trackThumbnails,
        List<ThumbnailDetails>? playlistThumbnails = null,
        CancellationToken cancellationToken = default
    ) =>
        mediator.Send(
            new ResolveMusicCoverImageRequest(trackThumbnails, playlistThumbnails),
            cancellationToken
        );

    private static string GetFileName(TrackMetadata trackMetadata, string fileExtension) =>
        $"{trackMetadata.Title.ToSafeFilename()}.{fileExtension}";

    private static string GetMimeType(string fileExtension) =>
        MimeTypeUtils.MapExtensionToMimeType(fileExtension);

    #endregion
}
