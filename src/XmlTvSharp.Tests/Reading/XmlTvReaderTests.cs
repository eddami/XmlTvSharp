using System.Text;

namespace XmlTvSharp.Tests.Reading;

public sealed class XmlTvReaderTests
{
    [Fact]
    public async Task ReadAsync_ParsesRootMetadataAndChannels()
    {
        const string xml = """
                           <tv date="20260605"
                               source-info-url="https://example.test/source"
                               source-info-name="Source"
                               source-data-url="https://example.test/data"
                               generator-info-name="Generator"
                               generator-info-url="https://example.test/generator">
                             <channel id="channel-one">
                               <display-name lang="en">Channel One</display-name>
                               <display-name>One</display-name>
                               <icon src="channel.png" width="100" height="50" />
                               <url system="official">https://example.test/channel</url>
                             </channel>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        Assert.Equal("20260605", document.Metadata.Date!.ToXmlTvString());
        Assert.Equal("https://example.test/source", document.Metadata.SourceInfoUrl);
        Assert.Equal("Source", document.Metadata.SourceInfoName);
        Assert.Equal("https://example.test/data", document.Metadata.SourceDataUrl);
        Assert.Equal("Generator", document.Metadata.GeneratorInfoName);
        Assert.Equal("https://example.test/generator", document.Metadata.GeneratorInfoUrl);

        var channel = Assert.Single(document.Channels);
        Assert.Equal("channel-one", channel.Id);
        Assert.Collection(
            channel.DisplayNames,
            name => Assert.Equal(new XmlTvLocalizedText("Channel One", "en"), name),
            name => Assert.Equal(new XmlTvLocalizedText("One"), name));

        var icon = Assert.Single(channel.Icons);
        Assert.Equal("channel.png", icon.Source);
        Assert.Equal(100, icon.Width);
        Assert.Equal(50, icon.Height);

        var url = Assert.Single(channel.Urls);
        Assert.Equal("official", url.System);
        Assert.Equal("https://example.test/channel", url.Value);
    }

    [Fact]
    public async Task ReadElementAsync_ReturnsAdjacentTopLevelElementsInOrder()
    {
        const string xml = """
                           <tv generator-info-name="Generator">
                             <channel id="one"><display-name>One</display-name></channel>
                             <channel id="two"><display-name>Two</display-name></channel>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>News</title>
                             </programme>
                           </tv>
                           """;

        using var reader = new XmlTvReader(new StringReader(xml));

        var metadata = await reader.ReadMetadataAsync();
        var first = Assert.IsType<XmlTvChannel>(await reader.ReadElementAsync());
        var second = Assert.IsType<XmlTvChannel>(await reader.ReadElementAsync());
        var third = Assert.IsType<XmlTvProgramme>(await reader.ReadElementAsync());
        var end = await reader.ReadElementAsync();

        Assert.Equal("Generator", metadata.GeneratorInfoName);
        Assert.Equal("one", first.Id);
        Assert.Equal("two", second.Id);
        Assert.Equal("one", third.ChannelId);
        Assert.Null(end);
    }

    [Fact]
    public async Task ReadElementAsync_PreservesLenientTopLevelSourceOrder()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>News</title>
                             </programme>
                             <channel id="one"><display-name>One</display-name></channel>
                           </tv>
                           """;

        using var reader = new XmlTvReader(new StringReader(xml));

        var first = Assert.IsType<XmlTvProgramme>(await reader.ReadElementAsync());
        var second = Assert.IsType<XmlTvChannel>(await reader.ReadElementAsync());
        var end = await reader.ReadElementAsync();

        Assert.Equal("one", first.ChannelId);
        Assert.Equal("one", second.Id);
        Assert.Null(end);
    }

    [Fact]
    public async Task ReadMetadataAsync_ReturnsRootMetadataForEmptyDocument()
    {
        const string xml = """
                           <tv source-info-name="Source" />
                           """;

        using var reader = new XmlTvReader(new StringReader(xml));

        var metadata = await reader.ReadMetadataAsync();
        var end = await reader.ReadElementAsync();

        Assert.Equal("Source", metadata.SourceInfoName);
        Assert.Null(end);
    }

