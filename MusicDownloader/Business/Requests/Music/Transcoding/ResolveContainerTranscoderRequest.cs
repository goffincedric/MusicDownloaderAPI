using MediatR;
using MusicDownloader.Business.Strategies.Transcoding;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Business.Requests.Music.Transcoding;

public class ResolveContainerTranscoderRequest : IRequest<TranscoderStrategy>
{
    public string Container { get; init; }
}

public class
    ResolveContainerTranscoderRequestHandler : IRequestHandler<ResolveContainerTranscoderRequest, TranscoderStrategy>
{
    private readonly IMediator _mediator;

    public ResolveContainerTranscoderRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public Task<TranscoderStrategy> Handle(
        ResolveContainerTranscoderRequest request, CancellationToken cancellationToken
    ) =>
        Task.FromResult<TranscoderStrategy>(request.Container switch
        {
            ContainerConstants.Containers.Mp3 => new Mp3Transcoder(),
            ContainerConstants.Containers.Ogg => new OggTranscoder(),
            _ => new DefaultTranscoder(_mediator),
        });
}