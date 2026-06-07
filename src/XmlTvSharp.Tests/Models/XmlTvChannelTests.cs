namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvChannelTests
{
    [Fact]
    public void Constructor_SeedsRequiredDisplayName()
    {
        var channel = new XmlTvChannel("bbc-one", new XmlTvLocalizedText("BBC One", "en"));

        Assert.Equal("bbc-one", channel.Id);
        Assert.Equal(new XmlTvLocalizedText("BBC One", "en"), Assert.Single(channel.DisplayNames));
        Assert.Empty(channel.Icons);
        Assert.Empty(channel.Urls);
    }

    [Fact]
    public void Constructor_PreservesDisplayNameCollection()
    {
        var first = new XmlTvLocalizedText("BBC One", "en");
        var second = new XmlTvLocalizedText("BBC 1", "en-GB");

        var channel = new XmlTvChannel("bbc-one", new[] { first, second });

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
