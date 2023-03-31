using FFMpegCore.Builders.MetaData;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Business.Strategies.MetadataMapping;

public class ID3MetadataMapper : IMetadataMapperStrategy
{
    public IReadOnlyMetaData Execute(TrackMetadata trackMetadata)
    {
        var metadata = new MetaData();
        // Map metadata
        metadata.Entries.Add(MetadataConstants.ID3v2_4Tags.Artist, trackMetadata.Author);
        metadata.Entries.Add(MetadataConstants.ID3v2_4Tags.Title, trackMetadata.Title);
        if (trackMetadata.Album != null)
            metadata.Entries.Add(MetadataConstants.ID3v2_4Tags.Album, trackMetadata.Album);
        if (trackMetadata.TrackNumber.HasValue)
            metadata.Entries.Add(MetadataConstants.ID3v2_4Tags.TrackNumber, trackMetadata.TrackNumber.Value.ToString());
        // Return metadata
        return metadata;
    }
}