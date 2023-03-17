namespace MusicDownloader.Business.Models;

public class ThumbnailDetails
{
    public string Url { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int Area => Height * Width;
}