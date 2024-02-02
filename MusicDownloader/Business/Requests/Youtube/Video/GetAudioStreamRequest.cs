using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public record GetAudioStreamRequest(IStreamInfo StreamInfo) : IRequest<Stream>;

public class GetAudioStreamRequestHandler(YoutubeClient youtubeClient)
    : IRequestHandler<GetAudioStreamRequest, Stream>
{
    public async Task<Stream> Handle(
        GetAudioStreamRequest request,
        CancellationToken cancellationToken
    )
    {
        return await youtubeClient.Videos.Streams.GetAsync(request.StreamInfo, cancellationToken);
    }
}
