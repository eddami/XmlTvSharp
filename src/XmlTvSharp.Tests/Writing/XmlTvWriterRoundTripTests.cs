namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterRoundTripTests
{
    [Fact]
    public async Task WriteAsync_StandardDocument_RoundTripsSupportedModelData()
    {
        const string xml = """
                           <tv date="20260605"
                               source-info-name="Source"
                               source-info-url="https://example.test/source"
                               source-data-url="https://example.test/data"
                               generator-info-name="Generator"
                               generator-info-url="https://example.test/generator">
                             <channel id="channel-one">
                               <display-name lang="en">Channel One</display-name>
                               <icon src="channel.png" width="100" height="50" />
                               <url system="official">https://example.test/channel</url>
                             </channel>
                             <programme start="20260605120000 +0000"
                                 stop="20260605123000 +0000"
                                 channel="channel-one"
                                 clumpidx="1/2">
                               <title lang="en">News</title>
                               <sub-title>Evening</sub-title>
                               <desc>Description</desc>
                               <credits>
                                 <director>Jane Doe</director>
                                 <actor role="Host" guest="yes">John <image type="person">john.jpg</image><url system="official">https://example.test/john</url></actor>
                               </credits>
                               <date>20260605</date>
                               <category lang="en">News</category>
                               <keyword>breaking</keyword>
                               <language lang="en">English</language>
                               <orig-language>French</orig-language>
                               <length units="minutes">30</length>
                               <icon src="programme.png" />
                               <url>https://example.test/programme</url>
                               <country>GB</country>
                               <episode-num system="xmltv_ns">0.1.</episode-num>
                               <video><present>yes</present><colour>no</colour><aspect>16:9</aspect><quality>HDTV</quality></video>
                               <audio><present>yes</present><stereo>stereo</stereo></audio>
                               <previously-shown start="20260501120000 +0000" channel="old-channel" />
                               <premiere lang="en">Premiere</premiere>
                               <last-chance>Last chance</last-chance>
                               <new />
                               <subtitles type="teletext"><language>English</language></subtitles>
                               <rating system="MPAA"><value>PG</value><icon src="rating.png" /></rating>
                               <star-rating><value>4 / 5</value></star-rating>
                               <review type="url" source="Example" reviewer="Reviewer" lang="en">https://example.test/review</review>
                               <image type="poster" size="3" orient="P" system="tmdb">poster.jpg</image>
                             </programme>
                           </tv>
                           """;
        var document = await XmlTvReader.ReadAsync(new StringReader(xml));
        var output = new StringWriter();

        await XmlTvWriter.WriteAsync(document, output);
        var roundTripped = await XmlTvReader.ReadAsync(new StringReader(output.ToString()));

        Assert.Equal("20260605", roundTripped.Metadata.Date!.ToXmlTvString());
        Assert.Equal("Source", roundTripped.Metadata.SourceInfoName);
        Assert.Equal("https://example.test/source", roundTripped.Metadata.SourceInfoUrl);
        Assert.Equal("https://example.test/data", roundTripped.Metadata.SourceDataUrl);
        Assert.Equal("Generator", roundTripped.Metadata.GeneratorInfoName);
        Assert.Equal("https://example.test/generator", roundTripped.Metadata.GeneratorInfoUrl);

        var channel = Assert.Single(roundTripped.Channels);
        Assert.Equal("channel-one", channel.Id);
        Assert.Equal(new XmlTvLocalizedText("Channel One", "en"), Assert.Single(channel.DisplayNames));
        Assert.Equal(new XmlTvIcon("channel.png", 100, 50), Assert.Single(channel.Icons));
        Assert.Equal(new XmlTvUrl("https://example.test/channel", "official"), Assert.Single(channel.Urls));

        var programme = Assert.Single(roundTripped.Programmes);
        Assert.Equal("20260605120000 +0000", programme.Start.ToXmlTvString());
        Assert.Equal("20260605123000 +0000", programme.Stop!.ToXmlTvString());
        Assert.Equal("channel-one", programme.ChannelId);
        Assert.Equal(new XmlTvClumpIndex(1, 2), programme.ClumpIndex);
        Assert.Equal(new XmlTvLocalizedText("News", "en"), Assert.Single(programme.Titles));
        Assert.Equal(new XmlTvLocalizedText("Evening"), Assert.Single(programme.SubTitles));
        Assert.Equal(new XmlTvLocalizedText("Description"), Assert.Single(programme.Descriptions));

        var credits = programme.Credits!;
        Assert.Equal("Jane Doe", Assert.Single(credits.Directors).Text);
        var actor = Assert.Single(credits.Actors);
        Assert.Equal("Host", actor.Role);
        Assert.True(actor.IsGuest);
        Assert.Equal("John ", actor.Text);
        Assert.Equal(new XmlTvImage("john.jpg", XmlTvImageType.Person), Assert.Single(actor.Images));
        Assert.Equal(new XmlTvUrl("https://example.test/john", "official"), Assert.Single(actor.Urls));

        Assert.Equal("20260605", programme.Date!.ToXmlTvString());
        Assert.Equal(new XmlTvLocalizedText("News", "en"), Assert.Single(programme.Categories));
        Assert.Equal(new XmlTvLocalizedText("breaking"), Assert.Single(programme.Keywords));
        Assert.Equal(new XmlTvLocalizedText("English", "en"), programme.Language);
        Assert.Equal(new XmlTvLocalizedText("French"), programme.OriginalLanguage);
        Assert.Equal(XmlTvDuration.FromMinutes(30), programme.Length);
        Assert.Equal(new XmlTvIcon("programme.png"), Assert.Single(programme.Icons));
        Assert.Equal(new XmlTvUrl("https://example.test/programme"), Assert.Single(programme.Urls));
        Assert.Equal(new XmlTvLocalizedText("GB"), Assert.Single(programme.Countries));
        Assert.Equal(new XmlTvEpisodeNumber("0.1.", "xmltv_ns"), Assert.Single(programme.EpisodeNumbers));
        Assert.Equal(new XmlTvVideo(true, false, "16:9", "HDTV"), programme.Video);
        Assert.Equal(new XmlTvAudio(true, "stereo"), programme.Audio);
        Assert.Equal(new XmlTvPreviouslyShown(XmlTvDateTime.Parse("20260501120000 +0000"), "old-channel"),
            programme.PreviouslyShown);
        Assert.Equal(new XmlTvLocalizedText("Premiere", "en"), programme.Premiere);
        Assert.Equal(new XmlTvLocalizedText("Last chance"), programme.LastChance);
        Assert.True(programme.IsNew);
        Assert.Equal(new XmlTvSubtitles(XmlTvSubtitleType.Teletext, new XmlTvLocalizedText("English")),
            Assert.Single(programme.Subtitles));

        var rating = Assert.Single(programme.Ratings);
        Assert.Equal("MPAA", rating.System);
        Assert.Equal("PG", rating.Value);
        Assert.Equal(new XmlTvIcon("rating.png"), Assert.Single(rating.Icons));
        var starRating = Assert.Single(programme.StarRatings);
        Assert.Equal("4 / 5", starRating.Value);
        Assert.Null(starRating.System);
        Assert.Empty(starRating.Icons);
        Assert.Equal(new XmlTvReview("https://example.test/review", XmlTvReviewType.Url, "Example", "Reviewer", "en"),
            Assert.Single(programme.Reviews));
        Assert.Equal(new XmlTvImage("poster.jpg", XmlTvImageType.Poster, XmlTvImageSize.Large,
            XmlTvImageOrientation.Portrait, "tmdb"), Assert.Single(programme.Images));
    }
}
