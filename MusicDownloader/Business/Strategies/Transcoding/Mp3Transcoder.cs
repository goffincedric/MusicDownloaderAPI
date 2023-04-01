using FFMpegCore;
using FFMpegCore.Pipes;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;

namespace MusicDownloader.Business.Strategies.Transcoding;

public class Mp3Transcoder : TranscoderStrategy
{
    public Mp3Transcoder() : base(
        YoutubeConstants.SupportedContainers.First(
            container => container.Name.Equals(ContainerConstants.Containers.Mp3)),
        new ID3MetadataMapper()
    )
    {
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
        var coverArt = await coverArtStreamTask; // TODO: Fix cover art for mp3 not working
        if (coverArt != null)
            transcodeBuilder = transcodeBuilder.AddPipeInput(new StreamPipeSource(coverArt));

        // Add metadata, configure FFMpeg and start transcoding asynchronously
        var memoryStream = new MemoryStream();
        await transcodeBuilder
            .AddMetaData(metadata)
            .OutputToPipe(new StreamPipeSink(memoryStream),
                options => options
                    // Doesn't work currently: https://github.com/rosenbjerg/FFMpegCore/issues/429
                    // .WithCustomArgument("-c copy -map 0 -map 1")
                    .ForceFormat("mp3")
                    .WithAudioBitrate(YoutubeConstants.AudioQuality)
                    .WithAudioSamplingRate(YoutubeConstants.SamplingRate)
                    .UsingMultithreading(true)
                    .WithFastStart()
            )
            .ProcessAsynchronously();
        // Reset stream position
        memoryStream.Position = 0;

        return new MusicStream { Stream = memoryStream, FileName = fileName, Container = Container.Name };
    }
}