    [Fact]
    public async Task ReadMetadataAsync_ReturnsCachedMetadataAfterElementRead()
    {
        const string xml = """
                           <tv source-info-name="Source">
                             <channel id="one"><display-name>One</display-name></channel>
                           </tv>
                           """;

        using var reader = new XmlTvReader(new StringReader(xml));

        var element = Assert.IsType<XmlTvChannel>(await reader.ReadElementAsync());
        var metadata = await reader.ReadMetadataAsync();
        var cachedMetadata = await reader.ReadMetadataAsync();

        Assert.Equal("one", element.Id);
        Assert.Same(metadata, cachedMetadata);
        Assert.Equal("Source", metadata.SourceInfoName);
    }

    [Fact]
    public async Task ReadAsync_NormalizesLenientTopLevelSourceOrderIntoTypedCollections()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>News</title>
                             </programme>
                             <channel id="one"><display-name>One</display-name></channel>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var channel = Assert.Single(document.Channels);
        var programme = Assert.Single(document.Programmes);

        Assert.Equal("one", channel.Id);
        Assert.Equal("one", programme.ChannelId);
        Assert.Equal(new XmlTvLocalizedText("News"), Assert.Single(programme.Titles));
    }

    [Fact]
    public async Task ReadAsync_ParsesMinimalProgrammeWithoutStop()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title lang="en">News</title>
                               <sub-title>Evening</sub-title>
                               <desc>Description</desc>
                               <category>News</category>
                               <icon src="programme.png" />
                               <url>https://example.test/programme</url>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var programme = Assert.Single(document.Programmes);
        Assert.Equal("20260605120000 +0000", programme.Start.ToXmlTvString());
        Assert.Null(programme.Stop);
        Assert.Equal("channel-one", programme.ChannelId);
        Assert.Equal(new XmlTvLocalizedText("News", "en"), Assert.Single(programme.Titles));
        Assert.Equal(new XmlTvLocalizedText("Evening"), Assert.Single(programme.SubTitles));
        Assert.Equal(new XmlTvLocalizedText("Description"), Assert.Single(programme.Descriptions));
        Assert.Equal(new XmlTvLocalizedText("News"), Assert.Single(programme.Categories));
        Assert.Equal("programme.png", Assert.Single(programme.Icons).Source);
        Assert.Equal("https://example.test/programme", Assert.Single(programme.Urls).Value);
    }

    [Fact]
    public async Task ReadAsync_PreservesRepeatedProgrammeTitlesAndNewMarker()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title lang="en">News</title>
                               <title lang="fr">Nouvelles</title>
                               <new />
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var programme = Assert.Single(document.Programmes);
        Assert.True(programme.IsNew);
        Assert.Collection(
            programme.Titles,
            title => Assert.Equal(new XmlTvLocalizedText("News", "en"), title),
            title => Assert.Equal(new XmlTvLocalizedText("Nouvelles", "fr"), title));
    }

    [Fact]
    public async Task ReadAsync_ParsesStandardProgrammeMetadata()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000"
                                 stop="20260605123000 +0000"
                                 pdc-start="20260605115900 +0000"
                                 vps-start="20260605115800 +0000"
                                 showview="12345"
                                 videoplus="67890"
                                 channel="channel-one"
                                 clumpidx="1/3">
                               <title>News</title>
                               <credits>
                                 <director>Jane Doe</director>
                                 <actor role="Host" guest="yes">John <image type="person">john.jpg</image><url>https://example.test/john</url></actor>
                               </credits>
                               <date>20260605</date>
                               <keyword lang="en">breaking</keyword>
                               <language lang="en">English</language>
                               <orig-language>French</orig-language>
                               <length units="minutes">30</length>
                               <country>GB</country>
                               <episode-num system="xmltv_ns">0.1.</episode-num>
                               <video><present>yes</present><colour>no</colour><aspect>16:9</aspect><quality>HDTV</quality></video>
                               <audio><present>yes</present><stereo>stereo</stereo></audio>
                               <previously-shown start="20260501120000 +0000" channel="old-channel" />
                               <premiere lang="en">Premiere</premiere>
                               <last-chance>Last chance</last-chance>
                               <subtitles type="teletext"><language>English</language></subtitles>
                               <rating system="MPAA"><value>PG</value><icon src="rating.png" /></rating>
                               <star-rating><value>4 / 5</value></star-rating>
                               <review type="url" source="Example" reviewer="Reviewer" lang="en">https://example.test/review</review>
                               <image type="poster" size="3" orient="P" system="provider">poster.jpg</image>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var programme = Assert.Single(document.Programmes);
        Assert.Equal("20260605123000 +0000", programme.Stop!.ToXmlTvString());
        Assert.Equal("20260605115900 +0000", programme.PdcStart!.ToXmlTvString());
        Assert.Equal("20260605115800 +0000", programme.VpsStart!.ToXmlTvString());
        Assert.Equal("12345", programme.ShowView);
        Assert.Equal("67890", programme.VideoPlus);
        Assert.Equal(new XmlTvClumpIndex(1, 3), programme.ClumpIndex);
        Assert.Equal("20260605", programme.Date!.ToXmlTvString());
        Assert.Equal(new XmlTvLocalizedText("breaking", "en"), Assert.Single(programme.Keywords));
        Assert.Equal(new XmlTvLocalizedText("English", "en"), programme.Language);
        Assert.Equal(new XmlTvLocalizedText("French"), programme.OriginalLanguage);
        Assert.Equal(new XmlTvDuration(30, XmlTvDurationUnit.Minutes), programme.Length);
        Assert.Equal(new XmlTvLocalizedText("GB"), Assert.Single(programme.Countries));
        Assert.Equal(new XmlTvEpisodeNumber("0.1.", "xmltv_ns"), Assert.Single(programme.EpisodeNumbers));
        Assert.Equal(new XmlTvVideo(true, false, "16:9", "HDTV"), programme.Video);
        Assert.Equal(new XmlTvAudio(true, "stereo"), programme.Audio);
        Assert.Equal(new XmlTvPreviouslyShown(XmlTvDateTime.Parse("20260501120000 +0000"), "old-channel"),
            programme.PreviouslyShown);
        Assert.Equal(new XmlTvLocalizedText("Premiere", "en"), programme.Premiere);
        Assert.Equal(new XmlTvLocalizedText("Last chance"), programme.LastChance);
        Assert.Equal(new XmlTvSubtitles(XmlTvSubtitleType.Teletext, new XmlTvLocalizedText("English")),
            Assert.Single(programme.Subtitles));

        var director = Assert.Single(programme.Credits!.Directors);
        Assert.Equal("Jane Doe", director.Text);

        var actor = Assert.Single(programme.Credits.Actors);
        Assert.Equal("Host", actor.Role);
        Assert.True(actor.IsGuest);
        Assert.Equal("John ", actor.Text);
        Assert.Equal(new XmlTvImage("john.jpg", XmlTvImageType.Person), Assert.Single(actor.Images));
        Assert.Equal(new XmlTvUrl("https://example.test/john"), Assert.Single(actor.Urls));

        var rating = Assert.Single(programme.Ratings);
        Assert.Equal("PG", rating.Value);
        Assert.Equal("MPAA", rating.System);
        Assert.Equal("rating.png", Assert.Single(rating.Icons).Source);
        Assert.Equal("4 / 5", Assert.Single(programme.StarRatings).Value);
        Assert.Equal(new XmlTvReview("https://example.test/review", XmlTvReviewType.Url, "Example", "Reviewer", "en"),
            Assert.Single(programme.Reviews));
        Assert.Equal(
            new XmlTvImage("poster.jpg", XmlTvImageType.Poster, XmlTvImageSize.Large, XmlTvImageOrientation.Portrait,
                "provider"), Assert.Single(programme.Images));
    }

    [Fact]
    public async Task ReadAsync_ParsesLocalizedDefaultsAndEmptyAnnouncements()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <desc lang="en">Description</desc>
                               <category lang="fr">Actualités</category>
                               <country lang="en">GB</country>
                               <episode-num>Episode One</episode-num>
                               <premiere></premiere>
                               <last-chance></last-chance>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var programme = Assert.Single(document.Programmes);
        Assert.Equal(new XmlTvLocalizedText("Description", "en"), Assert.Single(programme.Descriptions));
        Assert.Equal(new XmlTvLocalizedText("Actualités", "fr"), Assert.Single(programme.Categories));
        Assert.Equal(new XmlTvLocalizedText("GB", "en"), Assert.Single(programme.Countries));

        var episodeNumber = Assert.Single(programme.EpisodeNumbers);
        Assert.Null(episodeNumber.System);
        Assert.Equal("onscreen", episodeNumber.EffectiveSystem);
        Assert.Equal(new XmlTvLocalizedText(string.Empty), programme.Premiere);
        Assert.Equal(new XmlTvLocalizedText(string.Empty), programme.LastChance);
    }

    [Fact]
    public async Task ReadAsync_ParsesAllCreditRoleCollections()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <credits>
                                 <writer>Writer</writer>
                                 <adapter>Adapter</adapter>
                                 <producer>Producer</producer>
                                 <composer>Composer</composer>
                                 <editor>Editor</editor>
                                 <presenter>Presenter</presenter>
                                 <commentator>Commentator</commentator>
                                 <guest>Guest</guest>
                               </credits>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var credits = Assert.Single(document.Programmes).Credits!;
        Assert.Equal("Writer", Assert.Single(credits.Writers).Text);
        Assert.Equal("Adapter", Assert.Single(credits.Adapters).Text);
        Assert.Equal("Producer", Assert.Single(credits.Producers).Text);
        Assert.Equal("Composer", Assert.Single(credits.Composers).Text);
        Assert.Equal("Editor", Assert.Single(credits.Editors).Text);
        Assert.Equal("Presenter", Assert.Single(credits.Presenters).Text);
        Assert.Equal("Commentator", Assert.Single(credits.Commentators).Text);
        Assert.Equal("Guest", Assert.Single(credits.Guests).Text);
    }

    [Fact]
    public async Task ReadAsync_ParsesSubtitleRatingAndImageEnumValues()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <subtitles type="teletext" />
                               <subtitles type="onscreen" />
                               <subtitles type="deaf-signed" />
                               <star-rating system="stars"><value>4 / 5</value><icon src="star.png" /></star-rating>
                               <image type="poster" size="1" orient="P">poster-small.jpg</image>
                               <image type="backdrop" size="2" orient="L">backdrop-medium.jpg</image>
                               <image type="still" size="3">still-large.jpg</image>
                               <image type="person">person.jpg</image>
                               <image type="character">character.jpg</image>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var programme = Assert.Single(document.Programmes);
        Assert.Collection(
            programme.Subtitles,
            subtitles => Assert.Equal(XmlTvSubtitleType.Teletext, subtitles.Type),
            subtitles => Assert.Equal(XmlTvSubtitleType.Onscreen, subtitles.Type),
            subtitles => Assert.Equal(XmlTvSubtitleType.DeafSigned, subtitles.Type));

        var starRating = Assert.Single(programme.StarRatings);
        Assert.Equal("stars", starRating.System);
        Assert.Equal("star.png", Assert.Single(starRating.Icons).Source);

        Assert.Collection(
            programme.Images,
            image => Assert.Equal(
                new XmlTvImage("poster-small.jpg", XmlTvImageType.Poster, XmlTvImageSize.Small,
                    XmlTvImageOrientation.Portrait), image),
            image => Assert.Equal(
                new XmlTvImage("backdrop-medium.jpg", XmlTvImageType.Backdrop, XmlTvImageSize.Medium,
                    XmlTvImageOrientation.Landscape), image),
            image => Assert.Equal(new XmlTvImage("still-large.jpg", XmlTvImageType.Still, XmlTvImageSize.Large), image),
            image => Assert.Equal(new XmlTvImage("person.jpg", XmlTvImageType.Person), image),
            image => Assert.Equal(new XmlTvImage("character.jpg", XmlTvImageType.Character), image));
    }

    [Fact]
    public async Task ReadAsync_LengthRejectsGroupedDecimalText()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <length units="minutes">1,000</length>
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Theory]
    [InlineData("<icon src=\"programme.png\">unexpected</icon>")]
    [InlineData("<previously-shown>unexpected</previously-shown>")]
    [InlineData("<new>unexpected</new>")]
    public async Task ReadAsync_RecognizedEmptyElementRejectsContent(string element)
    {
        var xml = $$"""
                    <tv>
                      <programme start="20260605120000 +0000" channel="channel-one">
                        <title>News</title>
                        {{element}}
                      </programme>
                    </tv>
                    """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_RecognizedEmptyElementAllowsLongEmptyForm()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <new></new>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        Assert.True(Assert.Single(document.Programmes).IsNew);
    }

    [Theory]
    [InlineData("<url></url>")]
    [InlineData("<episode-num></episode-num>")]
    [InlineData("<rating><value></value></rating>")]
    [InlineData("<star-rating><value></value></star-rating>")]
    [InlineData("<review type=\"text\"></review>")]
    [InlineData("<image></image>")]
    public async Task ReadAsync_InvalidRequiredTextValueThrowsReadException(string element)
    {
        var xml = $$"""
                    <tv>
                      <programme start="20260605120000 +0000" channel="channel-one">
                        <title>News</title>
                        {{element}}
                      </programme>
                    </tv>
                    """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Theory]
    [InlineData("<title>News<x-provider /></title>", "title")]
    [InlineData("<desc>Description<x-provider /></desc>", "desc")]
    [InlineData("<rating><value>PG<x-provider /></value></rating>", "value")]
    [InlineData("<image>poster<x-provider /></image>", "image")]
    public async Task ReadAsync_TextOnlyElementWithChildThrowsReadException(string element, string elementName)
    {
        var xml = $$"""
                    <tv>
                      <programme start="20260605120000 +0000" channel="channel-one">
                        <title>News</title>
                        {{element}}
                      </programme>
                    </tv>
                    """;

        var exception =
            await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));

        Assert.Contains(elementName, exception.Message, StringComparison.Ordinal);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task ReadAsync_PreservesSemanticCreditMixedContentOrder()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <credits>
                                 <actor>John <image type="person">john.jpg</image> Doe<url>https://example.test/john</url></actor>
                               </credits>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var actor = Assert.Single(Assert.Single(document.Programmes).Credits!.Actors);
        Assert.Collection(
            actor.Content,
            content => Assert.Equal(new XmlTvCreditText("John "), content),
            content => Assert.Equal(new XmlTvCreditImage(new XmlTvImage("john.jpg", XmlTvImageType.Person)), content),
            content => Assert.Equal(new XmlTvCreditText(" Doe"), content),
            content => Assert.Equal(new XmlTvCreditUrl(new XmlTvUrl("https://example.test/john")), content));
    }

    [Fact]
    public async Task ReadAsync_IgnoresFormattingOnlyWhitespaceInCreditMixedContent()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <credits>
                                 <actor>
                                   <image type="person">john.jpg</image>
                                   <url>https://example.test/john</url>
                                 </actor>
                               </credits>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var actor = Assert.Single(Assert.Single(document.Programmes).Credits!.Actors);
        Assert.Collection(
            actor.Content,
            content => Assert.Equal(new XmlTvCreditImage(new XmlTvImage("john.jpg", XmlTvImageType.Person)), content),
            content => Assert.Equal(new XmlTvCreditUrl(new XmlTvUrl("https://example.test/john")), content));
    }

    [Fact]
    public async Task ReadAsync_LeavesStreamOpenWhenUsingStaticStreamOverload()
    {
        const string xml = """
                           <tv>
                             <channel id="one"><display-name>One</display-name></channel>
                           </tv>
                           """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        await XmlTvReader.ReadAsync(stream);

        Assert.True(stream.CanRead);
    }

    [Fact]
    public async Task ReadAsync_AcceptsCancellationTokenAsSecondArgument()
    {
        const string xml = """
                           <tv>
                             <channel id="one"><display-name>One</display-name></channel>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), CancellationToken.None);

        Assert.Single(document.Channels);
    }

    [Fact]
    public async Task ReadAsync_AcceptsImmutableOptionsWithCancellationToken()
    {
        const string xml = """
                           <tv>
                             <channel id="one" unknown="value"><display-name>One</display-name></channel>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            UnknownAttributeHandling = XmlTvUnknownContentHandling.Ignore
        };

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), options, CancellationToken.None);

        Assert.Single(document.Channels);
    }

    [Fact]
    public async Task ReadAsync_WithCancelledToken_ThrowsBeforeOpeningFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xml");
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            XmlTvReader.ReadAsync(path, cancellationTokenSource.Token));
    }

    [Theory]
    [InlineData("profile")]
    [InlineData("unknown-element")]
    [InlineData("unknown-attribute")]
    [InlineData("x-extension")]
    public void Constructor_InvalidReaderOption_ThrowsArgumentOutOfRangeException(string option)
    {
        var options = option switch
        {
            "profile" => new XmlTvReaderOptions
            {
                CompatibilityProfile = (XmlTvCompatibilityProfile)999
            },
            "unknown-element" => new XmlTvReaderOptions
            {
                UnknownElementHandling = (XmlTvUnknownContentHandling)999
            },
            "unknown-attribute" => new XmlTvReaderOptions
            {
                UnknownAttributeHandling = (XmlTvUnknownContentHandling)999
            },
            _ => new XmlTvReaderOptions
            {
                XExtensionHandling = (XmlTvUnknownContentHandling)999
            }
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new XmlTvReader(new StringReader("<tv />"), options));
    }
}
