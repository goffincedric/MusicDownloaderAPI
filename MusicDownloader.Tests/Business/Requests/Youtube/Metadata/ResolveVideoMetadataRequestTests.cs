using FFMpegCore.Builders.MetaData;
using MusicDownloader.Business.Requests.Youtube.Metadata;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Tests.TestData.ResolveVideoMetadataRequest;
using YoutubeReExplode;

namespace MusicDownloader.Tests.Business.Requests.Youtube.Metadata;

public class ResolveVideoMetadataRequestTests
{
    private readonly YoutubeClient _youtubeClient;

    public ResolveVideoMetadataRequestTests()
    {
        _youtubeClient = new YoutubeClient(new HttpClient());
    }

    [Theory]
    [ClassData(typeof(ResolveVideoMetadataRequestTestData.Handle_MusicVideoUrl))]
    public async Task Handle_MusicVideoUrl_ReturnExpectedMetadata(
        string testCase, string url, MetaData expectedMetadata
    )
    {
        // Arrange
        var resolvedVideo = await _youtubeClient.Videos.GetAsync(url);
        var request = new ResolveVideoMetadataRequest
        {
            Video = resolvedVideo
        };
        var sut = new ResolveVideoMetadataRequestHandler();

        // Act
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedMetadata.Entries.ContainsKey(MetadataConstants.VorbisTags.Artist),
            result.Entries.ContainsKey(MetadataConstants.VorbisTags.Artist));
        if (result.Entries.TryGetValue(MetadataConstants.VorbisTags.Artist, out var artist))
            Assert.Equal(expectedMetadata.Entries[MetadataConstants.VorbisTags.Artist], artist);

        Assert.Equal(expectedMetadata.Entries.ContainsKey(MetadataConstants.VorbisTags.Title),
            result.Entries.ContainsKey(MetadataConstants.VorbisTags.Title));
        if (result.Entries.TryGetValue(MetadataConstants.VorbisTags.Title, out var title))
            Assert.Equal(expectedMetadata.Entries[MetadataConstants.VorbisTags.Title], title);

        Assert.Equal(expectedMetadata.Entries.ContainsKey(MetadataConstants.VorbisTags.Album),
            result.Entries.ContainsKey(MetadataConstants.VorbisTags.Album));
        if (result.Entries.TryGetValue(MetadataConstants.VorbisTags.Album, out var album))
            Assert.Equal(expectedMetadata.Entries[MetadataConstants.VorbisTags.Album], album);
    }
}