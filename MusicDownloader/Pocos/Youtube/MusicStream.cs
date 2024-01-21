namespace MusicDownloader.Pocos.Youtube;

public class MusicStream
{
    public Stream Stream { get; set; }
    public string FileName { get; set; }
    public string Container { get; set; }
    public string ContentType { get; set; }
}