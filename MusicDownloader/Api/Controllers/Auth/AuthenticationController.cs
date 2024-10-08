﻿using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Api.Controllers._base;
using MusicDownloader.Api.Models.Read;
using MusicDownloader.Api.Models.Write;
using MusicDownloader.Business.Requests.User;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Api.Controllers.Auth;

[Route("auth")]
public class AuthenticationController : AnonymousApiController
{
    private readonly IMediator _mediator;

    public AuthenticationController(ILogger logger, IMediator mediator)
        : base(logger)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        // Validate
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Get user and generate JWT token
        var apiUser = await _mediator.Send(new GetApiUserRequest(request.ApiToken));
        var accessToken = await _mediator.Send(new GetJwtTokenForUserRequest(apiUser));
        return Ok(new AuthResponse { JwtToken = accessToken });
    }
}
