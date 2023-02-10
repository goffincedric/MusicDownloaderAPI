using FFMpegCore.Builders.MetaData;
using MediatR;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace MusicDownloader.Business.Requests.Youtube.Metadata;

public class ResolveVideoMetadataRequest : IRequest<MetaData>
{
    public IVideo Video { get; set; }
    public IPlaylist? Playlist { get; set; }
    public IReadOnlyList<PlaylistVideo>? PlaylistVideos { get; set; }
}

public class ResolveVideoMetadataRequestHandler : IRequestHandler<ResolveVideoMetadataRequest, MetaData>
{
    public Task<MetaData> Handle(ResolveVideoMetadataRequest request, CancellationToken cancellationToken)
    {
        var metadata = new MetaData();
        // Song details
        metadata.Entries.Add(MetadataConstants.VorbisTags.Title, request.Video.Title);
        metadata.Entries.Add(MetadataConstants.VorbisTags.Artist, CleanupAuthorName(request.Playlist?.Author?.ChannelTitle ?? request.Video.Author.ChannelTitle));
        if (request.Playlist?.Title != null) metadata.Entries.Add(MetadataConstants.VorbisTags.Album, request.Playlist.Title);
        if (request.PlaylistVideos != null)
        {
            int? index = request.PlaylistVideos
                .FindIndex(playlistVideo => playlistVideo.Id.Value == request.Video.Id.Value);
            if (index < 0) index = null;
            metadata.Entries.Add(MetadataConstants.VorbisTags.TrackNumber, $"{index + 1}");
        }
        return Task.FromResult(metadata);
    }

    private static readonly List<string> KeywordsToRemove = new()
    {
        " - Topic",
        "VEVO"
    };

    private static string CleanupAuthorName(string authorName)
    {
        // Cleanup author name
        KeywordsToRemove.ForEach((keyword) => authorName = authorName.Replace(keyword, ""));
        return authorName;
    }
}