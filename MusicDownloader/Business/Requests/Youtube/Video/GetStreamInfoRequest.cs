using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public record GetStreamInfoRequest(string Url) : IRequest<IStreamInfo>;

public class GetAudioUrlRequestHandler(YoutubeClient youtubeClient)
    : IRequestHandler<GetStreamInfoRequest, IStreamInfo>
{
    public async Task<IStreamInfo> Handle(
        GetStreamInfoRequest request,
        CancellationToken cancellationToken
    )
    {
        // Get highest quality audio stream
        var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(
            request.Url,
            cancellationToken
        );
        return streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
    }
}
