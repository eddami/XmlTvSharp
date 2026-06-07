namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvCreditTests
{
    [Fact]
    public void EmptyRoles_RemainRepresentable()
    {
        var credits = new XmlTvCredits();

        credits.Directors.Add(new XmlTvCredit());
        credits.Actors.Add(new XmlTvActorCredit());

        Assert.Empty(Assert.Single(credits.Directors).Content);
        Assert.Empty(Assert.Single(credits.Actors).Content);
    }

    [Fact]
    public void MixedContent_PreservesOrderAndProvidesProjections()
    {
        var credit = new XmlTvCredit("Jane");
        var image = new XmlTvImage("jane.jpg", XmlTvImageType.Person);
        var url = new XmlTvUrl("https://example.test/jane");

        credit.Content.Add(new XmlTvCreditImage(image));
        credit.Content.Add(new XmlTvCreditText(" Doe"));
        credit.Content.Add(new XmlTvCreditUrl(url));

        Assert.Equal("Jane Doe", credit.Text);
        Assert.Equal(image, Assert.Single(credit.Images));
        Assert.Equal(url, Assert.Single(credit.Urls));
        Assert.Collection(
            credit.Content,
            item => Assert.IsType<XmlTvCreditText>(item),
            item => Assert.IsType<XmlTvCreditImage>(item),
            item => Assert.IsType<XmlTvCreditText>(item),
            item => Assert.IsType<XmlTvCreditUrl>(item));
    }
}
