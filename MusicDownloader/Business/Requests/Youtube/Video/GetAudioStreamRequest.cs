using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class GetAudioStreamRequest: IRequest<Stream>
{
    public string Url { get; set; }
}

public class GetAudioStreamRequestHandler: IRequestHandler<GetAudioStreamRequest, Stream>
{
    private readonly YoutubeClient _youtubeClient;

    public GetAudioStreamRequestHandler(YoutubeClient youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }
    
    public async Task<Stream> Handle(GetAudioStreamRequest request, CancellationToken cancellationToken)
    {    
        // Get highest quality audio stream
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(request.Url, cancellationToken);
        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        return await _youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
    }
}