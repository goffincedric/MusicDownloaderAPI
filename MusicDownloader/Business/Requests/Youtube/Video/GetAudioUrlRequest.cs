using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public record GetAudioUrlRequest(string Url) : IRequest<string>;

public class GetAudioUrlRequestHandler(YoutubeClient youtubeClient)
    : IRequestHandler<GetAudioUrlRequest, string>
{
    public async Task<string> Handle(
        GetAudioUrlRequest request,
        CancellationToken cancellationToken
    )
    {
        // Get highest quality audio stream
        var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(
            request.Url,
            cancellationToken
        );
        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        return streamInfo.Url;
    }
}
