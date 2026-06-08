using System.Xml.Linq;

namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterProgrammeComplexTests
{
    [Fact]
    public async Task WriteProgrammeAsync_WritesMediaRatingsReviewsAndImagesInDtdOrder()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title")
        {
            Video = new XmlTvVideo(true, false, "16:9", "HDTV"),
            Audio = new XmlTvAudio(true, "stereo")
        };
        programme.Subtitles.Add(new XmlTvSubtitles(XmlTvSubtitleType.Teletext,
            new XmlTvLocalizedText("English", "en")));

        var rating = new XmlTvRating("PG", "BBFC");
        rating.Icons.Add(new XmlTvIcon("rating.png"));
        programme.Ratings.Add(rating);

        var starRating = new XmlTvStarRating("4 / 5", "stars");
        starRating.Icons.Add(new XmlTvIcon("star.png", 50, 25));
        programme.StarRatings.Add(starRating);

        programme.Reviews.Add(new XmlTvReview(
            "https://example.test/review",
            XmlTvReviewType.Url,
            "Example",
            "Reviewer",
            "en"));
        programme.Images.Add(new XmlTvImage(
            "poster.jpg",
            XmlTvImageType.Poster,
            XmlTvImageSize.Large,
            XmlTvImageOrientation.Portrait,
            "tmdb"));

        using var writer = new XmlTvWriter(output);

        await writer.StartAsync();
        await writer.WriteProgrammeAsync(programme);
        await writer.CompleteAsync();

        var programmeElement = Assert.Single(XDocument.Parse(output.ToString()).Root!.Elements("programme"));
        Assert.Equal(
            new[]
            {
                "title",
                "video",
                "audio",
                "subtitles",
                "rating",
                "star-rating",
                "review",
                "image"
            },
            programmeElement.Elements().Select(element => element.Name.LocalName));

        var video = programmeElement.Element("video")!;
        Assert.Equal(new[] { "present", "colour", "aspect", "quality" },
            video.Elements().Select(element => element.Name.LocalName));
        Assert.Equal("yes", video.Element("present")!.Value);
        Assert.Equal("no", video.Element("colour")!.Value);
        Assert.Equal("16:9", video.Element("aspect")!.Value);
        Assert.Equal("HDTV", video.Element("quality")!.Value);

        var audio = programmeElement.Element("audio")!;
        Assert.Equal(new[] { "present", "stereo" }, audio.Elements().Select(element => element.Name.LocalName));
        Assert.Equal("yes", audio.Element("present")!.Value);
        Assert.Equal("stereo", audio.Element("stereo")!.Value);

        var subtitles = programmeElement.Element("subtitles")!;
        Assert.Equal("teletext", (string?)subtitles.Attribute("type"));
        Assert.Equal("English", subtitles.Element("language")!.Value);
        Assert.Equal("en", (string?)subtitles.Element("language")!.Attribute("lang"));

        var ratingElement = programmeElement.Element("rating")!;
        Assert.Equal("BBFC", (string?)ratingElement.Attribute("system"));
        Assert.Equal(new[] { "value", "icon" }, ratingElement.Elements().Select(element => element.Name.LocalName));
        Assert.Equal("PG", ratingElement.Element("value")!.Value);
        Assert.Equal("rating.png", (string?)ratingElement.Element("icon")!.Attribute("src"));

        var starRatingElement = programmeElement.Element("star-rating")!;
        Assert.Equal("stars", (string?)starRatingElement.Attribute("system"));
        Assert.Equal("4 / 5", starRatingElement.Element("value")!.Value);
        Assert.Equal("star.png", (string?)starRatingElement.Element("icon")!.Attribute("src"));
        Assert.Equal("50", (string?)starRatingElement.Element("icon")!.Attribute("width"));
        Assert.Equal("25", (string?)starRatingElement.Element("icon")!.Attribute("height"));

        var review = programmeElement.Element("review")!;
        Assert.Equal("url", (string?)review.Attribute("type"));
        Assert.Equal("Example", (string?)review.Attribute("source"));
        Assert.Equal("Reviewer", (string?)review.Attribute("reviewer"));
        Assert.Equal("en", (string?)review.Attribute("lang"));
        Assert.Equal("https://example.test/review", review.Value);

        var image = programmeElement.Element("image")!;
        Assert.Equal("poster", (string?)image.Attribute("type"));
        Assert.Equal("3", (string?)image.Attribute("size"));
        Assert.Equal("P", (string?)image.Attribute("orient"));
        Assert.Equal("tmdb", (string?)image.Attribute("system"));
        Assert.Equal("poster.jpg", image.Value);
    }

    [Theory]
    [InlineData(XmlTvSubtitleType.Teletext, "teletext")]
    [InlineData(XmlTvSubtitleType.Onscreen, "onscreen")]
    [InlineData(XmlTvSubtitleType.DeafSigned, "deaf-signed")]
    public async Task WriteProgrammeAsync_WritesSubtitleTypeTokens(XmlTvSubtitleType type, string expected)
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title");
        programme.Subtitles.Add(new XmlTvSubtitles(type));
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync();
        await writer.WriteProgrammeAsync(programme);
        await writer.CompleteAsync();

        var subtitles =
            Assert.Single(XDocument.Parse(output.ToString()).Root!.Element("programme")!.Elements("subtitles"));
        Assert.Equal(expected, (string?)subtitles.Attribute("type"));
    }

    [Fact]
    public async Task WriteProgrammeAsync_NullRatingIcon_ThrowsXmlTvWriteExceptionBeforeEmittingProgramme()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title");
        var rating = new XmlTvRating("PG");
        rating.Icons.Add(null!);
        programme.Ratings.Add(rating);
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync();

        await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteProgrammeAsync(programme));
        Assert.DoesNotContain("<programme", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteProgrammeAsync_EmptyRatingValue_ThrowsXmlTvWriteExceptionBeforeEmittingProgramme()
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title");
        programme.Ratings.Add(new XmlTvRating("PG") { Value = "" });
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync();

        await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteProgrammeAsync(programme));
        Assert.DoesNotContain("<programme", output.ToString(), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("audio-stereo")]
    [InlineData("video-aspect")]
    [InlineData("video-quality")]
    public async Task WriteProgrammeAsync_EmptyOptionalMediaText_ThrowsXmlTvWriteExceptionBeforeEmittingProgramme(
        string field)
    {
        var output = new StringWriter();
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "Title");
        if (field == "audio-stereo")
        {
            programme.Audio = new XmlTvAudio(Stereo: "");
        }
        else if (field == "video-aspect")
        {
            programme.Video = new XmlTvVideo(Aspect: "");
        }
        else
        {
            programme.Video = new XmlTvVideo(Quality: "");
        }

        using var writer = new XmlTvWriter(output);

        await writer.StartAsync();

        var exception = await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.WriteProgrammeAsync(programme));
        Assert.Contains(field, exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<programme", output.ToString(), StringComparison.Ordinal);
    }
}
