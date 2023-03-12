using System.Net;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Controllers.Models;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Controllers._base;

[ApiController]
[ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.InternalServerError)]
public class ApiControllerBase : ControllerBase
{
    protected readonly ILogger Logger;

    public ApiControllerBase(ILogger logger)
    {
        Logger = logger;
    }
}