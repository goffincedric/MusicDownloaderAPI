using FFMpegCore.Builders.MetaData;
using MusicDownloader.Pocos.Youtube;

namespace MusicDownloader.Business.Strategies.MetadataMapping._base;

public interface IMetadataMapperStrategy
{
    /// <summary>
    /// Executes mappings to different tag systems, depending on the chosen strategy.
    /// </summary>
    /// <param name="trackMetadata">Object that contains various details about a specific track, such as Artists, Author, Song names, ...</param>
    /// <returns>A collection containing the filled out metadata tags</returns>
    internal IReadOnlyMetaData Execute(TrackMetadata trackMetadata);
}