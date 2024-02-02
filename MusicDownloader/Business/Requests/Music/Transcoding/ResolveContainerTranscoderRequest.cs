using MediatR;
using MusicDownloader.Business.Strategies.Transcoding;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Business.Requests.Music.Transcoding;

public record ResolveContainerTranscoderRequest(string? Container) : IRequest<TranscoderStrategy?>;

public class ResolveContainerTranscoderRequestHandler()
    : IRequestHandler<ResolveContainerTranscoderRequest, TranscoderStrategy?>
{
    public Task<TranscoderStrategy?> Handle(
        ResolveContainerTranscoderRequest request,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult<TranscoderStrategy?>(
            request.Container switch
            {
                ContainerConstants.Containers.Mp3 => new Mp3Transcoder(),
                ContainerConstants.Containers.Ogg => new OggTranscoder(),
                _ => null,
            }
        );
}
