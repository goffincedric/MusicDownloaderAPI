using MediatR;
using YoutubeReExplode;

namespace MusicDownloader.Business.Requests.Youtube.Playlist;

public class GetPlaylistDetailsRequest : IRequest<YoutubeReExplode.Playlists.Playlist>
{
    public string Url { get; set; }
}

public class GetPlaylistDetailsRequestHandler : IRequestHandler<GetPlaylistDetailsRequest, YoutubeReExplode.Playlists.Playlist>
{
    private readonly YoutubeClient _youtube;

    public GetPlaylistDetailsRequestHandler(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public async Task<YoutubeReExplode.Playlists.Playlist> Handle(GetPlaylistDetailsRequest metadataRequest,
        CancellationToken cancellationToken)
    {
        // Get playlist and videos
        return await _youtube.Playlists.GetAsync(metadataRequest.Url, cancellationToken);
    }
}