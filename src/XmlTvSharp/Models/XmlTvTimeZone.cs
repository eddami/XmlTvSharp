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

        if (ContainsWhiteSpace(Value.AsSpan()))
        {
            throw new ArgumentException("A timezone token cannot contain whitespace.", nameof(value));
        }

        UtcOffset = TryParseNumericOffset(Value.AsSpan(), out var offset) ? offset : null;
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

        return CreateNumeric(CreateNumericToken(offset), offset);
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

    internal static XmlTvTimeZone CreateNumeric(string value, TimeSpan offset)
    {
        return new XmlTvTimeZone(value, offset);
    }

    internal static bool TryParseNumericOffset(ReadOnlySpan<char> value, out TimeSpan offset)
    {
        offset = default;

        if (value.Length is not (5 or 6) || (value[0] != '+' && value[0] != '-'))
        {
            return false;
        }

        var minuteStart = 3;
        if (value.Length == 6)
        {
            if (value[3] != ':')
            {
                return false;
            }

            minuteStart = 4;
        }

        if (!TryParseTwoDigits(value.Slice(1, 2), out var hours) ||
            !TryParseTwoDigits(value.Slice(minuteStart, 2), out var minutes) ||
            minutes > 59)
        {
            return false;
        }

        offset = new TimeSpan(hours, minutes, 0);
        if (value[0] == '-')
        {
            offset = -offset;
        }

        ValidateOffset(offset, nameof(value));
        return true;
    }

    private XmlTvTimeZone(string value, TimeSpan offset)
    {
        Value = XmlTvArgument.NotWhiteSpace(value, nameof(value));
        ValidateOffset(offset, nameof(offset));
        UtcOffset = offset;
        IsNumericOffset = true;
    }

    private static string CreateNumericToken(TimeSpan offset)
    {
        var sign = offset < TimeSpan.Zero ? '-' : '+';
        var absolute = offset.Duration();
        var hours = absolute.Hours;
        var minutes = absolute.Minutes;

#if NET8_0_OR_GREATER
        return string.Create(5, (sign, hours, minutes), static (target, state) =>
        {
            target[0] = state.sign;
            target[1] = (char)('0' + state.hours / 10);
            target[2] = (char)('0' + state.hours % 10);
            target[3] = (char)('0' + state.minutes / 10);
            target[4] = (char)('0' + state.minutes % 10);
        });
#else
        return new string(
        [
            sign,
            (char)('0' + hours / 10),
            (char)('0' + hours % 10),
            (char)('0' + minutes / 10),
            (char)('0' + minutes % 10)
        ]);
#endif
    }

    private static bool TryParseTwoDigits(ReadOnlySpan<char> value, out int result)
    {
        result = 0;

        if (value.Length != 2 || value[0] is < '0' or > '9' || value[1] is < '0' or > '9')
        {
            return false;
        }

        result = (value[0] - '0') * 10 + value[1] - '0';
        return true;
    }

    private static bool ContainsWhiteSpace(ReadOnlySpan<char> value)
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
