using MediatR;
using YoutubeExplode;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class GetVideoDetailsRequest : IRequest<YoutubeExplode.Videos.Video>
{
    public string Url { get; set; }
}

public class GetVideoDetailsRequestHandler : IRequestHandler<GetVideoDetailsRequest, YoutubeExplode.Videos.Video>
{
    private readonly YoutubeClient _youtube;

    public GetVideoDetailsRequestHandler(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public async Task<YoutubeExplode.Videos.Video> Handle(GetVideoDetailsRequest request, CancellationToken cancellationToken)
    {
        // Get playlist and videos
        return await _youtube.Videos.GetAsync(request.Url, cancellationToken);
    }
}