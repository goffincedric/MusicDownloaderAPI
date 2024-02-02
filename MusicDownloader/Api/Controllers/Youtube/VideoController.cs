using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Api.Controllers._base;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Music.Streaming;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Business.Strategies.MusicStream;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Controllers.Youtube;

[Route("youtube/[controller]")]
public class VideoController(ILogger logger, IMediator mediator)
    : AuthenticatedAnonymousApiController(logger)
{
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(TrackDetails), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetVideoMetadata([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest();

        // Get result
        var video = await mediator.Send(new GetVideoDetailsRequest(url));

        // Return
        return Ok(video);
    }

    [HttpGet("download")]
    [Produces(
        $"audio/{ContainerConstants.Containers.Ogg}",
        $"audio/{ContainerConstants.Containers.Mp3}",
        $"audio/{ContainerConstants.Containers.Opus}",
        $"audio/{ContainerConstants.Containers.Aac}",
        $"audio/{ContainerConstants.Containers.Webm}"
    )]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    // TODO: Move to controller query models
    public async Task DownloadVideo(
        [FromQuery(Name = "url")] string url,
        [FromQuery(Name = "container")] string? container
    )
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url))
        {
            BadRequest();
            return;
        }

        // Check if container is supported youtube container
        if (
            !string.IsNullOrWhiteSpace(container)
            && !YoutubeConstants.SupportedContainers.Any(
                supportedContainer => supportedContainer.Name.Equals(container)
            )
        )
            throw new MusicDownloaderException(
                $"Unsupported container: {container}",
                ErrorCodes.UnsupportedAudioContainer,
                HttpStatusCode.BadRequest
            );

        // Stream audio using youtube strategy
        await mediator.Send(
            new StreamAudioRequest(url, container, new YoutubeStreamStrategy(mediator), Response)
        );
    }
}
