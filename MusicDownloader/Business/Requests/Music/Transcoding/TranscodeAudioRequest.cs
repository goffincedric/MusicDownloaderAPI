using FFMpegCore;
using FFMpegCore.Pipes;
using MediatR;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;

namespace MusicDownloader.Business.Requests.Music.Transcoding;

public class TranscodeAudioRequest : IRequest<MusicStream>
{
    public Task<Stream> AudioStreamTask { get; init; }
    public Task<Stream?> CoverArtTask { get; init; }
    public Task<TrackMetadata> TrackMetadataTask { get; init; }
    public IMetadataMapperStrategy MetadataMapperStrategy { get; init; }
}

public class TranscodeAudioRequestHandler : IRequestHandler<TranscodeAudioRequest, MusicStream>
{
    public async Task<MusicStream> Handle(TranscodeAudioRequest request, CancellationToken cancellationToken)
    {
        // Set filename
        var trackMetadata = await request.TrackMetadataTask;
        var fileName = $"{trackMetadata.Title.ToSafeFilename()}.{YoutubeConstants.Container}";

        // Map metadata to desired tag system
        var metadata = request.MetadataMapperStrategy.Execute(trackMetadata);

        // Pipe in audio stream and cover as video stream if available
        var transcodeBuilder = FFMpegArguments.FromPipeInput(new StreamPipeSource(await request.AudioStreamTask));
        var coverArt = await request.CoverArtTask;
        if (coverArt != null) transcodeBuilder = transcodeBuilder.AddPipeInput(new StreamPipeSource(coverArt));

        // Add metadata, configure FFMpeg and start transcoding asynchronously
        var memoryStream = new MemoryStream();
        await transcodeBuilder
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
        // Reset stream position
        memoryStream.Position = 0;

        return new MusicStream { Stream = memoryStream, FileName = fileName, Container = YoutubeConstants.Container };
    }
}