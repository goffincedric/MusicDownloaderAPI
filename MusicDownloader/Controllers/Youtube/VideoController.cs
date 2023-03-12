using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Business.Requests.Youtube.Metadata;
using MusicDownloader.Business.Requests.Youtube.Playlist;
using MusicDownloader.Business.Requests.Youtube.Video;
using MusicDownloader.Controllers._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Extensions;
using YoutubeReExplode.Playlists;
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
        catch (Exception)
        {
            Logger.Information("Couldn't resolve playlist info from url.");
        }

        // Download audio
        var request = new DownloadAudioRequest
        {
            Video = videoDetails, Playlist = playlistDetails, PlaylistVideos = playlistVideos
        };
        var videoStream = await _mediator.Send(request);

        // Set filename in header and return stream
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        // TODO: Move file naming logic to downloadAudioRequest. Wrap stream in class that contains stream + filename + extra details
        Response.Headers.Add("Content-Disposition",
            $"attachment; filename=\"{videoDetails.Title.ToSafeFilename()}.{YoutubeConstants.Container}\"");
        return File(videoStream.Stream, $"audio/{YoutubeConstants.Container}", true);
    }
}