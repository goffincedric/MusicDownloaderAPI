namespace MusicDownloader.Pocos.Youtube;

public class TrackMetadata
{
    public string Title { get; set;  }

    public string Author { get; set;  }

    public string? Album { get; set;  }

    public int? TrackNumber { get; set;  }

    public TrackMetadata()
    {
    }

    public TrackMetadata(
        string title, string author, string? album, int? trackNumber
    )
    {
        Title = title;
        Author = author;
        Album = album;
        TrackNumber = trackNumber;
    }
}