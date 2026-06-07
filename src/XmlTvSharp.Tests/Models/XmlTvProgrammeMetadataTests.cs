namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvProgrammeMetadataTests
{
    [Fact]
    public void Rating_PreservesSystemAndOwnsInitializedIcons()
    {
        var rating = new XmlTvRating("PG-13", "MPAA");

        rating.Icons.Add(new XmlTvIcon("rating.png"));

        Assert.Equal("PG-13", rating.Value);
        Assert.Equal("MPAA", rating.System);
        Assert.Single(rating.Icons);
    }

    [Fact]
    public void StarRating_PreservesSourceText()
    {
        var rating = new XmlTvStarRating("4 / 5");

        Assert.Equal("4 / 5", rating.Value);
    }

    [Fact]
    public void PreviouslyShown_PresenceCanHaveNoAttributes()
    {
        var previouslyShown = new XmlTvPreviouslyShown();

        Assert.Null(previouslyShown.Start);
        Assert.Null(previouslyShown.ChannelId);
    }

    [Fact]
    public void Subtitles_InvalidEnum_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new XmlTvSubtitles((XmlTvSubtitleType)100));
    }

    [Fact]
    public void Review_PreservesTypedMetadata()
    {
        var review = new XmlTvReview(
            "https://example.test/review",
            XmlTvReviewType.Url,
            "Example",
            "Reviewer",
            "en");

        Assert.Equal(XmlTvReviewType.Url, review.Type);
        Assert.Equal("Example", review.Source);
        Assert.Equal("Reviewer", review.Reviewer);
        Assert.Equal("en", review.Language);
    }

    [Fact]
    public void Image_PreservesClosedEnumsAndOpenSystem()
    {
        var image = new XmlTvImage(
            "poster.jpg",
            XmlTvImageType.Poster,
            XmlTvImageSize.Large,
            XmlTvImageOrientation.Portrait,
            "provider");

        Assert.Equal(XmlTvImageType.Poster, image.Type);
        Assert.Equal(XmlTvImageSize.Large, image.Size);
        Assert.Equal(XmlTvImageOrientation.Portrait, image.Orientation);
        Assert.Equal("provider", image.System);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Image_InvalidClosedEnum_Throws(int property)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new XmlTvImage(
            "poster.jpg",
            property == 0 ? (XmlTvImageType)100 : null,
            property == 1 ? (XmlTvImageSize)100 : null,
            property == 2 ? (XmlTvImageOrientation)100 : null));
    }

    [Fact]
    public void Review_InvalidType_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new XmlTvReview("review", (XmlTvReviewType)100));
    }

    [Fact]
    public void VideoAndAudio_PreserveOmittedVersusExplicitFalse()
    {
        var video = new XmlTvVideo(false);
        var audio = new XmlTvAudio();

        Assert.False(video.IsPresent);
        Assert.Null(video.IsColour);
        Assert.Null(audio.IsPresent);
    }
}
