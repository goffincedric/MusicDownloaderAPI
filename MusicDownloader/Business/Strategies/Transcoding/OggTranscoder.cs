using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;

namespace MusicDownloader.Business.Strategies.Transcoding;

public class OggTranscoder : TranscoderStrategy
{
    private Codec AudioCodec { get; }
    private Codec VideoCodec { get; }

    public OggTranscoder() : base(
        YoutubeConstants.SupportedContainers.First(container =>
            container.Name.Equals(ContainerConstants.Containers.Ogg)),
        new VorbisMetadataMapper()
    )
    {
        AudioCodec = FFMpegCore.Enums.AudioCodec.LibVorbis;
        VideoCodec = FFMpegCore.Enums.VideoCodec.LibTheora;
    }

    public override async Task<MusicStream> Execute(string audioUrl, Task<Stream?> coverArtStreamTask,
        Task<TrackMetadata> trackMetadataTask)
    {
        // Map metadata to vorbis tag system and set filename
        var trackMetadata = await trackMetadataTask;
        var metadata = MetadataMapper.Execute(trackMetadata);
        var fileName = $"{trackMetadata.Title.ToSafeFilename()}.{Container.Name}";

        // Pipe in audio stream and cover art as video stream if available
        var transcodeBuilder = FFMpegArguments.FromUrlInput(new Uri(audioUrl));
        var coverArt = await coverArtStreamTask;
        if (coverArt != null) transcodeBuilder = transcodeBuilder.AddPipeInput(new StreamPipeSource(coverArt));

        // Add metadata, configure FFMpeg and start transcoding asynchronously
        var memoryStream = new MemoryStream();
        await transcodeBuilder
            .AddMetaData(metadata)
            .OutputToPipe(new StreamPipeSink(memoryStream),
                options => options
                    .ForceFormat(Container.Name)
                    .WithAudioCodec(AudioCodec)
                    .WithAudioBitrate(YoutubeConstants.AudioQuality)
                    .WithAudioSamplingRate(YoutubeConstants.SamplingRate)
                    .WithVideoCodec(VideoCodec) // Codec needed for cover art video
                    .WithFramerate(YoutubeConstants.CoverFramerate)
                    .WithCustomArgument("-q:v 10") // Highest quality video
                    .UsingMultithreading(true)
                    .WithFastStart()
            )
            .ProcessAsynchronously();
        // Reset stream position
        memoryStream.Position = 0;

        return new MusicStream { Stream = memoryStream, FileName = fileName, Container = Container.Name };
    }
}