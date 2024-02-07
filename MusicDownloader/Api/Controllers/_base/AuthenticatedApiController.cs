using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Controllers._base;

[Authorize]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
public class AuthenticatedAnonymousApiController : AnonymousApiController
{
    public AuthenticatedAnonymousApiController(ILogger logger)
        : base(logger) { }
}
