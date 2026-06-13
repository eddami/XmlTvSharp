using System.Text;
using System.Xml;

namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterDtdConformanceTests
{
    [Fact]
    public async Task WriteAsync_MinimalDocument_ProducesDtdValidXmlTv()
    {
        var output = new StringWriter();
        var document = new XmlTvDocument();

        await XmlTvWriter.WriteAsync(document, output, new XmlTvWriterOptions
        {
            OmitXmlDeclaration = true
        }, TestContext.Current.CancellationToken);

        ValidateAgainstXmlTvDtd(output.ToString());
    }

    [Fact]
    public async Task WriteAsync_StandardDocument_ProducesDtdValidXmlTv()
    {
        var output = new StringWriter();
        var document = CreateFullStandardDocument();

        await XmlTvWriter.WriteAsync(document, output, new XmlTvWriterOptions
        {
            OmitXmlDeclaration = true
        }, TestContext.Current.CancellationToken);

        ValidateAgainstXmlTvDtd(output.ToString());
    }

    [Fact]
    public async Task WriteAsync_RepeatedStandardCollections_ProducesDtdValidXmlTv()
    {
        var output = new StringWriter();
        var document = CreateDocumentWithRepeatedCollections();

        await XmlTvWriter.WriteAsync(document, output, new XmlTvWriterOptions
        {
            OmitXmlDeclaration = true
        }, TestContext.Current.CancellationToken);

        ValidateAgainstXmlTvDtd(output.ToString());
    }

    [Fact]
    public async Task WriteAsync_StandardProfileWithExtensionData_ProducesDtdValidXmlTv()
    {
        var output = new StringWriter();
        var document = new XmlTvDocument();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one.tv",
            "Live Programme");
        programme.Extensions = new XmlTvProgrammeExtensions
        {
            Jellyfin = new XmlTvJellyfinProgrammeExtensions
            {
                IsLive = true
            }
        };
        document.Programmes.Add(programme);

        await XmlTvWriter.WriteAsync(document, output, new XmlTvWriterOptions
        {
            OmitXmlDeclaration = true
        }, TestContext.Current.CancellationToken);

        ValidateAgainstXmlTvDtd(output.ToString());
        Assert.DoesNotContain("<live", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteAsync_EmptyOptionalMediaText_ProducesDtdValidXmlTv()
    {
        var output = new StringWriter();
        var document = new XmlTvDocument();
        document.Programmes.Add(new XmlTvProgramme(
            XmlTvDateTime.Parse("20260605120000 +0000"),
            "channel-one.tv",
            "Programme One")
        {
            Video = new XmlTvVideo(Aspect: "", Quality: ""),
            Audio = new XmlTvAudio(Stereo: "")
        });

        await XmlTvWriter.WriteAsync(document, output, new XmlTvWriterOptions
        {
            OmitXmlDeclaration = true
        }, TestContext.Current.CancellationToken);

        ValidateAgainstXmlTvDtd(output.ToString());
    }

    private static XmlTvDocument CreateFullStandardDocument()
    {
        var document = new XmlTvDocument(new XmlTvMetadata
        {
            Date = XmlTvDateTime.Parse("20260605"),
            SourceInfoUrl = "https://example.test/source",
            SourceInfoName = "Example Source",
            SourceDataUrl = "https://example.test/data.xml",
            GeneratorInfoName = "XmlTvSharp.Tests",
            GeneratorInfoUrl = "https://example.test/generator"
        });

        var channel = new XmlTvChannel("channel-one.tv", new XmlTvLocalizedText("Channel One", "en"));
        channel.DisplayNames.Add(new XmlTvLocalizedText("One"));
        channel.Icons.Add(new XmlTvIcon("https://example.test/channel.png", 100, 100));
        channel.Urls.Add(new XmlTvUrl("https://example.test/channel", "official"));
        document.Channels.Add(channel);

        var programme = new XmlTvProgramme(
            XmlTvDateTime.Parse("20260605120000 +0000"),
            channel.Id,
            new XmlTvLocalizedText("Programme One", "en"))
        {
            Stop = XmlTvDateTime.Parse("20260605130000 +0000"),
            PdcStart = XmlTvDateTime.Parse("20260605115900 +0000"),
            VpsStart = XmlTvDateTime.Parse("20260605115800 +0000"),
            ShowView = "12345",
            VideoPlus = "67890",
            ClumpIndex = new XmlTvClumpIndex(0, 1),
            Credits = CreateCredits(),
            Date = XmlTvDateTime.Parse("20260605"),
            Language = new XmlTvLocalizedText("English", "en"),
            OriginalLanguage = new XmlTvLocalizedText("French", "fr"),
            Length = XmlTvDuration.FromMinutes(60),
            Video = new XmlTvVideo(true, false, "16:9", "HDTV"),
            Audio = new XmlTvAudio(true, "stereo"),
            PreviouslyShown = new XmlTvPreviouslyShown(XmlTvDateTime.Parse("20260501120000 +0000"), "channel-two.tv"),
            Premiere = new XmlTvLocalizedText("First showing", "en"),
            LastChance = new XmlTvLocalizedText("Last showing", "en"),
            IsNew = true
        };
        programme.SubTitles.Add(new XmlTvLocalizedText("Pilot", "en"));
        programme.Descriptions.Add(new XmlTvLocalizedText("A programme description.", "en"));
        programme.Categories.Add(new XmlTvLocalizedText("Drama", "en"));
        programme.Keywords.Add(new XmlTvLocalizedText("pilot", "en"));
        programme.Icons.Add(new XmlTvIcon("https://example.test/programme.png", 200, 100));
        programme.Urls.Add(new XmlTvUrl("https://example.test/programme", "official"));
        programme.Countries.Add(new XmlTvLocalizedText("US"));
        programme.EpisodeNumbers.Add(new XmlTvEpisodeNumber("S01E01", "onscreen"));
        programme.Subtitles.Add(new XmlTvSubtitles(XmlTvSubtitleType.Teletext,
            new XmlTvLocalizedText("English", "en")));

        var rating = new XmlTvRating("PG", "MPAA");
        rating.Icons.Add(new XmlTvIcon("https://example.test/rating.png"));
        programme.Ratings.Add(rating);

        var starRating = new XmlTvStarRating("4/5", "stars");
        starRating.Icons.Add(new XmlTvIcon("https://example.test/stars.png"));
        programme.StarRatings.Add(starRating);

        programme.Reviews.Add(new XmlTvReview("Strong review.", XmlTvReviewType.Text, "Example", "Reviewer", "en"));
        programme.Images.Add(new XmlTvImage(
            "https://example.test/poster.jpg",
            XmlTvImageType.Poster,
            XmlTvImageSize.Large,
            XmlTvImageOrientation.Portrait,
            "tmdb"));
        document.Programmes.Add(programme);

        document.Programmes.Add(new XmlTvProgramme(
            XmlTvDateTime.Parse("20260605130000 +0000"),
            channel.Id,
            "Minimal Programme"));

        return document;
    }

    private static XmlTvDocument CreateDocumentWithRepeatedCollections()
    {
        var document = new XmlTvDocument();

        document.Channels.Add(CreateRepeatedChannel("alpha.tv", "Alpha"));
        document.Channels.Add(CreateRepeatedChannel("beta.tv", "Beta"));

        var firstProgramme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "alpha.tv", "First");
        firstProgramme.Titles.Add(new XmlTvLocalizedText("Premier", "fr"));
        firstProgramme.Descriptions.Add(new XmlTvLocalizedText("First description."));
        firstProgramme.Descriptions.Add(new XmlTvLocalizedText("Second description."));
        firstProgramme.Categories.Add(new XmlTvLocalizedText("News"));
        firstProgramme.Categories.Add(new XmlTvLocalizedText("Current affairs"));
        firstProgramme.Urls.Add(new XmlTvUrl("https://example.test/first"));
        firstProgramme.Urls.Add(new XmlTvUrl("https://example.test/first/alternate"));
        firstProgramme.Images.Add(new XmlTvImage("https://example.test/first-small.jpg", XmlTvImageType.Poster,
            XmlTvImageSize.Small));
        firstProgramme.Images.Add(new XmlTvImage("https://example.test/first-large.jpg", XmlTvImageType.Poster,
            XmlTvImageSize.Large));

        var firstRating = new XmlTvRating("PG");
        firstRating.Icons.Add(new XmlTvIcon("https://example.test/pg.png"));
        firstProgramme.Ratings.Add(firstRating);
        firstProgramme.Ratings.Add(new XmlTvRating("12", "BBFC"));

        var firstStarRating = new XmlTvStarRating("3/5");
        firstStarRating.Icons.Add(new XmlTvIcon("https://example.test/three-stars.png"));
        firstProgramme.StarRatings.Add(firstStarRating);
        firstProgramme.StarRatings.Add(new XmlTvStarRating("7/10", "imdb"));

        var secondProgramme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605130000 +0000"), "beta.tv", "Second");
        secondProgramme.EpisodeNumbers.Add(new XmlTvEpisodeNumber("S01E02"));
        secondProgramme.EpisodeNumbers.Add(new XmlTvEpisodeNumber("0.1.", "xmltv_ns"));
        secondProgramme.Subtitles.Add(new XmlTvSubtitles(XmlTvSubtitleType.Teletext));
        secondProgramme.Subtitles.Add(new XmlTvSubtitles(XmlTvSubtitleType.Onscreen,
            new XmlTvLocalizedText("English")));
        secondProgramme.Reviews.Add(new XmlTvReview("Good.", XmlTvReviewType.Text));
        secondProgramme.Reviews.Add(new XmlTvReview("https://example.test/review", XmlTvReviewType.Url));

        document.Programmes.Add(firstProgramme);
        document.Programmes.Add(secondProgramme);

        return document;
    }

    private static XmlTvChannel CreateRepeatedChannel(string id, string name)
    {
        var channel = new XmlTvChannel(id, name);
        channel.DisplayNames.Add(new XmlTvLocalizedText($"{name} HD"));
        channel.Icons.Add(new XmlTvIcon($"https://example.test/{id}-small.png", 64, 64));
        channel.Icons.Add(new XmlTvIcon($"https://example.test/{id}-large.png", 256, 256));
        channel.Urls.Add(new XmlTvUrl($"https://example.test/{id}"));
        channel.Urls.Add(new XmlTvUrl($"https://example.test/{id}/schedule", "schedule"));

        return channel;
    }

    private static XmlTvCredits CreateCredits()
    {
        var credits = new XmlTvCredits();
        credits.Directors.Add(new XmlTvCredit("Director One"));

        var actor = new XmlTvActorCredit
        {
            Role = "Lead",
            IsGuest = true
        };
        actor.Content.Add(new XmlTvCreditText("Actor One"));
        actor.Content.Add(
            new XmlTvCreditImage(new XmlTvImage("https://example.test/actor.jpg", XmlTvImageType.Person)));
        actor.Content.Add(new XmlTvCreditUrl(new XmlTvUrl("https://example.test/actor", "official")));
        credits.Actors.Add(actor);

        credits.Writers.Add(new XmlTvCredit("Writer One"));
        credits.Adapters.Add(new XmlTvCredit("Adapter One"));
        credits.Producers.Add(new XmlTvCredit("Producer One"));
        credits.Composers.Add(new XmlTvCredit("Composer One"));
        credits.Editors.Add(new XmlTvCredit("Editor One"));
        credits.Presenters.Add(new XmlTvCredit("Presenter One"));
        credits.Commentators.Add(new XmlTvCredit("Commentator One"));
        credits.Guests.Add(new XmlTvCredit("Guest One"));

        return credits;
    }

    private static void ValidateAgainstXmlTvDtd(string xml)
    {
        var dtdPath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Dtd", "xmltv.dtd");
        var dtd = File.ReadAllText(dtdPath);
        var xmlWithDtd = $"<!DOCTYPE tv [{dtd}]>{xml}";
        var errors = new StringBuilder();
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            ValidationType = ValidationType.DTD,
            XmlResolver = null
        };
        settings.ValidationEventHandler += (_, args) => { errors.AppendLine(args.Message); };

        using var reader = XmlReader.Create(new StringReader(xmlWithDtd), settings);
        while (reader.Read())
        {
        }

        Assert.True(errors.Length == 0, errors.ToString());
    }
}
