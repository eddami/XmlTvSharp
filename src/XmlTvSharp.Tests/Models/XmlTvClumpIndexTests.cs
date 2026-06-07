namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvClumpIndexTests
{
    [Theory]
    [InlineData("0/1", 0, 1)]
    [InlineData("2/5", 2, 5)]
    public void Parse_ValidValue_RoundTrips(string source, int index, int count)
    {
        var clumpIndex = XmlTvClumpIndex.Parse(source);

        Assert.Equal(index, clumpIndex.Index);
        Assert.Equal(count, clumpIndex.Count);
        Assert.Equal(source, clumpIndex.ToXmlTvString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("/1")]
    [InlineData("1")]
    [InlineData("1/")]
    [InlineData("1/1")]
    [InlineData("1/2/3")]
    [InlineData("-1/2")]
    [InlineData("2147483648/2147483649")]
    [InlineData("2/1")]
    public void TryParse_InvalidValue_ReturnsFalse(string source)
    {
        Assert.False(XmlTvClumpIndex.TryParse(source, out var value));
        Assert.Null(value);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void Constructor_InvalidPositionOrCount_Throws(int index, int count)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new XmlTvClumpIndex(index, count));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1/1")]
    public void Parse_InvalidValue_ThrowsFormatException(string source)
    {
        Assert.Throws<FormatException>(() => XmlTvClumpIndex.Parse(source));
    }

    [Fact]
    public void Parse_EmptyValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => XmlTvClumpIndex.Parse(""));
    }
}
