using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Music.Download;
using MusicDownloader.Business.Requests.Music.Metadata;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Business.Strategies.MetadataMapping;
using MusicDownloader.Business.Strategies.MusicDownload;
using MusicDownloader.Controllers._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Controllers.Youtube;

[Route("youtube/[controller]")]
public class VideoController : ApiControllerBase
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
    [Produces($"audio/{YoutubeConstants.Container}")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DownloadVideo([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Download audio using youtube strategy
        var videoStream = await _mediator.Send(new DownloadAudioRequest
        {
            Url = url,
            DownloadStrategy = new YoutubeDownloadStrategy(_mediator),
            MetadataMapperStrategy = new VorbisMetadataMapper()
        });

        // Set response headers to correctly reflect stream contents and return stream as file
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{videoStream.FileName}\"");
        return File(videoStream.Stream, $"audio/{videoStream.Container}", true);
    }
}