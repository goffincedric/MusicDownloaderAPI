using MediatR;
using MusicDownloader.Business.Requests.Music.Transcoding;
using MusicDownloader.Business.Strategies.MusicDownload._base;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Requests.Music.Download;

public record DownloadAudioRequest(
    string Url,
    string? Container,
    IMusicDownloadStrategy DownloadStrategy
) : IRequest<MusicStream>;

public class DownloadAudioRequestHandler(IMediator mediator)
    : IRequestHandler<DownloadAudioRequest, MusicStream>
{
    public async Task<MusicStream> Handle(
        DownloadAudioRequest request,
        CancellationToken cancellationToken
    )
    {
        // Resolve transcoder strategy from container
        var transcodingStrategy = await mediator.Send(
            new ResolveContainerTranscoderRequest(request.Container),
            cancellationToken
        );
        // Download url and transcode using resolved strategy
        return await request.DownloadStrategy.Execute(
            request.Url,
            transcodingStrategy,
            cancellationToken
        );
    }
}
