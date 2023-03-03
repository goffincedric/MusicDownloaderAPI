using MediatR;
using YoutubeReExplode;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class GetVideoDetailsRequest : IRequest<YoutubeReExplode.Videos.Video>
{
    public string Url { get; set; }
}

public class GetVideoDetailsRequestHandler : IRequestHandler<GetVideoDetailsRequest, YoutubeReExplode.Videos.Video>
{
    private readonly YoutubeClient _youtube;

    public GetVideoDetailsRequestHandler(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public async Task<YoutubeReExplode.Videos.Video> Handle(GetVideoDetailsRequest request, CancellationToken cancellationToken)
    {
        // Get playlist and videos
        return await _youtube.Videos.GetAsync(request.Url, cancellationToken);
    }
}