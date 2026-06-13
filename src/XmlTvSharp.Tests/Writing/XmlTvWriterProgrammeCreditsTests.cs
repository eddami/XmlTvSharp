using System.Xml.Linq;

namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterProgrammeCreditsTests
{
    [Fact]
    public async Task WriteProgrammeAsync_WritesCreditsBeforeDateAndRoleGroupsInDtdOrder()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Date = XmlTvDateTime.Parse("20260605"),
            Credits = new XmlTvCredits()
        };
        programme.Credits.Guests.Add(new XmlTvCredit("Guest"));
        programme.Credits.Commentators.Add(new XmlTvCredit("Commentator"));
        programme.Credits.Presenters.Add(new XmlTvCredit("Presenter"));
        programme.Credits.Editors.Add(new XmlTvCredit("Editor"));
        programme.Credits.Composers.Add(new XmlTvCredit("Composer"));
        programme.Credits.Producers.Add(new XmlTvCredit("Producer"));
        programme.Credits.Adapters.Add(new XmlTvCredit("Adapter"));
        programme.Credits.Writers.Add(new XmlTvCredit("Writer"));
        programme.Credits.Actors.Add(new XmlTvActorCredit("Actor"));
        programme.Credits.Directors.Add(new XmlTvCredit("Director"));
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        var programmeElement = Assert.Single(XDocument.Parse(output.ToString()).Root!.Elements("programme"));
        Assert.Equal(new[] { "title", "credits", "date" },
            programmeElement.Elements().Select(element => element.Name.LocalName));

        var credits = programmeElement.Element("credits")!;
        Assert.Equal(
            new[]
            {
                "director",
                "actor",
                "writer",
                "adapter",
                "producer",
                "composer",
                "editor",
                "presenter",
                "commentator",
                "guest"
            },
            credits.Elements().Select(element => element.Name.LocalName));
        Assert.Equal("Director", credits.Element("director")!.Value);
        Assert.Equal("Actor", credits.Element("actor")!.Value);
        Assert.Equal("Writer", credits.Element("writer")!.Value);
        Assert.Equal("Guest", credits.Element("guest")!.Value);
    }

    [Fact]
    public async Task WriteProgrammeAsync_WritesActorAttributesAndMixedContentInModelOrder()
    {
        var output = new StringWriter();
        var actor = new XmlTvActorCredit
        {
            Role = "Hero",
            IsGuest = true
        };
        actor.Content.Add(new XmlTvCreditText("John "));
        actor.Content.Add(new XmlTvCreditImage(new XmlTvImage("john.jpg", XmlTvImageType.Person)));
        actor.Content.Add(new XmlTvCreditText(" Doe"));
        actor.Content.Add(new XmlTvCreditUrl(new XmlTvUrl("https://example.test/john", "official")));

        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Credits = new XmlTvCredits()
        };
        programme.Credits.Actors.Add(actor);
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        var actorElement = Assert.Single(
            XDocument.Parse(output.ToString()).Root!.Element("programme")!.Element("credits")!.Elements("actor"));
        Assert.Equal("Hero", (string?)actorElement.Attribute("role"));
        Assert.Equal("yes", (string?)actorElement.Attribute("guest"));
        Assert.Collection(
            actorElement.Nodes(),
            node => Assert.Equal("John ", Assert.IsType<XText>(node).Value),
            node => Assert.Equal("image", Assert.IsType<XElement>(node).Name.LocalName),
            node => Assert.Equal(" Doe", Assert.IsType<XText>(node).Value),
            node => Assert.Equal("url", Assert.IsType<XElement>(node).Name.LocalName));

        var image = actorElement.Element("image")!;
        Assert.Equal("person", (string?)image.Attribute("type"));
        Assert.Equal("john.jpg", image.Value);

        var url = actorElement.Element("url")!;
        Assert.Equal("official", (string?)url.Attribute("system"));
        Assert.Equal("https://example.test/john", url.Value);
    }

    [Fact]
    public async Task WriteProgrammeAsync_EmptyCredits_WritesEmptyCreditsElement()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Credits = new XmlTvCredits()
        };
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        var credits = Assert.Single(XDocument.Parse(output.ToString()).Root!.Element("programme")!.Elements("credits"));
        Assert.Empty(credits.Elements());
    }

    [Fact]
    public async Task WriteProgrammeAsync_EmptyCreditText_WritesMinimalCreditRoleElement()
    {
        var output = new StringWriter();
        var credit = new XmlTvCredit();
        credit.Content.Add(new XmlTvCreditText(""));
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Credits = new XmlTvCredits()
        };
        programme.Credits.Directors.Add(credit);
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        Assert.Contains("<director />", output.ToString(), StringComparison.Ordinal);
        var director = Assert.Single(XDocument.Parse(output.ToString()).Root!.Element("programme")!
            .Element("credits")!.Elements("director"));
        Assert.Empty(director.Value);
    }

    [Fact]
    public async Task WriteProgrammeAsync_NullCreditItem_ThrowsXmlTvWriteExceptionBeforeEmittingProgramme()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Credits = new XmlTvCredits()
        };
        programme.Credits.Directors.Add(null!);
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken));
        Assert.DoesNotContain("<programme", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteProgrammeAsync_NullCreditContentItem_ThrowsXmlTvWriteExceptionBeforeEmittingProgramme()
    {
        var output = new StringWriter();
        var credit = new XmlTvCredit();
        credit.Content.Add(null!);
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Credits = new XmlTvCredits()
        };
        programme.Credits.Directors.Add(credit);
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken));
        Assert.DoesNotContain("<programme", output.ToString(), StringComparison.Ordinal);
    }
}
