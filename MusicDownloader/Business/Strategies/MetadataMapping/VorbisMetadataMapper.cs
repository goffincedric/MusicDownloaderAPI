using FFMpegCore.Builders.MetaData;
using MusicDownloader.Business.Strategies.MetadataMapping._base;
using MusicDownloader.Pocos.Youtube;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Business.Strategies.MetadataMapping;

public class VorbisMetadataMapper : IMetadataMapperStrategy
{
    public IReadOnlyMetaData Execute(TrackMetadata trackMetadata)
    {
        var metadata = new MetaData();
        // Map metadata
        metadata.Entries.Add(MetadataConstants.VorbisTags.Artist, trackMetadata.Author);
        metadata.Entries.Add(MetadataConstants.VorbisTags.Title, trackMetadata.Title);
        if (trackMetadata.Album != null)
            metadata.Entries.Add(MetadataConstants.VorbisTags.Album, trackMetadata.Album);
        if (trackMetadata.TrackNumber.HasValue)
            metadata.Entries.Add(
                MetadataConstants.VorbisTags.TrackNumber,
                trackMetadata.TrackNumber.Value.ToString()
            );
        // Return metadata
        return metadata;
    }
}
