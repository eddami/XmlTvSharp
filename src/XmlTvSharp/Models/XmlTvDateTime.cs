using System.Text;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV date/time value while preserving its original precision and timezone token.
/// </summary>
public sealed record class XmlTvDateTime
{
    /// <summary>
    ///     Initializes an XMLTV date/time from contiguous components.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The optional month.</param>
    /// <param name="day">The optional day of the month.</param>
    /// <param name="hour">The optional hour.</param>
    /// <param name="minute">The optional minute.</param>
    /// <param name="second">The optional second.</param>
    /// <param name="zone">The optional numeric or named timezone token.</param>
    /// <remarks>Components cannot contain precision gaps; each supplied component requires all preceding components.</remarks>
    public XmlTvDateTime(
        int year,
        int? month = null,
        int? day = null,
        int? hour = null,
        int? minute = null,
        int? second = null,
        XmlTvTimeZone? zone = null)
    {
        ValidateComponents(year, month, day, hour, minute, second);

        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
        Zone = zone;
        Precision = GetPrecision(month, day, hour, minute, second);
    }

    /// <summary>Gets the year.</summary>
    public int Year { get; }

    /// <summary>Gets the month, or <see langword="null" /> when the value has year precision.</summary>
    public int? Month { get; }

    /// <summary>Gets the day, or <see langword="null" /> when the value has month or lower precision.</summary>
    public int? Day { get; }

    /// <summary>Gets the hour, or <see langword="null" /> when the value has day or lower precision.</summary>
    public int? Hour { get; }

    /// <summary>Gets the minute, or <see langword="null" /> when the value has hour or lower precision.</summary>
    public int? Minute { get; }

    /// <summary>Gets the second, or <see langword="null" /> when the value has minute or lower precision.</summary>
    public int? Second { get; }

    /// <summary>Gets the optional numeric or named timezone token.</summary>
    public XmlTvTimeZone? Zone { get; }

    /// <summary>Gets the least-significant component present in the value.</summary>
    public XmlTvDateTimePrecision Precision { get; }

    /// <summary>Parses an XMLTV date/time string.</summary>
    /// <param name="value">The XMLTV date/time string.</param>
    /// <returns>The parsed value.</returns>
    /// <exception cref="FormatException"><paramref name="value" /> is not a valid XMLTV date/time.</exception>
    public static XmlTvDateTime Parse(string value)
    {
        XmlTvArgument.NotWhiteSpace(value, nameof(value));

        var source = value.AsSpan().Trim();
        var separator = IndexOfWhiteSpace(source);
        var componentText = separator < 0 ? source : source.Slice(0, separator);
        var zoneText = separator < 0 ? ReadOnlySpan<char>.Empty : source.Slice(separator).Trim();

        if (componentText.Length is not (4 or 6 or 8 or 10 or 12 or 14) ||
            !HasOnlyDigits(componentText) ||
            (separator >= 0 && zoneText.Length == 0))
        {
            throw new FormatException("Value is not a valid XMLTV date/time.");
        }

        try
        {
            return new XmlTvDateTime(
                ParseComponent(componentText.Slice(0, 4)),
                ParseOptionalComponent(componentText, 4),
                ParseOptionalComponent(componentText, 6),
                ParseOptionalComponent(componentText, 8),
                ParseOptionalComponent(componentText, 10),
                ParseOptionalComponent(componentText, 12),
                CreateTimeZone(zoneText));
        }
        catch (ArgumentException exception)
        {
            throw new FormatException("Value is not a valid XMLTV date/time.", exception);
        }
    }

