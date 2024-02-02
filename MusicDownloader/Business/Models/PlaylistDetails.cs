namespace MusicDownloader.Business.Models;

public class PlaylistDetails
{
    /// <summary>
    /// Playlist identifier
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Url of the playlist
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Author of the playlist
    /// </summary>
    public string AuthorName { get; set; }

    /// <summary>
    /// Title of the playlist
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// List of thumbnails corresponding to playlist
    /// </summary>
    public List<ThumbnailDetails> Thumbnails { get; init; } = new();
}

public class PlaylistDetailsExtended : PlaylistDetails
{
    public List<TrackDetails> Tracks { get; set; }
}
