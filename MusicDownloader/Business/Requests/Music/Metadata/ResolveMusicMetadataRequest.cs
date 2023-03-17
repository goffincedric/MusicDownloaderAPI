using System.Text.RegularExpressions;
using MediatR;
using MusicDownloader.Business.Models;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Business.Requests.Music.Metadata;

public class ResolveMusicMetadataRequest : IRequest<TrackMetadata>
{
    public TrackDetails TrackDetails { get; set; }
    public PlaylistDetailsExtended? PlaylistDetails { get; set; }
}

public class ResolveMusicMetadataRequestHandler : IRequestHandler<ResolveMusicMetadataRequest, TrackMetadata>
{
    public Task<TrackMetadata> Handle(ResolveMusicMetadataRequest request, CancellationToken cancellationToken)
    {
        // Author name
        var author = request.TrackDetails.MusicDetails?.ArtistNames?.Count > 0
            ? string.Join(", ", request.TrackDetails.MusicDetails.ArtistNames)
            : CleanupAuthorName(request.TrackDetails.AuthorName);

        // Song name
        var title = request.TrackDetails.MusicDetails?.Song ?? CleanupTitle(request.TrackDetails.Title, author);

        // Playlist or album name
        var playlist = request.PlaylistDetails?.Title;
        if (request.TrackDetails.MusicDetails?.Album != null)
            playlist = request.TrackDetails.MusicDetails.Album;

        // Track number
        int? trackNumber = null;
        if (request.PlaylistDetails?.Tracks.Count > 0)
        {
            var index = request.PlaylistDetails.Tracks
                .FindIndex(track => track.Id == request.TrackDetails.Id);
            trackNumber = index + 1;
        }

        // Map and return metadata
        return Task.FromResult(new TrackMetadata
        {
            Title = title,
            Author = author,
            Album = playlist,
            TrackNumber = trackNumber,
        });
    }

    private static string CleanupAuthorName(string authorName)
    {
        // Cleanup author name
        AuthorRegexes.ForEach(regex => authorName = regex.Replace(authorName, ""));
        return authorName.Trim();
    }

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

    #region Regexes

    private static readonly List<Regex> AuthorRegexes = RegexConstants.Youtube.VEVO_REGEXES.Concat(new List<Regex>
    {
        new("-\\stopic", RegexOptions.IgnoreCase),
        new("VEVO$"),
        new("^VEVO"),
    }).ToList();

    private static readonly List<Regex> SongRegex = RegexConstants.Youtube.VEVO_REGEXES.Concat(new List<Regex>
    {
        new("-?\\s?\\([^)]+\\)\\s?-?", RegexOptions.IgnoreCase), // Everything in between brackets
        new("-?\\s?\\[[^]]+\\]\\s?-?", RegexOptions.IgnoreCase), // Everything in between square brackets
    }).ToList();

    #endregion
}