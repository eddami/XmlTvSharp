namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvTimeZoneTests
{
    [Theory]
    [InlineData("+0000", 0)]
    [InlineData("+0130", 90)]
    [InlineData("+01:30", 90)]
    [InlineData("-0400", -240)]
    [InlineData("-04:00", -240)]
    [InlineData("+1400", 840)]
    [InlineData("+14:00", 840)]
    [InlineData("-1400", -840)]
    [InlineData("-14:00", -840)]
    public void Constructor_NumericOffset_Parses(string source, int expectedMinutes)
    {
        var zone = new XmlTvTimeZone(source);

        Assert.True(zone.IsNumericOffset);
        Assert.Equal(TimeSpan.FromMinutes(expectedMinutes), zone.UtcOffset);
        Assert.Equal(source, zone.Value);
    }

    [Fact]
    public void Constructor_NamedToken_RemainsUnresolved()
    {
        var zone = new XmlTvTimeZone("BST");

        Assert.False(zone.IsNumericOffset);
        Assert.Null(zone.UtcOffset);
        Assert.Equal("BST", zone.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("B ST")]
    [InlineData("+")]
    [InlineData("+010")]
    [InlineData("+01000")]
    [InlineData("+01:3")]
    [InlineData("+1:30")]
    [InlineData("+013:0")]
    [InlineData("+01:60")]
    [InlineData("+1460")]
    [InlineData("+14:01")]
    [InlineData("-2500")]
    [InlineData("+ABCD")]
    public void Constructor_MalformedNumericToken_Throws(string source)
    {
        Assert.ThrowsAny<ArgumentException>(() => new XmlTvTimeZone(source));
    }

    [Theory]
    [InlineData(0, "+0000")]
    [InlineData(90, "+0130")]
    [InlineData(-240, "-0400")]
    [InlineData(840, "+1400")]
    [InlineData(-840, "-1400")]
    public void FromUtcOffset_ValidOffset_FormatsCanonicalToken(int minutes, string expected)
    {
        var zone = XmlTvTimeZone.FromUtcOffset(TimeSpan.FromMinutes(minutes));

        Assert.Equal(expected, zone.Value);
        Assert.Equal(TimeSpan.FromMinutes(minutes), zone.UtcOffset);
    }

    [Theory]
    [InlineData(841)]
    [InlineData(-841)]
    public void FromUtcOffset_OutOfRange_Throws(int minutes)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => XmlTvTimeZone.FromUtcOffset(TimeSpan.FromMinutes(minutes)));
    }

    [Fact]
    public void FromUtcOffset_FractionalMinute_Throws()
    {
        Assert.Throws<ArgumentException>(() => XmlTvTimeZone.FromUtcOffset(TimeSpan.FromSeconds(1)));
    }
}
