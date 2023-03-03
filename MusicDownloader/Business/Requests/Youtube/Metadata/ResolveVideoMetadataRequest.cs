using System.Text.RegularExpressions;
using FFMpegCore.Builders.MetaData;
using MediatR;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;
using YoutubeReExplode.Playlists;
using YoutubeReExplode.Videos;

namespace MusicDownloader.Business.Requests.Youtube.Metadata;

public class ResolveVideoMetadataRequest : IRequest<MetaData>
{
    public IMusicVideo Video { get; set; }
    public IPlaylist? Playlist { get; set; }
    public IReadOnlyList<PlaylistVideo>? PlaylistVideos { get; set; }
}

public class ResolveVideoMetadataRequestHandler : IRequestHandler<ResolveVideoMetadataRequest, MetaData>
{
    public Task<MetaData> Handle(ResolveVideoMetadataRequest request, CancellationToken cancellationToken)
    {
        // Author name
        string author;
        if (request.Video.Music?.Artists?.Length > 0)
        {
            author = string.Join(", ", request.Video.Music.Artists);
        }
        // else if (RegexConstants.Youtube.ARTIST_EXTRACTION.IsMatch(request.Video))
        // {
        //     
        // }
        else
        {
            author = CleanupAuthorName(request.Video.Author.ChannelName ?? request.Playlist?.Author?.ChannelTitle ?? request.Video.Author.ChannelTitle);
        }

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

        // Add metadata
        var metadata = new MetaData();
        metadata.Entries.Add(MetadataConstants.VorbisTags.Artist, author);
        metadata.Entries.Add(MetadataConstants.VorbisTags.Title, title);
        if (playlist != null)
            metadata.Entries.Add(MetadataConstants.VorbisTags.Album, playlist);
        if (trackNumber.HasValue)
            metadata.Entries.Add(MetadataConstants.VorbisTags.TrackNumber, trackNumber.Value.ToString());

        // Return metadata
        return Task.FromResult(metadata);
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