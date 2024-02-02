using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Api.Controllers._base;
using MusicDownloader.Business.Models;
using MusicDownloader.Business.Requests.Youtube.Playlist;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Controllers.Youtube;

[Route("youtube/[controller]")]
public class PlaylistController(ILogger logger, IMediator mediator)
    : AuthenticatedAnonymousApiController(logger)
{
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PlaylistDetailsExtended), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPlaylistMetadata([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest();

        // Get result
        var result = await mediator.Send(new GetPlaylistDetailsExtendedRequest(url));

        // Return
        return Ok(result);
    }
}
