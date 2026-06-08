using System.Text;
using System.Xml.Linq;

namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterTests
{
    [Fact]
    public async Task StartAsync_WithoutMetadata_WritesEmptyRoot()
    {
        var output = new StringBuilder();
        using var writer = new XmlTvWriter(new StringWriter(output));

        await writer.StartAsync();
        await writer.CompleteAsync();

        var document = XDocument.Parse(output.ToString());
        Assert.Equal("tv", document.Root!.Name.LocalName);
        Assert.False(document.Root.HasAttributes);
        Assert.Empty(document.Root.Elements());
    }

    [Fact]
    public async Task StartAsync_WritesMetadataAttributes()
    {
        var output = new StringBuilder();
        var metadata = new XmlTvMetadata
        {
            Date = XmlTvDateTime.Parse("20260605"),
            SourceInfoUrl = "https://example.test/source",
            SourceInfoName = "Source",
            SourceDataUrl = "https://example.test/data",
            GeneratorInfoName = "Generator",
            GeneratorInfoUrl = "https://example.test/generator"
        };

        using var writer = new XmlTvWriter(new StringWriter(output));

        await writer.StartAsync(metadata);
        await writer.CompleteAsync();

        var root = XDocument.Parse(output.ToString()).Root!;
        Assert.Equal("20260605", (string?)root.Attribute("date"));
        Assert.Equal("https://example.test/source", (string?)root.Attribute("source-info-url"));
        Assert.Equal("Source", (string?)root.Attribute("source-info-name"));
        Assert.Equal("https://example.test/data", (string?)root.Attribute("source-data-url"));
        Assert.Equal("Generator", (string?)root.Attribute("generator-info-name"));
        Assert.Equal("https://example.test/generator", (string?)root.Attribute("generator-info-url"));
    }

    [Fact]
    public async Task WriteChannelAsync_WritesChannelChildrenInDtdOrder()
    {
        var output = new StringBuilder();
        var channel = new XmlTvChannel("channel-one", new XmlTvLocalizedText("Channel One", "en"));
        channel.DisplayNames.Add(new XmlTvLocalizedText("One"));
        channel.Icons.Add(new XmlTvIcon("channel.png", 100, 50));
        channel.Urls.Add(new XmlTvUrl("https://example.test/channel", "official"));

        using var writer = new XmlTvWriter(new StringWriter(output));

        await writer.StartAsync();
        await writer.WriteChannelAsync(channel);
        await writer.CompleteAsync();

        var channelElement = Assert.Single(XDocument.Parse(output.ToString()).Root!.Elements("channel"));
        Assert.Equal("channel-one", (string?)channelElement.Attribute("id"));
        Assert.Equal(
            new[] { "display-name", "display-name", "icon", "url" },
            channelElement.Elements().Select(element => element.Name.LocalName));
        Assert.Equal("en", (string?)channelElement.Elements("display-name").First().Attribute("lang"));
        Assert.Equal("Channel One", channelElement.Elements("display-name").First().Value);
        Assert.Equal("One", channelElement.Elements("display-name").Skip(1).Single().Value);

        var icon = Assert.Single(channelElement.Elements("icon"));
        Assert.Equal("channel.png", (string?)icon.Attribute("src"));
        Assert.Equal("100", (string?)icon.Attribute("width"));
        Assert.Equal("50", (string?)icon.Attribute("height"));

        var url = Assert.Single(channelElement.Elements("url"));
        Assert.Equal("official", (string?)url.Attribute("system"));
        Assert.Equal("https://example.test/channel", url.Value);
    }

    [Fact]
    public async Task WriteProgrammeAsync_WritesRequiredProgrammeAndAttributes()
    {
        var output = new StringBuilder();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "News")
        {
            Stop = XmlTvDateTime.Parse("20260605123000 +0000"),
            PdcStart = XmlTvDateTime.Parse("20260605115900 +0000"),
            VpsStart = XmlTvDateTime.Parse("20260605115800 +0000"),
            ShowView = "123",
            VideoPlus = "456",
            ClumpIndex = new XmlTvClumpIndex(1, 3)
        };
        programme.Titles.Add(new XmlTvLocalizedText("News EN", "en"));
        programme.SubTitles.Add(new XmlTvLocalizedText("Subtitle"));
        programme.Descriptions.Add(new XmlTvLocalizedText("Description"));

        using var writer = new XmlTvWriter(new StringWriter(output));

        await writer.StartAsync();
        await writer.WriteProgrammeAsync(programme);
        await writer.CompleteAsync();

        var programmeElement = Assert.Single(XDocument.Parse(output.ToString()).Root!.Elements("programme"));
        Assert.Equal("20260605120000 +0000", (string?)programmeElement.Attribute("start"));
        Assert.Equal("20260605123000 +0000", (string?)programmeElement.Attribute("stop"));
        Assert.Equal("20260605115900 +0000", (string?)programmeElement.Attribute("pdc-start"));
        Assert.Equal("20260605115800 +0000", (string?)programmeElement.Attribute("vps-start"));
        Assert.Equal("123", (string?)programmeElement.Attribute("showview"));
        Assert.Equal("456", (string?)programmeElement.Attribute("videoplus"));
        Assert.Equal("channel-one", (string?)programmeElement.Attribute("channel"));
        Assert.Equal("1/3", (string?)programmeElement.Attribute("clumpidx"));
        Assert.Equal(
            new[] { "title", "title", "sub-title", "desc" },
            programmeElement.Elements().Select(element => element.Name.LocalName));
        Assert.Equal("News", programmeElement.Elements("title").First().Value);
        Assert.Equal("en", (string?)programmeElement.Elements("title").Skip(1).Single().Attribute("lang"));
        Assert.Equal("News EN", programmeElement.Elements("title").Skip(1).Single().Value);
    }

    [Fact]
    public async Task WriteAsync_Document_WritesChannelsBeforeProgrammes()
    {
        var output = new StringBuilder();
        var document = new XmlTvDocument(new XmlTvMetadata { GeneratorInfoName = "Generator" });
        document.Programmes.Add(new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "News"));
        document.Channels.Add(new XmlTvChannel("channel-one", "Channel One"));

        await XmlTvWriter.WriteAsync(document, new StringWriter(output));

        var root = XDocument.Parse(output.ToString()).Root!;
        Assert.Equal("Generator", (string?)root.Attribute("generator-info-name"));
        Assert.Equal(new[] { "channel", "programme" }, root.Elements().Select(element => element.Name.LocalName));
    }

    [Fact]
    public async Task WriteAsync_WithOmitXmlDeclaration_DoesNotWriteXmlDeclaration()
    {
        var output = new StringBuilder();
        var options = new XmlTvWriterOptions { OmitXmlDeclaration = true };

        await XmlTvWriter.WriteAsync(new XmlTvDocument(), new StringWriter(output), options);

        Assert.StartsWith("<tv", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteAsync_Stream_UsesUtf8WithoutBom()
    {
        using var stream = new MemoryStream();

        await XmlTvWriter.WriteAsync(new XmlTvDocument(), stream);

        var bytes = stream.ToArray();
        Assert.False(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
        Assert.Equal('<', (char)bytes[0]);
    }

    [Fact]
    public async Task WriteAsync_Stream_LeavesStreamOpenByDefault()
    {
        using var stream = new MemoryStream();

        await XmlTvWriter.WriteAsync(new XmlTvDocument(), stream);

        stream.WriteByte(0);
    }

    [Fact]
    public async Task WriteAsync_TextWriter_LeavesTextWriterOpenByDefault()
    {
        var output = new StringBuilder();
        using var textWriter = new StringWriter(output);

        await XmlTvWriter.WriteAsync(new XmlTvDocument(), textWriter);

        await textWriter.WriteAsync("<!-- still open -->");
    }

    [Fact]
    public void Dispose_WithLeaveOpenFalse_ClosesStream()
    {
        var stream = new MemoryStream();
        using (var writer = new XmlTvWriter(stream))
        {
        }

        Assert.Throws<ObjectDisposedException>(() => stream.WriteByte(0));
    }

    [Fact]
    public void Dispose_WithLeaveOpenTrue_LeavesStreamOpen()
    {
        using var stream = new MemoryStream();
        using (var writer = new XmlTvWriter(stream, leaveOpen: true))
        {
        }

        stream.WriteByte(0);
    }

    [Fact]
    public async Task WriteChannelAsync_DuplicateChannelId_ThrowsXmlTvWriteException()
    {
        var output = new StringBuilder();
        using var writer = new XmlTvWriter(new StringWriter(output));

        await writer.StartAsync();
        await writer.WriteChannelAsync(new XmlTvChannel("channel-one", "One"));

        var exception = await Assert.ThrowsAsync<XmlTvWriteException>(() =>
            writer.WriteChannelAsync(new XmlTvChannel("channel-one", "One Again")));

        Assert.Contains("Duplicate channel id", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WriteChannelAsync_EmptyDisplayNames_ThrowsXmlTvWriteExceptionBeforeEmittingChannel()
    {
        var output = new StringBuilder();
        var channel = new XmlTvChannel("channel-one", "One");
        channel.DisplayNames.Clear();
        using var writer = new XmlTvWriter(new StringWriter(output));

        await writer.StartAsync();

        var exception = await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteChannelAsync(channel));

        Assert.Contains("channel-one", exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("<channel", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteProgrammeAsync_EmptyTitles_ThrowsXmlTvWriteExceptionBeforeEmittingProgramme()
    {
        var output = new StringBuilder();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "News");
        programme.Titles.Clear();
        using var writer = new XmlTvWriter(new StringWriter(output));

        await writer.StartAsync();

        var exception = await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteProgrammeAsync(programme));

        Assert.Contains("channel-one", exception.Message, StringComparison.Ordinal);
        Assert.Contains("20260605120000 +0000", exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("<programme", output.ToString(), StringComparison.Ordinal);
    }
}
