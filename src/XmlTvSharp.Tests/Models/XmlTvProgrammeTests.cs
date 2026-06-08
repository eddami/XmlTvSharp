namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvProgrammeTests
{
    private static readonly DateTimeOffset Start = new(2026, 6, 5, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_SeedsRequiredDataAndCollections()
    {
        var start = XmlTvDateTime.FromDateTimeOffset(Start);

        var programme = new XmlTvProgramme(start, "channel-one", "News");

        Assert.Same(start, programme.Start);
        Assert.Equal("channel-one", programme.ChannelId);
        Assert.Equal(new XmlTvLocalizedText("News"), Assert.Single(programme.Titles));
        Assert.Null(programme.Stop);
        Assert.Equal(new XmlTvClumpIndex(0, 1), programme.EffectiveClumpIndex);
        Assert.Empty(programme.Categories);
        Assert.Empty(programme.Ratings);
        Assert.Empty(programme.Images);
        Assert.NotNull(programme.Extensions);
    }

    [Fact]
    public void Constructor_PreservesTitleCollection()
    {
        var first = new XmlTvLocalizedText("News", "en");
        var second = new XmlTvLocalizedText("Nouvelles", "fr");

        var programme = new XmlTvProgramme(Start, "channel", new[] { first, second });

        Assert.Equal(new[] { first, second }, programme.Titles);
    }

    [Fact]
    public void EffectiveClumpIndex_ExplicitValueOverridesDefault()
    {
        var programme = new XmlTvProgramme(Start, "channel", "Title")
        {
            ClumpIndex = new XmlTvClumpIndex(1, 3)
        };

        Assert.Equal(new XmlTvClumpIndex(1, 3), programme.EffectiveClumpIndex);
    }

    [Fact]
    public void Collections_PreserveDuplicatesAndOrder()
    {
        var programme = new XmlTvProgramme(Start, "channel", "Title");
        var first = new XmlTvLocalizedText("Drama", "en");
        var second = new XmlTvLocalizedText("Drama", "en");

        programme.Categories.Add(first);
        programme.Categories.Add(second);

        Assert.Equal(new[] { first, second }, programme.Categories);
    }

    [Fact]
    public void Extensions_JellyfinIsOptionalAndTyped()
    {
        var programme = new XmlTvProgramme(Start, "channel", "Title");

        Assert.Null(programme.Extensions.Jellyfin);

        programme.Extensions.Jellyfin = new XmlTvJellyfinProgrammeExtensions { IsLive = true };

        Assert.True(programme.Extensions.Jellyfin.IsLive);
    }

    [Fact]
    public void Constructor_NullStart_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new XmlTvProgramme(null!, "channel", "Title"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidChannelId_Throws(string channelId)
    {
        Assert.Throws<ArgumentException>(() => new XmlTvProgramme(Start, channelId, "Title"));
    }

    [Fact]
    public void Constructor_NullTitle_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new XmlTvProgramme(XmlTvDateTime.FromDateTimeOffset(Start), "channel", (XmlTvLocalizedText)null!));
    }

    [Fact]
    public void Constructor_EmptyTitleCollection_Throws()
    {
        Assert.Throws<ArgumentException>(() => new XmlTvProgramme(Start, "channel", Array.Empty<XmlTvLocalizedText>()));
    }
}
