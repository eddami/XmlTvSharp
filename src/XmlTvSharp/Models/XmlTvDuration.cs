namespace XmlTvSharp.Models;

/// <summary>
///     Represents the non-negative value and unit of an XMLTV <c>length</c> element.
/// </summary>
public readonly record struct XmlTvDuration
{
    /// <summary>
    ///     Initializes an XMLTV duration.
    /// </summary>
    /// <param name="value">The non-negative duration value.</param>
    /// <param name="unit">The duration unit.</param>
    public XmlTvDuration(decimal value, XmlTvDurationUnit unit)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        if (!Enum.IsDefined(typeof(XmlTvDurationUnit), unit))
        {
            throw new ArgumentOutOfRangeException(nameof(unit));
        }

        Value = value;
        Unit = unit;
    }

    /// <summary>Gets the duration value in <see cref="Unit" />.</summary>
    public decimal Value { get; }

    /// <summary>Gets the duration unit.</summary>
    public XmlTvDurationUnit Unit { get; }

    /// <summary>Converts this duration to a <see cref="TimeSpan" />, rounding fractional ticks away from zero.</summary>
    /// <returns>The converted duration.</returns>
    public TimeSpan ToTimeSpan()
    {
        var ticksPerUnit = Unit switch
        {
            XmlTvDurationUnit.Seconds => TimeSpan.TicksPerSecond,
            XmlTvDurationUnit.Minutes => TimeSpan.TicksPerMinute,
            XmlTvDurationUnit.Hours => TimeSpan.TicksPerHour,
            _ => throw new InvalidOperationException("Duration has an invalid unit.")
        };

        var ticks = decimal.Round(Value * ticksPerUnit, 0, MidpointRounding.AwayFromZero);
        if (ticks > TimeSpan.MaxValue.Ticks)
        {
            throw new OverflowException("Duration exceeds TimeSpan.MaxValue.");
        }

        return TimeSpan.FromTicks(decimal.ToInt64(ticks));
    }

    /// <summary>Creates an XMLTV duration from a <see cref="TimeSpan" /> using the requested unit.</summary>
    /// <param name="value">The non-negative duration.</param>
    /// <param name="unit">The unit in which to represent the duration.</param>
    /// <returns>The XMLTV duration.</returns>
    public static XmlTvDuration FromTimeSpan(TimeSpan value, XmlTvDurationUnit unit)
    {
        if (value < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        var ticksPerUnit = unit switch
        {
            XmlTvDurationUnit.Seconds => TimeSpan.TicksPerSecond,
            XmlTvDurationUnit.Minutes => TimeSpan.TicksPerMinute,
            XmlTvDurationUnit.Hours => TimeSpan.TicksPerHour,
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };

        return new XmlTvDuration((decimal)value.Ticks / ticksPerUnit, unit);
    }

    /// <summary>Creates a duration expressed in seconds.</summary>
    /// <param name="value">The non-negative number of seconds.</param>
    /// <returns>The XMLTV duration.</returns>
    public static XmlTvDuration FromSeconds(decimal value)
    {
        return new XmlTvDuration(value, XmlTvDurationUnit.Seconds);
    }

    /// <summary>Creates a duration expressed in minutes.</summary>
    /// <param name="value">The non-negative number of minutes.</param>
    /// <returns>The XMLTV duration.</returns>
    public static XmlTvDuration FromMinutes(decimal value)
    {
        return new XmlTvDuration(value, XmlTvDurationUnit.Minutes);
    }

    /// <summary>Creates a duration expressed in hours.</summary>
    /// <param name="value">The non-negative number of hours.</param>
    /// <returns>The XMLTV duration.</returns>
    public static XmlTvDuration FromHours(decimal value)
    {
        return new XmlTvDuration(value, XmlTvDurationUnit.Hours);
    }
}
