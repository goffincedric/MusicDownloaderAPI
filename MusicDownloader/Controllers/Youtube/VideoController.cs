using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Business.Requests.Youtube;
using MusicDownloader.Business.Requests.Youtube.Playlist;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Shared.Constants;
using YoutubeReExplode.Playlists;
using YoutubeReExplode.Videos;

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

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Video), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetVideoMetadata([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Get result
        var request = new GetVideoDetailsRequest { Url = url };
        var result = await _mediator.Send(request);

        // Return
        return Ok(result);
    }

    [HttpGet("download")]
    [Produces($"audio/{YoutubeConstants.Container}")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DownloadVideo([FromQuery(Name = "url")] string url)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(url)) return BadRequest();

        // Get videoDetails
        var videoDetails = await _mediator.Send(new GetVideoDetailsRequest { Url = url });
        // Validate it is not a livestream

        // Get playlist details
        IPlaylist? playlistDetails = null;
        IReadOnlyList<PlaylistVideo>? playlistVideos = null;
        try
        {
            var playlistDetailsTask = _mediator.Send(new GetPlaylistDetailsRequest { Url = url });
            var playlistVideosTask = _mediator.Send(new GetPlaylistVideosRequest { Url = url });
            await Task.WhenAny(playlistDetailsTask, playlistVideosTask);
            playlistDetails = playlistDetailsTask.Result;
            playlistVideos = playlistVideosTask.Result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        // Download audio
        var request = new DownloadAudioRequest
        {
            Video = videoDetails, Playlist = playlistDetails, PlaylistVideos = playlistVideos
        };
        var videoStream = await _mediator.Send(request);

        // Set filename in header and return stream
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        Response.Headers.Add("Content-Disposition",
            $"attachment; filename=\"{videoDetails.Title}.{YoutubeConstants.Container}\"");
        return File(videoStream.Stream, $"audio/{YoutubeConstants.Container}", true);
    }
}