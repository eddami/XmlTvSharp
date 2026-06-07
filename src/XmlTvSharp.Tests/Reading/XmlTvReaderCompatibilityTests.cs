namespace XmlTvSharp.Tests.Reading;

public sealed class XmlTvReaderCompatibilityTests
{
    [Fact]
    public async Task ReadAsync_ParsesJellyfinLiveMarkerWhenProfileIsEnabled()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <live />
                             </programme>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
        };

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), options);

        var programme = Assert.Single(document.Programmes);
        Assert.True(programme.Extensions.Jellyfin?.IsLive);
    }

    [Fact]
    public async Task ReadAsync_DisallowsJellyfinLiveMarkerInStandardProfileByDefault()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <live />
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_DisallowsJellyfinLiveMarkerContentWhenProfileIsEnabled()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <live>unexpected</live>
                             </programme>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
        };

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml), options));
    }
}
