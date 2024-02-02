namespace MusicDownloader.Business.Models;

public record DownloadRequestDetails(
    TrackDetails TrackDetails,
    PlaylistDetailsExtended? PlaylistDetailsExtended
);
