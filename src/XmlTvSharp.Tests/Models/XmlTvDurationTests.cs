namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvDurationTests
{
    [Fact]
    public void FromMinutes_PreservesUnitAndConvertsExactly()
    {
        var duration = XmlTvDuration.FromMinutes(1.5m);

        Assert.Equal(1.5m, duration.Value);
        Assert.Equal(XmlTvDurationUnit.Minutes, duration.Unit);
        Assert.Equal(TimeSpan.FromSeconds(90), duration.ToTimeSpan());
    }

    [Fact]
    public void Default_IsValidZeroSeconds()
    {
        var duration = default(XmlTvDuration);

        Assert.Equal(XmlTvDurationUnit.Seconds, duration.Unit);
        Assert.Equal(TimeSpan.Zero, duration.ToTimeSpan());
    }

    [Theory]
    [InlineData(XmlTvDurationUnit.Seconds, 90)]
    [InlineData(XmlTvDurationUnit.Minutes, 1.5)]
    [InlineData(XmlTvDurationUnit.Hours, 0.025)]
    public void FromTimeSpan_UsesRequestedUnit(XmlTvDurationUnit unit, double expectedValue)
    {
        var duration = XmlTvDuration.FromTimeSpan(TimeSpan.FromSeconds(90), unit);

        Assert.Equal((decimal)expectedValue, duration.Value);
        Assert.Equal(unit, duration.Unit);
        Assert.Equal(TimeSpan.FromSeconds(90), duration.ToTimeSpan());
    }

    [Fact]
    public void ToTimeSpan_FractionalTick_RoundsAwayFromZero()
    {
        var duration = new XmlTvDuration(0.00000005m, XmlTvDurationUnit.Seconds);

        Assert.Equal(TimeSpan.FromTicks(1), duration.ToTimeSpan());
    }

    [Theory]
    [InlineData(XmlTvDurationUnit.Seconds)]
    [InlineData(XmlTvDurationUnit.Minutes)]
    [InlineData(XmlTvDurationUnit.Hours)]
    public void Constructor_NegativeValue_Throws(XmlTvDurationUnit unit)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new XmlTvDuration(-1, unit));
    }

    [Fact]
    public void Constructor_InvalidUnit_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new XmlTvDuration(1, (XmlTvDurationUnit)100));
    }

    [Fact]
    public void FromTimeSpan_NegativeValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            XmlTvDuration.FromTimeSpan(TimeSpan.FromSeconds(-1), XmlTvDurationUnit.Seconds));
    }

    [Fact]
    public void ToTimeSpan_Overflow_Throws()
    {
        var duration = XmlTvDuration.FromHours(decimal.MaxValue);

        Assert.Throws<OverflowException>(() => duration.ToTimeSpan());
    }
}
