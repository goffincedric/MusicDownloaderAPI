using MediatR;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Business.Strategies.MusicDownload._base;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Requests.Music.Download;

public class DownloadAudioRequest : IRequest<MusicStream>
{
    public string Url { get; init; }
    public IMetadataMapperStrategy MetadataMapperStrategy { get; init; }
    public IMusicDownloadStrategy DownloadStrategy { get; init; }
}

public class DownloadAudioRequestHandler : IRequestHandler<DownloadAudioRequest, MusicStream>
{
    public Task<MusicStream> Handle(DownloadAudioRequest request, CancellationToken cancellationToken)
    {
        return request.DownloadStrategy.Execute(request.Url, request.MetadataMapperStrategy, cancellationToken);
    }
}