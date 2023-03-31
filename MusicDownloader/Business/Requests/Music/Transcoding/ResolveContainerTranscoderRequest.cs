using System.Net;
using MediatR;
using MusicDownloader.Business.Strategies.Transcoding;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Edxceptions;

namespace MusicDownloader.Business.Requests.Music.Transcoding;

public class ResolveContainerTranscoderRequest : IRequest<ITranscoderStrategy>
{
    public string Container { get; init; }
}

public class
    ResolveContainerTranscoderRequestHandler : IRequestHandler<ResolveContainerTranscoderRequest, ITranscoderStrategy>
{
    public Task<ITranscoderStrategy> Handle(
        ResolveContainerTranscoderRequest request, CancellationToken cancellationToken
    ) =>
        Task.FromResult<ITranscoderStrategy>(request.Container switch
        {
            ContainerConstants.Containers.Mp3 => new Mp3Transcoder(),
            ContainerConstants.Containers.Ogg => new OggTranscoder(),
            _ => throw new MusicDownloaderException($"No transcoding strategy found for container: {request.Container}",
                ErrorCodes.UnsupportedAudioContainer, HttpStatusCode.BadRequest)
        });
}