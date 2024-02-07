using System.Net;
using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Business.Strategies.MusicStream._base;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Strategies.MusicStream;

public class YoutubeStreamStrategy : MusicStreamStrategy
{
    public YoutubeStreamStrategy(IMediator mediator)
        : base(mediator) { }

    private IStreamInfo SelectedStreamInfo { get; set; } = null!;

    protected override void Validate(
        TrackDetails trackDetails,
        PlaylistDetailsExtended? playlistDetailsExtended,
        CancellationToken cancellationToken = default
    )
    {
        // Check if video is livestream
        if (trackDetails.IsLive == true)
            throw new MusicDownloaderException(
                "Livestreams cannot be downloaded",
                ErrorCodes.Youtube.LivestreamDownloadNotAllowed,
                HttpStatusCode.BadRequest
            );
        // Check if video doesn't go over allowed maximum duration
        if (trackDetails.Duration > YoutubeConstants.MaxAllowedDownloadDuration)
            throw new MusicDownloaderException(
                $"Videos longer than {YoutubeConstants.MaxAllowedDownloadDuration.ToString()} cannot be downloaded",
                ErrorCodes.Youtube.LongVideoDownloadNotAllowed,
                HttpStatusCode.BadRequest
            );
    }

    protected override async Task<SelectedMediaSource> SelectAudioSource(
        string url,
        CancellationToken cancellationToken
    )
    {
        // Set the stream info
        SelectedStreamInfo = await GetStreamInfo(url, cancellationToken);
        return new SelectedMediaSource { Container = SelectedStreamInfo.Container.Name };
    }

    protected override async Task Transcode(
        ITranscoderStrategy? transcoderStrategy,
        Stream targetStream,
        CancellationToken cancellationToken
    )
    {
        // Check if media source was selected
        if (SelectedStreamInfo is null)
            throw new MusicDownloaderException(
                "No media source was selected yet",
                ErrorCodes.Youtube.NoMediaSourceSelected,
                HttpStatusCode.InternalServerError
            );

        if (transcoderStrategy is null)
        {
            // Get stream info and set response headers
            var stream = await GetAudioStream(SelectedStreamInfo, cancellationToken);
            // Stream audio straight to response
            await stream.CopyToAsync(targetStream, cancellationToken);
        }
        // TODO: Check if transcoder is a StreamSourceTranscoder or UrlSourceTranscoder
        else
        {
            // Transcode and stream to response
            await transcoderStrategy.Execute(
                SelectedStreamInfo.Url,
                targetStream,
                cancellationToken
            );
        }
    }

    #region Youtube specific helper methods

    private async Task<IStreamInfo> GetStreamInfo(
        string url,
        CancellationToken cancellationToken
    ) => await Mediator.Send(new GetStreamInfoRequest(url), cancellationToken);

    private Task<Stream> GetAudioStream(
        IStreamInfo streamInfo,
        CancellationToken cancellationToken = default
    ) => Mediator.Send(new GetAudioStreamRequest(streamInfo), cancellationToken);

    #endregion
}
