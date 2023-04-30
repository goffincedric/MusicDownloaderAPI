using System.Net;
using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Business.Strategies.MusicDownload._base;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;

namespace MusicDownloader.Business.Strategies.MusicDownload;

public class YoutubeDownloadStrategy : MusicDownloadStrategy
{
    public YoutubeDownloadStrategy(IMediator mediator) : base(mediator)
    {
    }

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
                $@"Videos longer than {YoutubeConstants.MaxAllowedDownloadDuration.ToString()} cannot be downloaded",
                ErrorCodes.Youtube.LongVideoDownloadNotAllowed,
                HttpStatusCode.BadRequest
            );
    }

    protected override Task<Stream> GetAudioStream(string url, CancellationToken cancellationToken = default) =>
        Mediator.Send(new GetAudioStreamRequest { Url = url }, cancellationToken);

    protected override Task<string> GetAudioUrl(string url, CancellationToken cancellationToken = default) =>
        Mediator.Send(new GetAudioUrlRequest { Url = url }, cancellationToken);
}