using MediatR;
using MusicDownloader.Shared.Constants;
using YoutubeReExplode.Common;
using YoutubeReExplode.Playlists;
using YoutubeReExplode.Videos;

namespace MusicDownloader.Business.Requests.Youtube.Metadata;

public class ResolveVideoCoverImageRequest : IRequest<Stream>
{
    public IMusicVideo Video { get; set; }
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
        // Get best video and playlist thumbnails, if present
        var playlistThumbnail = request.Playlist?.Thumbnails.TryGetWithHighestResolution();
        var videoThumbnail = request.Video.Thumbnails.TryGetWithHighestResolution();
        /*
         * First, prefer playlist cover that satisfies minimum required resolution,
         * then video that satisfies same requirements,
         * then just take thumbnail with best resolution.
         */
        Thumbnail thumbnail;
        if (playlistThumbnail?.Resolution.Area >= YoutubeConstants.MinRequiredCoverResolution)
            thumbnail = playlistThumbnail;
        else if (videoThumbnail?.Resolution.Area >= YoutubeConstants.MinRequiredCoverResolution)
            thumbnail = videoThumbnail;
        else
            thumbnail = request.Video.Thumbnails
                .Concat(request.Playlist?.Thumbnails ?? Array.Empty<Thumbnail>())
                .GetWithHighestResolution();
        // Download thumbnail as stream and return
        return _httpClient.GetStreamAsync(thumbnail.Url, cancellationToken);
    }
}