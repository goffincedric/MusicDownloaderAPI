using MusicDownloader.Business.Models;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.MusicDownload._base;

public interface IMusicDownloadStrategy
{
    /// <summary>
    /// Executes the general flow that is followed when downloading music from a supported provider.
    /// </summary>
    /// <param name="url">Url linking to the music to download</param>
    /// <param name="metadataMapperStrategy">Desired tag system</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A wrapper containing streamed music, along with some file metadata</returns>
    Task<MusicStream> Execute(
        string url,
        // TODO: Change to desired output container and then chose mapper strategy depending on chosen container
        IMetadataMapperStrategy metadataMapperStrategy,
        CancellationToken cancellationToken = default
    );
}