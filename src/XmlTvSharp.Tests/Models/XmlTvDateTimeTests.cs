namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvDateTimeTests
{
    public static TheoryData<int?, int?, int?, int?, int?> PrecisionGaps => new()
    {
        { null, 5, null, null, null },
        { 6, null, 14, null, null },
        { 6, 5, null, 30, null },
        { 6, 5, 14, null, 0 }
    };

    [Theory]
    [InlineData("2026", XmlTvDateTimePrecision.Year)]
    [InlineData("202606", XmlTvDateTimePrecision.Month)]
    [InlineData("20260605", XmlTvDateTimePrecision.Day)]
    [InlineData("2026060514", XmlTvDateTimePrecision.Hour)]
    [InlineData("202606051430", XmlTvDateTimePrecision.Minute)]
    [InlineData("20260605143000", XmlTvDateTimePrecision.Second)]
    [InlineData("20260605143000 +0100", XmlTvDateTimePrecision.Second)]
    [InlineData("20260605143000 BST", XmlTvDateTimePrecision.Second)]
    public void Parse_ValidValue_PreservesPrecisionAndRoundTrips(
        string source,
        XmlTvDateTimePrecision expectedPrecision)
    {
        var value = XmlTvDateTime.Parse(source);

        Assert.Equal(expectedPrecision, value.Precision);
        Assert.Equal(source, value.ToXmlTvString());
    }

    [Theory]
    [InlineData("00010101000000 +0000")]
    [InlineData("20000229000000 +1400")]
    [InlineData("99991231235959 -1400")]
    public void Parse_BoundaryValue_RoundTrips(string source)
    {
        Assert.Equal(source, XmlTvDateTime.Parse(source).ToXmlTvString());
    }

    [Fact]
    public void Parse_SurroundingAndSeparatorWhitespace_ProducesCanonicalString()
    {
        var value = XmlTvDateTime.Parse(" \t20260605143000\t +0100 \r\n");

        Assert.Equal("20260605143000 +0100", value.ToXmlTvString());
    }

    [Fact]
    public void Parse_PartialValueWithNamedZone_PreservesBoth()
    {
        var value = XmlTvDateTime.Parse("202606 BST");

        Assert.Equal(XmlTvDateTimePrecision.Month, value.Precision);
        Assert.Equal(new XmlTvTimeZone("BST"), value.Zone);
        Assert.Equal("202606 BST", value.ToXmlTvString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("202")]
    [InlineData("20260A")]
    [InlineData("0000")]
    [InlineData("202600")]
    [InlineData("202613")]
    [InlineData("20260200")]
    [InlineData("20260230")]
    [InlineData("20230229")]
    [InlineData("20260605240000")]
    [InlineData("20260605146000")]
    [InlineData("20260605143060")]
    [InlineData("20260605143000 +1500")]
    [InlineData("20260605143000 +1460")]
    [InlineData("20260605143000 BAD TOKEN")]
    public void TryParse_InvalidValue_ReturnsFalse(string source)
    {
        Assert.False(XmlTvDateTime.TryParse(source, out var value));
        Assert.Null(value);
    }

    [Fact]
    public void TryParse_ValidValue_ReturnsParsedValue()
    {
        Assert.True(XmlTvDateTime.TryParse("20260605", out var value));
        Assert.Equal(XmlTvDateTimePrecision.Day, value!.Precision);
    }

    [Theory]
    [MemberData(nameof(PrecisionGaps))]
    public void Constructor_PrecisionGap_Throws(
        int? month,
        int? day,
        int? hour,
        int? minute,
        int? second)
    {
        Assert.Throws<ArgumentException>(() => new XmlTvDateTime(2026, month, day, hour, minute, second));
    }

    [Fact]
    public void ToDateTimeOffset_SecondPrecisionWithoutZone_AssumesUtc()
    {
        var value = XmlTvDateTime.Parse("20260605143000");

        var instant = value.ToDateTimeOffset();

        Assert.Equal(TimeSpan.Zero, instant.Offset);
        Assert.Equal(new DateTimeOffset(2026, 6, 5, 14, 30, 0, TimeSpan.Zero), instant);
    }

    [Fact]
    public void ToDateTimeOffset_PartialValue_Throws()
    {
        var value = XmlTvDateTime.Parse("20260605");

        Assert.Throws<InvalidOperationException>(() => value.ToDateTimeOffset());
    }

    [Fact]
    public void ToDateTimeOffset_NumericZone_UsesParsedOffset()
    {
        var value = XmlTvDateTime.Parse("20260605143000 -0430");

        var instant = value.ToDateTimeOffset();

        Assert.Equal(new DateTimeOffset(2026, 6, 5, 14, 30, 0, TimeSpan.FromMinutes(-270)), instant);
    }

    [Fact]
    public void ToDateTimeOffset_ColonizedNumericZone_PreservesTokenAndUsesParsedOffset()
    {
        var value = XmlTvDateTime.Parse("20260605143000 +09:30");

        var instant = value.ToDateTimeOffset();

        Assert.Equal("20260605143000 +09:30", value.ToXmlTvString());
        Assert.Equal(new DateTimeOffset(2026, 6, 5, 14, 30, 0, TimeSpan.FromMinutes(570)), instant);
    }

    [Fact]
    public void ToDateTimeOffset_NamedZoneWithoutResolver_Throws()
    {
        var value = XmlTvDateTime.Parse("20260605143000 BST");

        Assert.Throws<InvalidOperationException>(() => value.ToDateTimeOffset());
    }

    [Fact]
    public void ToDateTimeOffset_NamedZone_UsesExplicitResolver()
    {
        var value = XmlTvDateTime.Parse("20260605143000 BST");

        var instant = value.ToDateTimeOffset((token, _, out offset) =>
        {
            offset = TimeSpan.FromHours(1);
            return token == "BST";
        });

        Assert.Equal(TimeSpan.FromHours(1), instant.Offset);
    }

    [Fact]
    public void ToDateTimeOffset_ResolverReceivesTokenAndValue()
    {
        var value = XmlTvDateTime.Parse("20260605143000 BST");
        string? receivedToken = null;
        XmlTvDateTime? receivedValue = null;

        value.ToDateTimeOffset((token, candidate, out offset) =>
        {
            receivedToken = token;
            receivedValue = candidate;
            offset = TimeSpan.Zero;
            return true;
        });

        Assert.Equal("BST", receivedToken);
        Assert.Same(value, receivedValue);
    }

    [Fact]
    public void ToDateTimeOffset_UnresolvedNamedZone_Throws()
    {
        var value = XmlTvDateTime.Parse("20260605143000 BST");

        Assert.Throws<InvalidOperationException>(() => value.ToDateTimeOffset((_, _, out offset) =>
        {
            offset = default;
            return false;
        }));
    }

    [Theory]
    [InlineData(15, 0, 0)]
    [InlineData(1, 0, 1)]
    public void ToDateTimeOffset_InvalidResolvedOffset_Throws(int hours, int minutes, int seconds)
    {
        var value = XmlTvDateTime.Parse("20260605143000 BST");

        Assert.ThrowsAny<ArgumentException>(() => value.ToDateTimeOffset((_, _, out offset) =>
        {
            offset = new TimeSpan(hours, minutes, seconds);
            return true;
        }));
    }

    [Fact]
    public void ToDateTimeOffset_NullResolver_Throws()
    {
        var value = XmlTvDateTime.Parse("20260605143000 BST");

        Assert.Throws<ArgumentNullException>(() => value.ToDateTimeOffset(null!));
    }

    [Fact]
    public void FromDateTimeOffset_PreservesExactInstantAndOffset()
    {
        var instant = new DateTimeOffset(2026, 6, 5, 14, 30, 0, TimeSpan.FromHours(-4));

        var value = XmlTvDateTime.FromDateTimeOffset(instant);

        Assert.Equal("20260605143000 -0400", value.ToXmlTvString());
        Assert.Equal(instant, value.ToDateTimeOffset());
    }
}
