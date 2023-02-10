using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Business.Requests.Youtube;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Shared.Constants;
using YoutubeExplode.Playlists;

namespace MusicDownloader.Controllers.Youtube;

[ApiController]
[Route("youtube/[controller]")]
public class VideoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VideoController> _logger;

    public VideoController(ILogger<VideoController> logger, IMediator mediator)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("download")]
    [Produces("audio/ogg")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DownloadVideo([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Get playlist details
        IPlaylist? playlistDetails = null;
        IReadOnlyList<PlaylistVideo>? playlistVideos = null;
        try
        {
            playlistDetails = await _mediator.Send(new GetPlaylistMetadataRequest { Url = url });
            playlistVideos = await _mediator.Send(new GetPlaylistVideosRequest { Url = url });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        // Get videoDetails
        var videoDetails = await _mediator.Send(new GetVideoDetailsRequest { Url = url });

        // Download audio
        var request = new DownloadAudioRequest
            { Video = videoDetails, Playlist = playlistDetails, PlaylistVideos = playlistVideos };
        var result = await _mediator.Send(request);

        // Return
        return File(result.Stream, $"audio/{YoutubeConstants.Container}", true);
    }
}