    /// <summary>Attempts to parse an XMLTV date/time string.</summary>
    /// <param name="value">The XMLTV date/time string.</param>
    /// <param name="result">The parsed value when parsing succeeds; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when parsing succeeds; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(string value, out XmlTvDateTime? result)
    {
        try
        {
            result = Parse(value);
            return true;
        }
        catch (ArgumentException)
        {
            result = null;
            return false;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }

    /// <summary>Creates a second-precision XMLTV date/time with a numeric timezone offset.</summary>
    /// <param name="value">The exact date/time value.</param>
    /// <returns>The equivalent XMLTV date/time.</returns>
    public static XmlTvDateTime FromDateTimeOffset(DateTimeOffset value)
    {
        return new XmlTvDateTime(
            value.Year,
            value.Month,
            value.Day,
            value.Hour,
            value.Minute,
            value.Second,
            XmlTvTimeZone.FromUtcOffset(value.Offset));
    }

    /// <summary>Converts a second-precision value with a numeric or omitted timezone to <see cref="DateTimeOffset" />.</summary>
    /// <returns>The exact date/time value. An omitted timezone is interpreted as UTC.</returns>
    /// <exception cref="InvalidOperationException">The value lacks second precision or contains a named timezone.</exception>
    public DateTimeOffset ToDateTimeOffset()
    {
        return ToDateTimeOffsetCore(null);
    }

    /// <summary>
    ///     Converts a second-precision value to <see cref="DateTimeOffset" />, resolving named timezone tokens when
    ///     needed.
    /// </summary>
    /// <param name="resolver">A resolver for named timezone tokens.</param>
    /// <returns>The exact date/time value. An omitted timezone is interpreted as UTC.</returns>
    /// <exception cref="InvalidOperationException">The value lacks second precision or its named timezone cannot be resolved.</exception>
    public DateTimeOffset ToDateTimeOffset(XmlTvTimeZoneResolver resolver)
    {
        if (resolver is null)
        {
            throw new ArgumentNullException(nameof(resolver));
        }

        return ToDateTimeOffsetCore(resolver);
    }

    /// <summary>Formats the value using XMLTV's contiguous-component date/time syntax.</summary>
    /// <returns>The XMLTV date/time string.</returns>
    public string ToXmlTvString()
    {
#if NET8_0_OR_GREATER
        var componentLength = Precision switch
        {
            XmlTvDateTimePrecision.Year => 4,
            XmlTvDateTimePrecision.Month => 6,
            XmlTvDateTimePrecision.Day => 8,
            XmlTvDateTimePrecision.Hour => 10,
            XmlTvDateTimePrecision.Minute => 12,
            XmlTvDateTimePrecision.Second => 14,
            _ => throw new InvalidOperationException("Date/time precision is invalid.")
        };
        var zone = Zone?.Value;
        var length = componentLength + (zone is null ? 0 : zone.Length + 1);

        return string.Create(length, this, static (target, value) =>
        {
            var index = 0;
            WriteFourDigits(target, ref index, value.Year);
            WriteOptionalDigits(target, ref index, value.Month);
            WriteOptionalDigits(target, ref index, value.Day);
            WriteOptionalDigits(target, ref index, value.Hour);
            WriteOptionalDigits(target, ref index, value.Minute);
            WriteOptionalDigits(target, ref index, value.Second);

            if (value.Zone is not null)
            {
                target[index++] = ' ';
                value.Zone.Value.AsSpan().CopyTo(target[index..]);
            }
        });
#else
        var value = new StringBuilder(20);
        AppendFourDigits(value, Year);
        Append(value, Month);
        Append(value, Day);
        Append(value, Hour);
        Append(value, Minute);
        Append(value, Second);

        if (Zone is not null)
        {
            value.Append(' ');
            value.Append(Zone.Value);
        }

        return value.ToString();
#endif
    }

    private DateTimeOffset ToDateTimeOffsetCore(XmlTvTimeZoneResolver? resolver)
    {
        if (Precision != XmlTvDateTimePrecision.Second)
        {
            throw new InvalidOperationException(
                "Only second-precision XMLTV date/time values represent exact instants.");
        }

        TimeSpan offset;
        if (Zone is null)
        {
            offset = TimeSpan.Zero;
        }
        else if (Zone.UtcOffset is { } numericOffset)
        {
            offset = numericOffset;
        }
        else if (resolver is not null && resolver(Zone.Value, this, out offset))
        {
            XmlTvTimeZone.ValidateOffset(offset, nameof(resolver));
        }
        else
        {
            throw new InvalidOperationException($"Timezone token '{Zone.Value}' requires an explicit resolver.");
        }

        return new DateTimeOffset(
            Year,
            Month!.Value,
            Day!.Value,
            Hour!.Value,
            Minute!.Value,
            Second!.Value,
            offset);
    }

    private static void ValidateComponents(
        int year,
        int? month,
        int? day,
        int? hour,
        int? minute,
        int? second)
    {
        if (year is < 1 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        RequirePrevious(month, day, nameof(day));
        RequirePrevious(day, hour, nameof(hour));
        RequirePrevious(hour, minute, nameof(minute));
        RequirePrevious(minute, second, nameof(second));

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        if (day is not null && day > DateTime.DaysInMonth(year, month!.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(day));
        }

        if (day is < 1 || hour is < 0 or > 23 || minute is < 0 or > 59 || second is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException("A date/time component is outside its valid range.");
        }
    }

    private static void RequirePrevious(int? previous, int? current, string parameterName)
    {
        if (current is not null && previous is null)
        {
            throw new ArgumentException("Date/time precision cannot contain gaps.", parameterName);
        }
    }

    private static XmlTvDateTimePrecision GetPrecision(
        int? month,
        int? day,
        int? hour,
        int? minute,
        int? second)
    {
        if (second is not null) return XmlTvDateTimePrecision.Second;
        if (minute is not null) return XmlTvDateTimePrecision.Minute;
        if (hour is not null) return XmlTvDateTimePrecision.Hour;
        if (day is not null) return XmlTvDateTimePrecision.Day;
        if (month is not null) return XmlTvDateTimePrecision.Month;
        return XmlTvDateTimePrecision.Year;
    }

#if NET8_0_OR_GREATER
    private static void WriteOptionalDigits(Span<char> target, ref int index, int? component)
    {
        if (component is not null)
        {
            WriteTwoDigits(target, ref index, component.Value);
        }
    }

    private static void WriteFourDigits(Span<char> target, ref int index, int value)
    {
        target[index++] = (char)('0' + value / 1000 % 10);
        target[index++] = (char)('0' + value / 100 % 10);
        target[index++] = (char)('0' + value / 10 % 10);
        target[index++] = (char)('0' + value % 10);
    }

    private static void WriteTwoDigits(Span<char> target, ref int index, int value)
    {
        target[index++] = (char)('0' + value / 10);
        target[index++] = (char)('0' + value % 10);
    }
#endif

    private static int IndexOfWhiteSpace(ReadOnlySpan<char> value)
    {
        for (var index = 0; index < value.Length; index++)
        {
            if (char.IsWhiteSpace(value[index]))
            {
                return index;
            }
        }

        return -1;
    }

    private static bool HasOnlyDigits(ReadOnlySpan<char> value)
    {
        for (var index = 0; index < value.Length; index++)
        {
            if (value[index] is < '0' or > '9')
            {
                return false;
            }
        }

        return true;
    }

    private static int ParseComponent(ReadOnlySpan<char> value)
    {
        var result = 0;
        for (var index = 0; index < value.Length; index++)
        {
            result = result * 10 + value[index] - '0';
        }

        return result;
    }

    private static int? ParseOptionalComponent(ReadOnlySpan<char> value, int componentOffset)
    {
        return value.Length > componentOffset ? ParseComponent(value.Slice(componentOffset, 2)) : null;
    }

    private static XmlTvTimeZone? CreateTimeZone(ReadOnlySpan<char> token)
    {
        if (token.Length == 0)
        {
            return null;
        }

        var value = token.ToString();
        return XmlTvTimeZone.TryParseNumericOffset(token, out var offset)
            ? XmlTvTimeZone.CreateNumeric(value, offset)
            : new XmlTvTimeZone(value);
    }

    private static void Append(StringBuilder target, int? component)
    {
        if (component is not null)
        {
            AppendTwoDigits(target, component.Value);
        }
    }

    private static void AppendFourDigits(StringBuilder target, int value)
    {
        target.Append((char)('0' + value / 1000 % 10));
        target.Append((char)('0' + value / 100 % 10));
        target.Append((char)('0' + value / 10 % 10));
        target.Append((char)('0' + value % 10));
    }

    private static void AppendTwoDigits(StringBuilder target, int value)
    {
        target.Append((char)('0' + value / 10));
        target.Append((char)('0' + value % 10));
    }
}
