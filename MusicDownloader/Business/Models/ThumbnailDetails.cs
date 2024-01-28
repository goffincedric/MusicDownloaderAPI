namespace MusicDownloader.Business.Models;

public class ThumbnailDetails
{
    /// <summary>
    /// Url of the thumbnail
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Height of the thumbnail in pixels
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Width of the thumbnail in pixels
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Total area of the thumbnail in pixels
    /// </summary>
    public int Area => Height * Width;
}
