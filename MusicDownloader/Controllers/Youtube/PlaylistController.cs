using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Youtube.Playlist;
using MusicDownloader.Controllers._base;
using MusicDownloader.Pocos.Youtube;
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
    [ProducesResponseType(typeof(PlaylistDetailsExtended), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPlaylistMetadata([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Get result
        var result = await _mediator.Send(new GetPlaylistDetailsExtendedRequest { Url = url });

        // Return
        return Ok(result);
    }
}