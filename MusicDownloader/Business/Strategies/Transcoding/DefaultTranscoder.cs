using System.Net;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Business.Strategies.Transcoding._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;
using MusicDownloader.Shared.Extensions;
using MusicDownloader.Shared.Utils;

namespace MusicDownloader.Business.Strategies.Transcoding;

public class DefaultTranscoder(IMediator mediator) : TranscoderStrategy(true, false)
{
    public override async Task<MusicStream> Execute(string audioUrl, CancellationToken cancellationToken)
    {
        // TODO: Remove code below
        // Get mime type and extension
        var foundMimeType = QueryHelpers.ParseQuery(new Uri(audioUrl).Query).TryGetValue("mime", out var mimeType);
        if (!foundMimeType)
            throw new MusicDownloaderException("Couldn't parse mimetype from download url",
                ErrorCodes.Youtube.MimeTypeNotInDownloadUrl, HttpStatusCode.InternalServerError);
        var extension = MimeTypeUtils.MapMimeTypeToExtension(mimeType.ToString());

        // Construct filename
        var trackMetadata = await TrackMetadataTask;
        var fileName = $"{trackMetadata.Title.ToSafeFilename()}.{extension}";

        // Get stream and return
        // TODO: Get stream via reexplode
        var stream = await mediator.Send(new GetAudioStreamRequest{Url = audioUrl}, cancellationToken);
        return new MusicStream
        {
            Stream = stream, FileName = fileName, Container = extension, ContentType = mimeType.ToString()
        };
    }
}