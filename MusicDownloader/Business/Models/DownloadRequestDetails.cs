namespace MusicDownloader.Business.Models;

public class DownloadRequestDetails
{
    public TrackDetails TrackDetails { get; init; }
    public PlaylistDetailsExtended? PlaylistDetailsExtended { get; init; }
}