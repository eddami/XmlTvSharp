using System.Globalization;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV numeric offset or named timezone token without discarding its source text.
/// </summary>
public sealed record XmlTvTimeZone
{
    /// <summary>
    ///     Initializes a timezone token.
    /// </summary>
    /// <param name="value">
    ///     A whitespace-free named token or numeric offset in <c>+HHMM</c>, <c>-HHMM</c>, <c>+HH:MM</c>, or
    ///     <c>-HH:MM</c> form.
    /// </param>
    public XmlTvTimeZone(string value)
    {
        Value = XmlTvArgument.NotWhiteSpace(value, nameof(value));

        if (ContainsWhiteSpace(Value))
        {
            throw new ArgumentException("A timezone token cannot contain whitespace.", nameof(value));
        }

        UtcOffset = ParseNumericOffset(Value);
        if ((Value[0] == '+' || Value[0] == '-') && UtcOffset is null)
        {
            throw new ArgumentException("A numeric timezone token must use the form +HHMM, -HHMM, +HH:MM, or -HH:MM.",
                nameof(value));
        }

        IsNumericOffset = UtcOffset.HasValue;
    }

    /// <summary>Gets the timezone token as supplied.</summary>
    public string Value { get; }

    /// <summary>Gets the parsed UTC offset for a numeric token, or <see langword="null" /> for a named token.</summary>
    public TimeSpan? UtcOffset { get; }

    /// <summary>Gets a value indicating whether this token is a numeric UTC offset.</summary>
    public bool IsNumericOffset { get; }

    /// <summary>Creates an XMLTV numeric timezone token from a UTC offset.</summary>
    /// <param name="offset">A whole-minute offset between -14:00 and +14:00.</param>
    /// <returns>The numeric timezone token.</returns>
    public static XmlTvTimeZone FromUtcOffset(TimeSpan offset)
    {
        ValidateOffset(offset, nameof(offset));

        var sign = offset < TimeSpan.Zero ? '-' : '+';
        var absolute = offset.Duration();
        var value = string.Format(
            CultureInfo.InvariantCulture,
            "{0}{1:00}{2:00}",
            sign,
            absolute.Hours,
            absolute.Minutes);

        return new XmlTvTimeZone(value);
    }

    internal static void ValidateOffset(TimeSpan offset, string parameterName)
    {
        if (offset < TimeSpan.FromHours(-14) || offset > TimeSpan.FromHours(14))
        {
            throw new ArgumentOutOfRangeException(parameterName, "UTC offset must be between -14:00 and +14:00.");
        }

        if (offset.Ticks % TimeSpan.TicksPerMinute != 0)
        {
            throw new ArgumentException("UTC offset must use whole minutes.", parameterName);
        }
    }

    private static TimeSpan? ParseNumericOffset(string value)
    {
        if (value.Length is not (5 or 6) ||
            (value[0] != '+' && value[0] != '-'))
        {
            return null;
        }

        var minuteStart = 3;
        if (value.Length == 6)
        {
            if (value[3] != ':')
            {
                return null;
            }

            minuteStart = 4;
        }

        if (!TryParseTwoDigits(value, 1, out var hours) ||
            !TryParseTwoDigits(value, minuteStart, out var minutes) ||
            minutes > 59)
        {
            return null;
        }

        var offset = new TimeSpan(hours, minutes, 0);
        if (value[0] == '-')
        {
            offset = -offset;
        }

        ValidateOffset(offset, nameof(value));
        return offset;
    }

    private static bool TryParseTwoDigits(string value, int start, out int result)
    {
        result = 0;

        if (start + 1 >= value.Length ||
            value[start] is < '0' or > '9' ||
            value[start + 1] is < '0' or > '9')
        {
            return false;
        }

        result = (value[start] - '0') * 10 + value[start + 1] - '0';
        return true;
    }

    private static bool ContainsWhiteSpace(string value)
    {
        for (var index = 0; index < value.Length; index++)
        {
            if (char.IsWhiteSpace(value[index]))
            {
                return true;
            }
        }

        return false;
    }
}
