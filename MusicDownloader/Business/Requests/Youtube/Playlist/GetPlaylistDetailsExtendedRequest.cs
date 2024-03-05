using MediatR;
using MusicDownloader.Business.Models;
using YoutubeReExplode;
using YoutubeReExplode.Common;

namespace MusicDownloader.Business.Requests.Youtube.Playlist;

public record GetPlaylistDetailsExtendedRequest(string Url, bool IncludeLiveStreams = false)
    : IRequest<PlaylistDetailsExtended>;

public class GetPlaylistDetailsExtendedRequestHandler
    : IRequestHandler<GetPlaylistDetailsExtendedRequest, PlaylistDetailsExtended>
{
    private readonly YoutubeClient _youtube;

    public GetPlaylistDetailsExtendedRequestHandler(YoutubeClient youtube)
    {
        _youtube = youtube;
    }

    public async Task<PlaylistDetailsExtended> Handle(
        GetPlaylistDetailsExtendedRequest metadataRequest,
        CancellationToken cancellationToken
    )
    {
        // Get playlist and videos
        var playlistTask = _youtube
            .Playlists.GetAsync(metadataRequest.Url, cancellationToken)
            .AsTask();
        var playlistVideosTask = _youtube
            .Playlists.GetVideosAsync(metadataRequest.Url, cancellationToken)
            .CollectAsync()
            .AsTask();

        // Await tasks
        await Task.WhenAll(playlistTask, playlistVideosTask);
        var playlist = playlistTask.Result;
        var playlistVideos = playlistVideosTask.Result;

        // Filter out livestreams if necessary
        if (!metadataRequest.IncludeLiveStreams)
            playlistVideos = playlistVideos.Where(video => !video.IsLive).ToList();

        // Map tracks
        var tracks = playlistVideos.Select(video => new TrackDetails
        {
            Id = video.Id,
            Url = video.Url,
            AuthorName = video.Author.ChannelName ?? video.Author.ChannelTitle,
            Title = video.Title,
            Duration = video.Duration ?? new TimeSpan(),
            IsLive = video.IsLive,
            Thumbnails = video
                .Thumbnails.Select(thumbnail => new ThumbnailDetails
                {
                    Url = thumbnail.Url,
                    Width = thumbnail.Resolution.Width,
                    Height = thumbnail.Resolution.Height
                })
                .ToList()
        });

        // Map thumbnails
        var thumbnails = playlist.Thumbnails.Select(thumbnail => new ThumbnailDetails
        {
            Url = thumbnail.Url,
            Width = thumbnail.Resolution.Width,
            Height = thumbnail.Resolution.Height
        });

        return new PlaylistDetailsExtended
        {
            Id = playlist.Id,
            Url = playlist.Url,
            AuthorName = playlist.Author?.ChannelTitle ?? playlist.Author?.ChannelTitle!,
            Title = playlist.Title,
            Tracks = tracks.ToList(),
            Thumbnails = thumbnails.ToList()
        };
    }
}
