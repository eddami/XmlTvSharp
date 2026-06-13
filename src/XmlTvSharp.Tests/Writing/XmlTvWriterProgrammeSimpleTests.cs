using System.Xml.Linq;

namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterProgrammeSimpleTests
{
    [Fact]
    public async Task WriteProgrammeAsync_WritesSimpleProgrammeChildrenInDtdOrder()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Date = XmlTvDateTime.Parse("20260605"),
            Language = new XmlTvLocalizedText("English", "en"),
            OriginalLanguage = new XmlTvLocalizedText("French", "fr"),
            Length = XmlTvDuration.FromMinutes(90),
            Video = new XmlTvVideo(true, false, "16:9", "HDTV"),
            Audio = new XmlTvAudio(true, "stereo"),
            PreviouslyShown = new XmlTvPreviouslyShown(XmlTvDateTime.Parse("20260501120000 +0000"), "old-channel"),
            Premiere = new XmlTvLocalizedText("", "en"),
            LastChance = new XmlTvLocalizedText("Final showing"),
            IsNew = true
        };
        programme.Categories.Add(new XmlTvLocalizedText("News", "en"));
        programme.Keywords.Add(new XmlTvLocalizedText("Politics"));
        programme.Icons.Add(new XmlTvIcon("programme.png", 200, 100));
        programme.Urls.Add(new XmlTvUrl("https://example.test/programme", "official"));
        programme.Countries.Add(new XmlTvLocalizedText("GB"));
        programme.EpisodeNumbers.Add(new XmlTvEpisodeNumber("1.2.", "xmltv_ns"));

        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        var programmeElement = Assert.Single(XDocument.Parse(output.ToString()).Root!.Elements("programme"));
        Assert.Equal(
            new[]
            {
                "title",
                "date",
                "category",
                "keyword",
                "language",
                "orig-language",
                "length",
                "icon",
                "url",
                "country",
                "episode-num",
                "video",
                "audio",
                "previously-shown",
                "premiere",
                "last-chance",
                "new"
            },
            programmeElement.Elements().Select(element => element.Name.LocalName));

        Assert.Equal("20260605", programmeElement.Element("date")!.Value);
        Assert.Equal("en", (string?)programmeElement.Element("category")!.Attribute("lang"));
        Assert.Equal("News", programmeElement.Element("category")!.Value);
        Assert.Equal("Politics", programmeElement.Element("keyword")!.Value);
        Assert.Equal("English", programmeElement.Element("language")!.Value);
        Assert.Equal("en", (string?)programmeElement.Element("language")!.Attribute("lang"));
        Assert.Equal("French", programmeElement.Element("orig-language")!.Value);
        Assert.Equal("fr", (string?)programmeElement.Element("orig-language")!.Attribute("lang"));

        var length = programmeElement.Element("length")!;
        Assert.Equal("minutes", (string?)length.Attribute("units"));
        Assert.Equal("90", length.Value);

        var icon = programmeElement.Element("icon")!;
        Assert.Equal("programme.png", (string?)icon.Attribute("src"));
        Assert.Equal("200", (string?)icon.Attribute("width"));
        Assert.Equal("100", (string?)icon.Attribute("height"));

        var url = programmeElement.Element("url")!;
        Assert.Equal("official", (string?)url.Attribute("system"));
        Assert.Equal("https://example.test/programme", url.Value);

        Assert.Equal("GB", programmeElement.Element("country")!.Value);

        var episodeNumber = programmeElement.Element("episode-num")!;
        Assert.Equal("xmltv_ns", (string?)episodeNumber.Attribute("system"));
        Assert.Equal("1.2.", episodeNumber.Value);

        var previouslyShown = programmeElement.Element("previously-shown")!;
        Assert.Equal("20260501120000 +0000", (string?)previouslyShown.Attribute("start"));
        Assert.Equal("old-channel", (string?)previouslyShown.Attribute("channel"));

        var premiere = programmeElement.Element("premiere")!;
        Assert.Equal("en", (string?)premiere.Attribute("lang"));
        Assert.Empty(premiere.Value);
        Assert.Equal("Final showing", programmeElement.Element("last-chance")!.Value);
        Assert.Empty(programmeElement.Element("new")!.Elements());
    }

    [Fact]
    public async Task WriteProgrammeAsync_OmitsDefaultedEpisodeNumberSystemWhenNull()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title");
        programme.EpisodeNumbers.Add(new XmlTvEpisodeNumber("S1E2"));
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        var episodeNumber =
            Assert.Single(XDocument.Parse(output.ToString()).Root!.Element("programme")!.Elements("episode-num"));
        Assert.Null(episodeNumber.Attribute("system"));
        Assert.Equal("S1E2", episodeNumber.Value);
    }

    [Fact]
    public async Task WriteProgrammeAsync_NullSimpleCollectionItem_ThrowsXmlTvWriteExceptionBeforeEmittingProgramme()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title");
        programme.Categories.Add(null!);
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken));
        Assert.DoesNotContain("<programme", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteProgrammeAsync_EmptyDescription_WritesPresentEmptyDescElement()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title");
        programme.Descriptions.Add(new XmlTvLocalizedText(""));
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        Assert.Contains("<desc />", output.ToString(), StringComparison.Ordinal);
        var desc = Assert.Single(XDocument.Parse(output.ToString()).Root!.Element("programme")!.Elements("desc"));
        Assert.Empty(desc.Value);
    }
}
