using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;

namespace MusicDownloader.Business.Requests.Music.Metadata;

public record ResolveMusicCoverImageRequest(
    List<ThumbnailDetails> TrackThumbnails,
    List<ThumbnailDetails>? PlaylistThumbnails
) : IRequest<Stream?>;

public class ResolveMusicCoverImageRequestHandler(HttpClient httpClient)
    : IRequestHandler<ResolveMusicCoverImageRequest, Stream?>
{
    public async Task<Stream?> Handle(
        ResolveMusicCoverImageRequest request,
        CancellationToken cancellationToken
    )
    {
        // Check if any thumbnails are present
        if (
            request.TrackThumbnails.Count == 0
            && (request.PlaylistThumbnails is null || request.PlaylistThumbnails.Count == 0)
        )
            return null;

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
            thumbnail = request
                .TrackThumbnails.Concat(request.PlaylistThumbnails ?? new List<ThumbnailDetails>())
                .GetWithHighestResolution();
        // Download thumbnail as stream and return
        return await httpClient.GetStreamAsync(thumbnail.Url, cancellationToken);
    }
}
