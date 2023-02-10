using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using MediatR;
using MusicDownloader.Business.Requests.Youtube.Metadata;
using MusicDownloader.Shared.Constants;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using VideoStream = MusicDownloader.Pocos.Youtube.VideoStream;

namespace MusicDownloader.Business.Requests.Youtube.Video;

public class DownloadAudioRequest : IRequest<VideoStream>
{
    public IVideo Video { get; set; }
    public IPlaylist? Playlist { get; set; }
    public IReadOnlyList<PlaylistVideo>? PlaylistVideos { get; set; }
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
        var metadataTask = _mediator.Send(new ResolveVideoMetadataRequest
        {
            Video = request.Video,
            Playlist = request.Playlist,
            PlaylistVideos = request.PlaylistVideos
        }, cancellationToken);

        // Transcode to ogg and encode metadata
        var memoryStream = new MemoryStream();
        await FFMpegArguments
            .FromPipeInput(new StreamPipeSource(await streamTask))
            .AddPipeInput(new StreamPipeSource(await coverStreamTask))
            .AddMetaData(await metadataTask)
            .OutputToPipe(new StreamPipeSink(memoryStream),
                options => options
                    .ForceFormat(YoutubeConstants.Container)
                    .WithAudioCodec(YoutubeConstants.AudioCodec)
                    .WithAudioBitrate(YoutubeConstants.AudioQuality)
                    .WithAudioSamplingRate(YoutubeConstants.SamplingRate)
                    .WithVideoCodec(YoutubeConstants.VideoCodec)
                    .WithCustomArgument("-q:v 10") // Highest quality video
                    .UsingMultithreading(true)
                    .WithFastStart()
            )
            .ProcessAsynchronously();
        memoryStream.Position = 0;

        // Add metadata
        return new VideoStream { Stream = memoryStream };
    }

    private async Task<IStreamInfo> GetAudioStream(IVideo video, CancellationToken cancellationToken)
    {
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(video.Url, cancellationToken);
        return streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
    }
}