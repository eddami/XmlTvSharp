namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvChannelTests
{
    [Fact]
    public void Constructor_SeedsRequiredDisplayName()
    {
        var channel = new XmlTvChannel("channel-one", new XmlTvLocalizedText("Channel One", "en"));

        Assert.Equal("channel-one", channel.Id);
        Assert.Equal(new XmlTvLocalizedText("Channel One", "en"), Assert.Single(channel.DisplayNames));
        Assert.Empty(channel.Icons);
        Assert.Empty(channel.Urls);
    }

    [Fact]
    public void Constructor_PreservesDisplayNameCollection()
    {
        var first = new XmlTvLocalizedText("Channel One", "en");
        var second = new XmlTvLocalizedText("Channel 1", "en-GB");

        var channel = new XmlTvChannel("channel-one", new[] { first, second });

        Assert.Equal(new[] { first, second }, channel.DisplayNames);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidId_Throws(string id)
    {
        Assert.Throws<ArgumentException>(() => new XmlTvChannel(id, "Name"));
    }

    [Fact]
    public void Constructor_NullDisplayName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new XmlTvChannel("channel", (XmlTvLocalizedText)null!));
    }

    [Fact]
    public void Constructor_EmptyDisplayNameCollection_Throws()
    {
        Assert.Throws<ArgumentException>(() => new XmlTvChannel("channel", Array.Empty<XmlTvLocalizedText>()));
    }
}
