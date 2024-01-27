using MediatR;
using MusicDownloader.Business.Models;
using YoutubeReExplode;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class GetAudioStreamRequest: IRequest<DownloadStreamInfo>
{
    public string Url { get; set; }
}

public class GetAudioStreamRequestHandler(YoutubeClient youtubeClient) : IRequestHandler<GetAudioStreamRequest, DownloadStreamInfo>
{
    public async Task<DownloadStreamInfo> Handle(GetAudioStreamRequest request, CancellationToken cancellationToken)
    {    
        // Get highest quality audio stream
        var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(request.Url, cancellationToken);
        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        var stream = await youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
        return new DownloadStreamInfo(stream, streamInfo.Container.Name);
    }
}