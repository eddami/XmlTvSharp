namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvLocalizedTextTests
{
    [Fact]
    public void ValueEquality_PreservesLanguage()
    {
        var first = new XmlTvLocalizedText("News", "en");
        var second = new XmlTvLocalizedText("News", "en");

        Assert.Equal(first, second);
    }
}
