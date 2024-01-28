namespace MusicDownloader.Business.Models;

public class TrackDetails
{
    /// <summary>
    /// Track identifier
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Url of the track
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Author of the video/audio url
    /// </summary>
    public string AuthorName { get; init; }

    /// <summary>
    /// Title of the track
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Duration of the track
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Flag indicating that track is currently being broadcast live
    /// </summary>
    public bool? IsLive { get; init; }

    /// <summary>
    /// List of thumbnails corresponding to track
    /// </summary>
    public List<ThumbnailDetails> Thumbnails { get; init; } = new();

    /// <summary>
    /// Music details about the track
    /// </summary>
    public MusicDetails? MusicDetails { get; init; }
}

public class TrackDetailsExtended : TrackDetails
{
    /// <summary>
    /// Playlist details of the track
    /// </summary>
    public PlaylistDetails PlaylistDetails { get; set; }
}
