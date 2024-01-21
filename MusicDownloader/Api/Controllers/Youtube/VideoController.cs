using System.Net;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Api.Controllers._base;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Music.Download;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.MusicDownload;
using MusicDownloader.Business.Strategies.Transcoding;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Controllers.Youtube;

[Route("youtube/[controller]")]
public class VideoController : AuthenticatedAnonymousApiController
{
    private readonly IMediator _mediator;

    public VideoController(ILogger logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(TrackDetails), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetVideoMetadata([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Get result
        var video = await _mediator.Send(new GetVideoDetailsRequest { Url = url });

        // Return
        return Ok(video);
    }

    [HttpGet("download")]
    [Produces(
        $"audio/{ContainerConstants.Containers.Ogg}",
        $"audio/{ContainerConstants.Containers.Mp3}",
        $"audio/{ContainerConstants.Containers.Opus}",
        $"audio/{ContainerConstants.Containers.Aac}"
    )]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DownloadVideo([FromQuery(Name = "url")] string url, [FromQuery(Name = "container")] string? container)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();
        // Check if container is supported youtube container
        if (!string.IsNullOrWhiteSpace(container) && !YoutubeConstants.SupportedContainers.Any(supportedContainer => supportedContainer.Name.Equals(container)))
            throw new MusicDownloaderException($"Unsupported container: {container}", ErrorCodes.UnsupportedAudioContainer, HttpStatusCode.BadRequest);
        var transcodedContainer = string.IsNullOrWhiteSpace(container) ? ContainerConstants.Containers.Default : container;
        
        // Download audio using youtube strategy
        var videoStream = await _mediator.Send(new DownloadAudioRequest
        {
            Url = url,
            Container = transcodedContainer,
            DownloadStrategy = new YoutubeDownloadStrategy(_mediator),
        });

        // Set response headers to correctly reflect stream contents and return stream as file
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        Response.Headers.Add("Content-Disposition", new ContentDispositionHeaderValue("attachment")
        {
            FileName = videoStream.FileName,
            FileNameStar = videoStream.FileName
        }.ToString());
        return File(videoStream.Stream, $"audio/{videoStream.Container}", true);
    }
}