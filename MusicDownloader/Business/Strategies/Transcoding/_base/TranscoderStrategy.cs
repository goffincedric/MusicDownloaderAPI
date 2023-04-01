using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Pocos.Youtube;
using YoutubeReExplode.Videos.Streams;

namespace MusicDownloader.Business.Strategies.Transcoding._base;

public abstract class TranscoderStrategy : ITranscoderStrategy
{
    protected Container Container { get; }
    protected IMetadataMapperStrategy MetadataMapper { get; }

    protected TranscoderStrategy(Container container, IMetadataMapperStrategy metadataMapper)
    {
        Container = container;
        MetadataMapper = metadataMapper;
    }

    public abstract Task<MusicStream> Execute(string audioUrl, Task<Stream?> coverArtStreamTask,
        Task<TrackMetadata> trackMetadataTask);
}