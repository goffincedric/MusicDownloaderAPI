using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class GetAudioUrlRequest : IRequest<string>
{
    public string Url { get; set; }
}

public class GetAudioUrlRequestHandler : IRequestHandler<GetAudioUrlRequest, string>
{
    private readonly YoutubeClient _youtubeClient;

    public GetAudioUrlRequestHandler(YoutubeClient youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }

    public async Task<string> Handle(GetAudioUrlRequest request, CancellationToken cancellationToken)
    {
        // Get highest quality audio stream
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(request.Url, cancellationToken);
        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        return streamInfo.Url;
    }
}