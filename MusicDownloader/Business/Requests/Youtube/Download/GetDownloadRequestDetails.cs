using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Youtube.Playlist;
using MusicDownloader.Business.Requests.Youtube.Video;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Business.Requests.Youtube.Download;

public record GetDownloadRequestDetailsRequest(string Url) : IRequest<DownloadRequestDetails>;

public class GetDownloadRequestDetailsRequestHandler(IMediator mediator, ILogger logger)
    : IRequestHandler<GetDownloadRequestDetailsRequest, DownloadRequestDetails>
{
    public async Task<DownloadRequestDetails> Handle(
        GetDownloadRequestDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        // Get playlist details
        PlaylistDetailsExtended? playlistDetailsExtended = null;
        try
        {
            playlistDetailsExtended = await mediator.Send(
                new GetPlaylistDetailsExtendedRequest(request.Url),
                cancellationToken
            );
        }
        catch (Exception)
        {
            logger.Information("Couldn't resolve playlist info from url.");
        }

        // Get track details
        var trackDetails = await mediator.Send(
            new GetVideoDetailsRequest(request.Url, playlistDetailsExtended),
            cancellationToken
        );

        return new DownloadRequestDetails(trackDetails, playlistDetailsExtended);
    }
}
