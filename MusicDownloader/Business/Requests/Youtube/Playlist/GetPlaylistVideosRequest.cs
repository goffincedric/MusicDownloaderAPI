using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Common;
using YoutubeReExplode.Playlists;

namespace MusicDownloader.Business.Requests.Youtube;

public class GetPlaylistVideosRequest : IRequest<IReadOnlyList<PlaylistVideo>>
{
    public string Url { get; set; }
}

public class GetPlaylistVideosRequestHandler : IRequestHandler<GetPlaylistVideosRequest, IReadOnlyList<PlaylistVideo>>
{
    private readonly YoutubeClient _youtube;

    public GetPlaylistVideosRequestHandler(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public async Task<IReadOnlyList<PlaylistVideo>> Handle(GetPlaylistVideosRequest metadataRequest,
        CancellationToken cancellationToken)
    {
        // Get playlist and videos
        return await _youtube.Playlists.GetVideosAsync(metadataRequest.Url, cancellationToken);
    }
}