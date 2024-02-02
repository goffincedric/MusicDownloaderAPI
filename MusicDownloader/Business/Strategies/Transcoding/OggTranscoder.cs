using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Business.Strategies.Transcoding;

public class OggTranscoder() : TranscoderStrategy(true, true, ContainerConstants.Containers.Ogg)
{
    private readonly VorbisMetadataMapper _metadataMapper = new();

    public override async Task Execute(
        string audioUrl,
        Stream targetStream,
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

        // Add metadata, configure FFMpeg and start transcoding asynchronously
        await transcodeBuilder
            .AddMetaData(metadata)
            .OutputToPipe(
                new StreamPipeSink(targetStream),
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
    }
}
