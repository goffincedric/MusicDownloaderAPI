using System.Net;
using FFMpegCore;
using FFMpegCore.Builders.MetaData;
using FFMpegCore.Pipes;
using MediatR;
using MusicDownloader.Business.Requests.Youtube.Metadata;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Edxceptions;
using YoutubeReExplode;
using YoutubeReExplode.Playlists;
using YoutubeReExplode.Videos;
using YoutubeReExplode.Videos.Streams;
using VideoStream = MusicDownloader.Pocos.Youtube.VideoStream;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class DownloadAudioRequest : IRequest<VideoStream>
{
    public IMusicVideo Video { get; set; }
    public IPlaylist? Playlist { get; set; }
    public List<PlaylistVideo>? PlaylistVideos { get; set; }
}

public class DownloadAudioRequestHandler : IRequestHandler<DownloadAudioRequest, VideoStream>
{
    private readonly IMediator _mediator;
    private readonly YoutubeClient _youtube;

    public DownloadAudioRequestHandler(IMediator mediator, YoutubeClient youtube)
    {
        _mediator = mediator;
        _youtube = youtube;
    }

    public async Task<VideoStream> Handle(DownloadAudioRequest request, CancellationToken cancellationToken)
    {
        // Check if video is livestream
        if (request.Video.IsLive)
            throw new MusicDownloaderException(
                "Livestreams cannot be downloaded",
                ErrorCodes.Youtube.LivestreamDownloadNotAllowed,
                HttpStatusCode.BadRequest
            );
        // Check if video doesn't go over allowed maximum duration
        if (request.Video.Duration > YoutubeConstants.MaxAllowedDownloadDuration)
            throw new MusicDownloaderException(
                $@"Videos longer than {YoutubeConstants.MaxAllowedDownloadDuration.ToString()} cannot be downloaded",
                ErrorCodes.Youtube.LongVideoDownloadNotAllowed,
                HttpStatusCode.BadRequest
            );

        // Get highest quality audio stream
        var streamInfo = await GetAudioStream(request.Video, cancellationToken);
        var streamTask = _youtube.Videos.Streams.GetAsync(streamInfo, cancellationToken);

        // Get cover art as stream
        var coverStreamTask = _mediator.Send(new ResolveVideoCoverImageRequest
        {
            Playlist = request.Playlist,
            Video = request.Video
        }, cancellationToken);

        // Create metadata
        var videoMetadata = await _mediator.Send(new ResolveVideoMetadataRequest
        {
            Video = request.Video,
            Playlist = request.Playlist,
            PlaylistVideos = request.PlaylistVideos
        }, cancellationToken);
        var metadata = MapMetadata(videoMetadata);

        // Transcode to ogg and encode metadata
        var memoryStream = new MemoryStream();
        await FFMpegArguments
            .FromPipeInput(new StreamPipeSource(await streamTask))
            .AddPipeInput(new StreamPipeSource(await coverStreamTask))
            .AddMetaData(metadata)
            .OutputToPipe(new StreamPipeSink(memoryStream),
                options => options
                    .ForceFormat(YoutubeConstants.Container)
                    .WithAudioCodec(YoutubeConstants.AudioCodec)
                    .WithAudioBitrate(YoutubeConstants.AudioQuality)
                    .WithAudioSamplingRate(YoutubeConstants.SamplingRate)
                    .WithVideoCodec(YoutubeConstants.VideoCodec)
                    .WithFramerate(YoutubeConstants.CoverFramerate)
                    .WithCustomArgument("-q:v 10") // Highest quality video
                    .UsingMultithreading(true)
                    .WithFastStart()
            )
            .ProcessAsynchronously();
        memoryStream.Position = 0;

        // Add metadata
        return new VideoStream { Stream = memoryStream };
    }

    private static IReadOnlyMetaData MapMetadata(VideoMetadata videoMetadata)
    {
        var metadata = new MetaData();
        // Map metadata
        metadata.Entries.Add(MetadataConstants.VorbisTags.Artist, videoMetadata.Author);
        metadata.Entries.Add(MetadataConstants.VorbisTags.Title, videoMetadata.Title);
        if (videoMetadata.Album != null)
            metadata.Entries.Add(MetadataConstants.VorbisTags.Album, videoMetadata.Album);
        if (videoMetadata.TrackNumber.HasValue)
            metadata.Entries.Add(MetadataConstants.VorbisTags.TrackNumber, videoMetadata.TrackNumber.Value.ToString());
        // Return metadata
        return metadata;
    }

    private async Task<IStreamInfo> GetAudioStream(IVideo video, CancellationToken cancellationToken)
    {
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(video.Url, cancellationToken);
        return streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
    }
}