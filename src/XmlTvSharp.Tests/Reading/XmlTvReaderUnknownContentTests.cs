namespace XmlTvSharp.Tests.Reading;

public sealed class XmlTvReaderUnknownContentTests
{
    [Fact]
    public async Task ReadAsync_IgnoredUnknownNestedSubtreeDoesNotLeakRecognizedDescendants()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <unknown><title>Leaked</title></unknown>
                             </programme>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            UnknownElementHandling = XmlTvUnknownContentHandling.Ignore
        };

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), options);

        var programme = Assert.Single(document.Programmes);
        Assert.Equal(new XmlTvLocalizedText("News"), Assert.Single(programme.Titles));
    }

    [Fact]
    public async Task ReadAsync_IgnoresUnknownXExtensionByDefault()
    {
        const string xml = """
                           <tv>
                             <x-provider>
                               <channel id="ignored"><display-name>Ignored</display-name></channel>
                             </x-provider>
                             <channel id="one"><display-name>One</display-name></channel>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        Assert.Equal("one", Assert.Single(document.Channels).Id);
    }

    [Fact]
    public async Task ReadAsync_DisallowsUnknownStandardElementByDefault()
    {
        const string xml = """
                           <tv>
                             <unknown />
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_DisallowsUnknownStandardAttributeByDefault()
    {
        const string xml = """
                           <tv>
                             <channel id="one" provider-id="external"><display-name>One</display-name></channel>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_IgnoresUnknownStandardAttributeWhenConfigured()
    {
        const string xml = """
                           <tv>
                             <channel id="one" provider-id="external"><display-name>One</display-name></channel>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            UnknownAttributeHandling = XmlTvUnknownContentHandling.Ignore
        };

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), options);

        Assert.Equal("one", Assert.Single(document.Channels).Id);
    }

    [Fact]
    public async Task ReadAsync_IgnoresUnknownXExtensionAttributeByDefault()
    {
        const string xml = """
                           <tv>
                             <channel id="one" x-provider-id="external"><display-name>One</display-name></channel>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        Assert.Equal("one", Assert.Single(document.Channels).Id);
    }

    [Fact]
    public async Task ReadAsync_DisallowsUnknownXExtensionAttributeWhenConfigured()
    {
        const string xml = """
                           <tv>
                             <channel id="one" x-provider-id="external"><display-name>One</display-name></channel>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            XExtensionHandling = XmlTvUnknownContentHandling.Disallow
        };

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml), options));
    }
}
