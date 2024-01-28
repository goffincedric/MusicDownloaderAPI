namespace MusicDownloader.Business.Models;

public class MusicDetails
{
    /// <summary>
    /// Song in a track
    /// </summary>
    public string? Song { get; set; }

    /// <summary>
    /// Album of a track
    /// </summary>
    public string? Album { get; set; }

    /// <summary>
    /// Artists of a song
    /// </summary>
    public List<string>? ArtistNames { get; set; }
}
