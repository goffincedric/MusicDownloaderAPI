using System.Net;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Api.Models;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Controllers._base;

[ApiController]
[ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.InternalServerError)]
public class AnonymousApiController : ControllerBase
{
    protected readonly ILogger Logger;

    public AnonymousApiController(ILogger logger)
    {
        Logger = logger;
    }
}
