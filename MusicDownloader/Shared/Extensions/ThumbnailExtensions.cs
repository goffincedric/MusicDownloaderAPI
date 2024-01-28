using MusicDownloader.Business.Models;

namespace MusicDownloader.Shared.Extensions;

public static class ThumbnailExtensions
{
    public static ThumbnailDetails GetWithHighestResolution(
        this IEnumerable<ThumbnailDetails> thumbnails
    ) => thumbnails.OrderByDescending(t => t.Area).First();
}
