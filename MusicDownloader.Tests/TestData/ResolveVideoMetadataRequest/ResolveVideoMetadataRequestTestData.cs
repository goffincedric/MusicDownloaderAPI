using System.Collections;
using FFMpegCore.Builders.MetaData;
using MusicDownloader.Shared.Constants;

namespace MusicDownloader.Tests.TestData.ResolveVideoMetadataRequest;

public class ResolveVideoMetadataRequestTestData
{
    public class Handle_MusicVideoUrl : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                "ROUND_BRACKETS TITLE_WITH_TOPIC TITLE_WITH_ARTIST_NAME_DASH",
                "https://www.youtube.com/watch?v=Nucv1NrK3V0",
                new MetaData
                {
                    Entries =
                    {
                        { MetadataConstants.VorbisTags.Artist, "Quiet Machine" },
                        { MetadataConstants.VorbisTags.Title, "The Trial" },
                    }
                }
            };
            yield return new object[]
            {
                "ROUND_BRACKETS TITLE_WITH_ARTIST_NAME_DASH",
                "https://www.youtube.com/watch?v=dI3xkL7qUAc",
                new MetaData
                {
                    Entries =
                    {
                        { MetadataConstants.VorbisTags.Artist, "Doja Cat" },
                        { MetadataConstants.VorbisTags.Title, "Need to Know" },
                    }
                }
            };
            yield return new object[]
            {
                "METADATA SONG ARTIST ALBUM",
                "https://www.youtube.com/watch?v=9bZkp7q19f0",
                new MetaData
                {
                    Entries =
                    {
                        { MetadataConstants.VorbisTags.Artist, "PSY" },
                        { MetadataConstants.VorbisTags.Title, "강남스타일(Gangnam Style)" },
                        { MetadataConstants.VorbisTags.Album, "PSY SIX RULES Pt.1" },
                    }
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
