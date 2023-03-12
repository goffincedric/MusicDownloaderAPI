using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Business.Requests.Youtube;
using MusicDownloader.Business.Requests.Youtube.Playlist;
using MusicDownloader.Controllers._base;
using YoutubeReExplode.Playlists;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Controllers.Youtube;

[Route("youtube/[controller]")]
public class PlaylistController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public PlaylistController(ILogger logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Playlist), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPlaylistMetadata([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Get result
        var request = new GetPlaylistDetailsRequest { Url = url };
        var result = await _mediator.Send(request);

        // Return
        return Ok(result);
    }

    [HttpGet("videos")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyList<PlaylistVideo>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPlaylistVideos([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Get result
        var request = new GetPlaylistVideosRequest { Url = url };
        var result = await _mediator.Send(request);

        // Return
        return Ok(result);
    }
}