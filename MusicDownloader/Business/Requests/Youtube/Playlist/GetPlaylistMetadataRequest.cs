using MediatR;
using YoutubeExplode;
using YoutubeExplode.Playlists;

namespace MusicDownloader.Business.Requests.Youtube;

public class GetPlaylistMetadataRequest : IRequest<Playlist>
{
    public string Url { get; set; }
}

public class GetPlaylistMetadataRequestHandler : IRequestHandler<GetPlaylistMetadataRequest, Playlist>
{
    private readonly YoutubeClient _youtube;

    public GetPlaylistMetadataRequestHandler(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public async Task<Playlist> Handle(GetPlaylistMetadataRequest metadataRequest,
        CancellationToken cancellationToken)
    {
        // Get playlist and videos
        return await _youtube.Playlists.GetAsync(metadataRequest.Url, cancellationToken);
    }
}