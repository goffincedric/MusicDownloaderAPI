using FFMpegCore;
using FFMpegCore.Pipes;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Strategies.Transcoding;

public class Mp3Transcoder : TranscoderStrategy
{
    private readonly Container _container;
    private readonly IMetadataMapperStrategy _metadataMapper;
    
    public Mp3Transcoder(): base(true, true)
    {
        _container = YoutubeConstants.SupportedContainers.First(container => container.Name.Equals(ContainerConstants.Containers.Mp3));
        _metadataMapper = new ID3MetadataMapper();
    }

    public override async Task<MusicStream> Execute(string audioUrl, CancellationToken cancellationToken)
    {
        // Map metadata to vorbis tag system and set filename
        var trackMetadata = await TrackMetadataTask;
        var metadata = _metadataMapper.Execute(trackMetadata);
        var fileName = $"{trackMetadata.Title.ToSafeFilename()}.{_container.Name}";

        // Pipe in audio stream and cover art as video stream if available
        var transcodeBuilder = FFMpegArguments.FromUrlInput(new Uri(audioUrl));
        var coverArt = await CoverArtStreamTask; // TODO: Fix cover art for mp3 not working
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

        return new MusicStream
        {
            Stream = memoryStream, FileName = fileName, Container = _container.Name, ContentType =
                $"audio/{_container.Name}"
        };
    }
}