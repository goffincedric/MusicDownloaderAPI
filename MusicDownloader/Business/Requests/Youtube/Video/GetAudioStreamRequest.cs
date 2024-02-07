using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public record GetAudioStreamRequest(IStreamInfo StreamInfo) : IRequest<Stream>;

public class GetAudioStreamRequestHandler : IRequestHandler<GetAudioStreamRequest, Stream>
{
    private readonly YoutubeClient _youtubeClient;

    public GetAudioStreamRequestHandler(YoutubeClient youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }

    public async Task<Stream> Handle(
        GetAudioStreamRequest request,
        CancellationToken cancellationToken
    )
    {
        return await _youtubeClient.Videos.Streams.GetAsync(request.StreamInfo, cancellationToken);
    }
}
