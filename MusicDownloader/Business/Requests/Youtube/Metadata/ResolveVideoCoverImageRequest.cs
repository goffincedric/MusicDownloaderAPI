using MediatR;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace MusicDownloader.Business.Requests.Youtube.Metadata;

public class ResolveVideoCoverImageRequest : IRequest<Stream>
{
    public IVideo Video { get; set; }
    public IPlaylist? Playlist { get; set; }
}

public class ResolveVideoCoverImageRequestHandler : IRequestHandler<ResolveVideoCoverImageRequest, Stream>
{
    private readonly HttpClient _httpClient;

    public ResolveVideoCoverImageRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Stream> Handle(ResolveVideoCoverImageRequest request, CancellationToken cancellationToken)
    {
        // Get thumbnail with best resolution, preferring album cover if part of playlist
        var thumbnail =
            request.Playlist?.Thumbnails.TryGetWithHighestResolution() ??
            request.Video.Thumbnails.GetWithHighestResolution();
        // Download image as stream and return
        return _httpClient.GetStreamAsync(thumbnail.Url, cancellationToken);
    }
}