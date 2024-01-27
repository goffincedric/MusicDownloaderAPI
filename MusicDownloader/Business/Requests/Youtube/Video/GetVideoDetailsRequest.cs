using MediatR;
using MusicDownloader.Business.Models;
using YoutubeReExplode;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class GetVideoDetailsRequest : IRequest<TrackDetails>
{
    public string Url { get; init; }
    public PlaylistDetailsExtended? PlaylistDetails { get; init; }
}

public class GetVideoDetailsRequestHandler(YoutubeClient youtube)
    : IRequestHandler<GetVideoDetailsRequest, TrackDetails>
{
    public async Task<TrackDetails> Handle(GetVideoDetailsRequest request, CancellationToken cancellationToken)
    {
        // Get playlist and videos
        var videoDetails = await youtube.Videos.GetAsync(request.Url, cancellationToken);

        // Map author name 
        var authorName =
            videoDetails.Author.ChannelName ??
            request.PlaylistDetails?.AuthorName ??
            videoDetails.Author.ChannelTitle;

        // Map music details
        MusicDetails? musicDetails = null;
        if (videoDetails.Music != null)
        {
            musicDetails = new MusicDetails
            {
                Song = videoDetails.Music.Song,
                Album = videoDetails.Music.Album,
                ArtistNames = videoDetails.Music.Artists?.ToList(),
            };
        }
        
        // Map thumbnails
        var thumbnails = videoDetails.Thumbnails.Select(thumbnail => new ThumbnailDetails
        {
            Url = thumbnail.Url,
            Width = thumbnail.Resolution.Width,
            Height = thumbnail.Resolution.Height
        });
        
        return new TrackDetails
        {
            Id = videoDetails.Id,
            Url = request.Url,
            Title = videoDetails.Title,
            AuthorName = authorName,
            Duration = videoDetails.Duration ?? new TimeSpan(),
            IsLive = videoDetails.IsLive,
            Thumbnails = thumbnails.ToList(),
            MusicDetails = musicDetails
        };
    }
}