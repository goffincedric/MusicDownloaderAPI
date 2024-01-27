using System.Net;
using FFMpegCore;
using FFMpegCore.Pipes;
using Microsoft.AspNetCore.WebUtilities;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;
using MusicDownloader.Shared.Utils;

namespace MusicDownloader.Business.Strategies.Transcoding;

public class Mp3Transcoder() : TranscoderStrategy(true, true)
{
    private const string TargetContainer = ContainerConstants.Containers.Mp3;
    private readonly IMetadataMapperStrategy _metadataMapper = new ID3MetadataMapper();

    public override async Task<DownloadStreamInfo> Execute(string audioUrl, CancellationToken cancellationToken)
    {
        // Map metadata to vorbis tag system and set filename
        var metadata = _metadataMapper.Execute(TrackMetadata);

        // Pipe in audio stream and cover art as video stream if available
        var transcodeBuilder = FFMpegArguments.FromUrlInput(new Uri(audioUrl));
        var coverArt = await CoverArtStreamTask; // TODO: Fix cover art for mp3 not working
        if (coverArt != null)
            transcodeBuilder = transcodeBuilder.AddPipeInput(new StreamPipeSource(coverArt));
        
        // Get file extension from download url
        var foundMimeType = QueryHelpers.ParseQuery(new Uri(audioUrl).Query).TryGetValue("mime", out var mimeType);
        if (!foundMimeType)
            throw new MusicDownloaderException("Couldn't parse mimetype from download url",
                ErrorCodes.Youtube.MimeTypeNotInDownloadUrl, HttpStatusCode.InternalServerError);
        var extension = MimeTypeUtils.MapMimeTypeToExtension(mimeType.ToString());

        // Add metadata, configure FFMpeg and start transcoding asynchronously
        var memoryStream = new MemoryStream();
        await transcodeBuilder
            .AddMetaData(metadata)
            .OutputToPipe(new StreamPipeSink(memoryStream),
                options => options
                    // Doesn't work currently: https://github.com/rosenbjerg/FFMpegCore/issues/429
                    // .WithCustomArgument("-c copy -map 0 -map 1")
                    .ForceFormat(TargetContainer)
                    .WithAudioBitrate(YoutubeConstants.AudioQuality)
                    .WithAudioSamplingRate(YoutubeConstants.SamplingRate)
                    .UsingMultithreading(true)
                    .WithFastStart()
            )
            .ProcessAsynchronously();
        // Reset stream position
        memoryStream.Position = 0;

        // Construct downloaded stream info and return
        return new DownloadStreamInfo(memoryStream, TargetContainer);
    }
}