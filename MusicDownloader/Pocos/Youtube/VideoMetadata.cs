using YoutubeReExplode.Common;

namespace MusicDownloader.Pocos.Youtube;

public class VideoMetadata
{
    public string Title { get; set;  }

    public string Author { get; set;  }

    public string? Album { get; set;  }

    public int? TrackNumber { get; set;  }

    public string Url { get; set;  }

    public bool IsLive { get; set;  }

    public Thumbnail Thumbnail { get; set;  }

    public VideoMetadata()
    {
    }

    public VideoMetadata(
        string title, string author, string? album, int? trackNumber, string url, bool isLive, Thumbnail thumbnail
    )
    {
        Title = title;
        Author = author;
        Album = album;
        TrackNumber = trackNumber;
        Url = url;
        IsLive = isLive;
        Thumbnail = thumbnail;
    }
}