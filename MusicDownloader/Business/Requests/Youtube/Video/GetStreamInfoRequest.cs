using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public record GetStreamInfoRequest(string Url) : IRequest<IStreamInfo>;

public class GetAudioUrlRequestHandler : IRequestHandler<GetStreamInfoRequest, IStreamInfo>
{
    private readonly YoutubeClient _youtubeClient;

    public GetAudioUrlRequestHandler(YoutubeClient youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }

    public async Task<IStreamInfo> Handle(
        GetStreamInfoRequest request,
        CancellationToken cancellationToken
    )
    {
        // Get highest quality audio stream
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(
            request.Url,
            cancellationToken
        );
        return streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
    }
}
