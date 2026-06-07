namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvIconTests
{
    [Fact]
    public void Constructor_InvalidDimension_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new XmlTvIcon("icon.png", 0));
    }
}
