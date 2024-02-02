using MediatR;
using MusicDownloader.Business.Requests.Music.Transcoding;
using MusicDownloader.Business.Strategies.MusicStream._base;

namespace MusicDownloader.Business.Requests.Music.Streaming;

public record StreamAudioRequest(
    string Url,
    string? Container,
    IMusicStreamStrategy StreamStrategy,
    HttpResponse HttpResponse
) : IRequest;

public class StreamAudioRequestHandler(IMediator mediator) : IRequestHandler<StreamAudioRequest>
{
    public async Task Handle(StreamAudioRequest request, CancellationToken cancellationToken)
    {
        // Resolve transcoder strategy from container
        var transcodingStrategy = await mediator.Send(
            new ResolveContainerTranscoderRequest(request.Container),
            cancellationToken
        );
        // Download url and transcode using resolved strategy
        await request.StreamStrategy.Execute(
            request.Url,
            transcodingStrategy,
            request.HttpResponse,
            cancellationToken
        );
    }
}
