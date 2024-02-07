using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Youtube.Playlist;
using MusicDownloader.Business.Requests.Youtube.Video;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Business.Requests.Youtube.Download;

public record GetDownloadRequestDetailsRequest(string Url) : IRequest<DownloadRequestDetails>;

public class GetDownloadRequestDetailsRequestHandler
    : IRequestHandler<GetDownloadRequestDetailsRequest, DownloadRequestDetails>
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public GetDownloadRequestDetailsRequestHandler(IMediator mediator, ILogger logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<DownloadRequestDetails> Handle(
        GetDownloadRequestDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        // Get playlist details
        PlaylistDetailsExtended? playlistDetailsExtended = null;
        try
        {
            playlistDetailsExtended = await _mediator.Send(
                new GetPlaylistDetailsExtendedRequest(request.Url),
                cancellationToken
            );
        }
        catch (Exception)
        {
            _logger.Information("Couldn't resolve playlist info from url.");
        }

        // Get track details
        var trackDetails = await _mediator.Send(
            new GetVideoDetailsRequest(request.Url, playlistDetailsExtended),
            cancellationToken
        );

        return new DownloadRequestDetails(trackDetails, playlistDetailsExtended);
    }
}
