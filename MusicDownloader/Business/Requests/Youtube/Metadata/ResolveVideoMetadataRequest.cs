using System.Text.RegularExpressions;
using MediatR;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using YoutubeReExplode.Common;
using YoutubeReExplode.Playlists;
using YoutubeReExplode.Videos;

namespace MusicDownloader.Business.Requests.Youtube.Metadata;

public class ResolveVideoMetadataRequest : IRequest<VideoMetadata>
{
    public IMusicVideo Video { get; set; }
    public IPlaylist? Playlist { get; set; }
    public List<PlaylistVideo>? PlaylistVideos { get; set; }
}

public class ResolveVideoMetadataRequestHandler : IRequestHandler<ResolveVideoMetadataRequest, VideoMetadata>
{
    public Task<VideoMetadata> Handle(ResolveVideoMetadataRequest request, CancellationToken cancellationToken)
    {
        // Author name
        string author;
        if (request.Video.Music?.Artists?.Length > 0)
            author = string.Join(", ", request.Video.Music.Artists);
        else
            author = CleanupAuthorName(
                request.Video.Author.ChannelName ??
                request.Playlist?.Author?.ChannelTitle ??
                request.Video.Author.ChannelTitle
            );

        // Song name
        var title = request.Video.Music?.Song ?? CleanupTitle(request.Video.Title, author);
        // Playlist or album name
        string? playlist = null;
        if (request.Video.Music?.Album != null)
            playlist = request.Video.Music.Album;
        else if (request.Playlist?.Title != null)
            playlist = request.Playlist.Title;
        // Track number
        int? trackNumber = null;
        if (request.Playlist != null && request.PlaylistVideos != null)
        {
            int? index = request.PlaylistVideos
                .FindIndex(playlistVideo => playlistVideo.Id.Value == request.Video.Id.Value);
            if (index >= 0) trackNumber = index + 1;
        }

        // Map and return metadata
        return Task.FromResult(new VideoMetadata
        {
            Title = title,
            Author = author,
            Album = playlist,
            TrackNumber = trackNumber,
            Url = request.Video.Url,
            IsLive = request.Video.IsLive,
            Thumbnail = request.Video.Thumbnails.TryGetWithHighestResolution()!
        });
    }

    private static readonly List<Regex> AuthorRegexes = RegexConstants.Youtube.VEVO_REGEXES.Concat(new List<Regex>
    {
        new("-\\stopic", RegexOptions.IgnoreCase),
        new("VEVO$"),
        new("^VEVO"),
    }).ToList();

    private static string CleanupAuthorName(string authorName)
    {
        // Cleanup author name
        AuthorRegexes.ForEach(regex =>
            authorName = regex.Replace(authorName, ""));
        return authorName.Trim();
    }

    private static readonly List<Regex> SongRegex = RegexConstants.Youtube.VEVO_REGEXES.Concat(new List<Regex>
    {
        new("-?\\s?\\([^(]+\\)\\s?-?", RegexOptions.IgnoreCase), // Everything in between brackets
        new("-?\\s?\\[[^(]+\\]\\s?-?", RegexOptions.IgnoreCase), // Everything in between square brackets
    }).ToList();

    private static List<Regex> GetSongAuthorRegexes(string authorName)
    {
        return new List<Regex>
        {
            new("\\s?-\\s?" + Regex.Escape(authorName), RegexOptions.IgnoreCase),
            new(Regex.Escape(authorName) + "\\s?-\\s?", RegexOptions.IgnoreCase),
        };
    }

    private static string CleanupTitle(string title, string authorName)
    {
        // Cleanup with various song regexes
        SongRegex.ForEach(keyword =>
            title = keyword.Replace(title, ""));
        // Cleanup author name from title with different formats
        GetSongAuthorRegexes(authorName).ForEach(regex => title = regex.Replace(title, ""));
        // TODO: Cleanup emoji's from string
        // Cleanup any dashes at start and end
        title = new Regex("\\s?-\\s?$").Replace(title, "");
        title = new Regex("^\\s?-\\s?").Replace(title, "");
        return title.Trim();
    }
}


// Test data songs
/*
https://www.youtube.com/watch?v=dI3xkL7qUAc
https://www.youtube.com/watch?v=HQnC1UHBvWA
https://www.youtube.com/watch?v=j5uAR9w7LBg
https://www.youtube.com/watch?v=G7KNmW9a75Y 
 */