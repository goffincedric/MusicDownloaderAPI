using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Business.Strategies.Transcoding;

public class OggTranscoder() : TranscoderStrategy(true, true)
{
    private const string TargetContainer = ContainerConstants.Containers.Ogg;
    private readonly VorbisMetadataMapper _metadataMapper = new();

    public override async Task<DownloadStreamInfo> Execute(
        string audioUrl,
        CancellationToken cancellationToken
    )
    {
        // Map metadata to vorbis tag system and set filename
        var metadata = _metadataMapper.Execute(TrackMetadata);

        // Pipe in audio stream and cover art as video stream if available
        var transcodeBuilder = FFMpegArguments.FromUrlInput(new Uri(audioUrl));
        var coverArt = await CoverArtStreamTask;
        if (coverArt != null)
            transcodeBuilder = transcodeBuilder.AddPipeInput(new StreamPipeSource(coverArt));

        // Get file extension from download url
        // var foundMimeType = QueryHelpers.ParseQuery(new Uri(audioUrl).Query).TryGetValue("mime", out var mimeType);
        // if (!foundMimeType)
        //     throw new MusicDownloaderException("Couldn't parse mimetype from download url",
        //         ErrorCodes.Youtube.MimeTypeNotInDownloadUrl, HttpStatusCode.InternalServerError);
        // var extension = MimeTypeUtils.MapMimeTypeToExtension(mimeType.ToString());

        // Add metadata, configure FFMpeg and start transcoding asynchronously
        var memoryStream = new MemoryStream();
        await transcodeBuilder
            .AddMetaData(metadata)
            .OutputToPipe(
                new StreamPipeSink(memoryStream),
                options =>
                    options
                        .ForceFormat(TargetContainer)
                        .WithAudioCodec(AudioCodec.LibVorbis)
                        .WithAudioBitrate(YoutubeConstants.AudioQuality)
                        .WithAudioSamplingRate(YoutubeConstants.SamplingRate)
                        .WithVideoCodec(VideoCodec.LibTheora) // Codec needed for cover art video
                        .WithFramerate(YoutubeConstants.CoverFramerate)
                        .WithCustomArgument("-q:v 10") // Highest quality video
                        .UsingMultithreading(true)
                        .WithFastStart()
            )
            .ProcessAsynchronously();
        // Reset stream position and return
        memoryStream.Position = 0;

        // Construct downloaded stream info and return
        return new DownloadStreamInfo(memoryStream, TargetContainer);
    }
}
