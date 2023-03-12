using MediatR;
using YoutubeReExplode;
using YoutubeReExplode.Common;
using YoutubeReExplode.Playlists;

namespace MusicDownloader.Business.Requests.Youtube.Playlist;

public class GetPlaylistVideosRequest : IRequest<IEnumerable<PlaylistVideo>>
{
    public string Url { get; set; }
    public bool IncludeLiveStreams { get; set; } = false;
}

public class GetPlaylistVideosRequestHandler : IRequestHandler<GetPlaylistVideosRequest, IEnumerable<PlaylistVideo>>
{
    private readonly YoutubeClient _youtube;

    public GetPlaylistVideosRequestHandler(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public async Task<IEnumerable<PlaylistVideo>> Handle(GetPlaylistVideosRequest metadataRequest,
        CancellationToken cancellationToken)
    {
        // Get playlist and videos
        var playlistVideosTask = _youtube.Playlists.GetVideosAsync(metadataRequest.Url, cancellationToken);
        if (metadataRequest.IncludeLiveStreams) return await playlistVideosTask;
        var playlistVideos = await playlistVideosTask;
        return playlistVideos.Where(video => !video.IsLive);
    }
}