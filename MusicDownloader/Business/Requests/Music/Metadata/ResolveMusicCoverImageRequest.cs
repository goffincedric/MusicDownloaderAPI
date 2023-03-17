using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;

namespace MusicDownloader.Business.Requests.Music.Metadata;

public class ResolveMusicCoverImageRequest : IRequest<Stream?>
{
    public List<ThumbnailDetails> TrackThumbnails { get; set; }
    public List<ThumbnailDetails>? PlaylistThumbnails { get; set; }
}

public class ResolveMusicCoverImageRequestHandler : IRequestHandler<ResolveMusicCoverImageRequest, Stream?>
{
    private readonly HttpClient _httpClient;

    public ResolveMusicCoverImageRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Stream?> Handle(ResolveMusicCoverImageRequest request, CancellationToken cancellationToken)
    {
        // Check if any thumbnails are present
        if (
            request.TrackThumbnails.Count == 0 &&
            (request.PlaylistThumbnails is null || request.PlaylistThumbnails.Count == 0)
        ) return null;

        // Get best track and playlist thumbnails, if present
        var playlistThumbnail = request.PlaylistThumbnails?.GetWithHighestResolution();
        var trackThumbnail = request.TrackThumbnails.GetWithHighestResolution();
        /*
         * First, prefer playlist cover that satisfies minimum required resolution,
         * then track that satisfies same requirements,
         * then just take thumbnail with best resolution.
         */
        ThumbnailDetails thumbnail;
        if (playlistThumbnail?.Area >= YoutubeConstants.MinRequiredCoverResolution)
            thumbnail = playlistThumbnail;
        else if (trackThumbnail.Area >= YoutubeConstants.MinRequiredCoverResolution)
            thumbnail = trackThumbnail;
        else
            thumbnail = request.TrackThumbnails
                .Concat(request.PlaylistThumbnails ?? new List<ThumbnailDetails>())
                .GetWithHighestResolution();
        // Download thumbnail as stream and return
        return await _httpClient.GetStreamAsync(thumbnail.Url, cancellationToken);
    }
}