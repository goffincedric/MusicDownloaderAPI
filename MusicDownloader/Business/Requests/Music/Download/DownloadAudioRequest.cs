﻿using MediatR;
using MusicDownloader.Business.Requests.Music.Transcoding;
using MusicDownloader.Business.Strategies.MusicDownload._base;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Requests.Music.Download;

public class DownloadAudioRequest : IRequest<MusicStream>
{
    public string Url { get; init; }
    public string? Container { get; init; }
    public IMusicDownloadStrategy DownloadStrategy { get; init; }
}

public class DownloadAudioRequestHandler(IMediator mediator) : IRequestHandler<DownloadAudioRequest, MusicStream>
{
    public async Task<MusicStream> Handle(DownloadAudioRequest request, CancellationToken cancellationToken)
    {
        // Resolve transcoder strategy from container
        var transcodingStrategy = await mediator.Send(new ResolveContainerTranscoderRequest
            { Container = request.Container }, cancellationToken);
        // Download url and transcode using resolved strategy
        return await request.DownloadStrategy.Execute(request.Url, transcodingStrategy, cancellationToken);
    }
}