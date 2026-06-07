namespace XmlTvSharp.Models;

/// <summary>
///     Represents the zero-based position and total count encoded by an XMLTV <c>clumpidx</c> attribute.
/// </summary>
public sealed record XmlTvClumpIndex
{
    /// <summary>
    ///     Initializes a clump index.
    /// </summary>
    /// <param name="index">The zero-based programme position.</param>
    /// <param name="count">The total number of programmes in the clump.</param>
    public XmlTvClumpIndex(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (count <= index)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and greater than index.");
        }

        Index = index;
        Count = count;
    }

    /// <summary>Gets the zero-based programme position.</summary>
    public int Index { get; }

    /// <summary>Gets the total number of programmes in the clump.</summary>
    public int Count { get; }

    /// <summary>Parses the XMLTV <c>index/count</c> representation.</summary>
    /// <param name="value">The clump-index text.</param>
    /// <returns>The parsed clump index.</returns>
    /// <exception cref="FormatException"><paramref name="value" /> is not a valid clump index.</exception>
    public static XmlTvClumpIndex Parse(string value)
    {
        XmlTvArgument.NotWhiteSpace(value, nameof(value));

        var separator = value.IndexOf('/');
        if (separator <= 0 ||
            separator == value.Length - 1 ||
            value.IndexOf('/', separator + 1) >= 0 ||
            !TryParseNonNegativeInt(value, 0, separator, out var index) ||
            !TryParseNonNegativeInt(value, separator + 1, value.Length - separator - 1, out var count))
        {
            throw new FormatException("Value is not a valid XMLTV clump index.");
        }

        try
        {
            return new XmlTvClumpIndex(index, count);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            throw new FormatException("Value is not a valid XMLTV clump index.", exception);
        }
    }

    /// <summary>Attempts to parse the XMLTV <c>index/count</c> representation.</summary>
    /// <param name="value">The clump-index text.</param>
    /// <param name="result">The parsed clump index when successful; otherwise, <see langword="null" />.</param>
    /// <returns><see langword="true" /> when parsing succeeds; otherwise, <see langword="false" />.</returns>
    public static bool TryParse(string value, out XmlTvClumpIndex? result)
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

    /// <summary>Formats the value using XMLTV's <c>index/count</c> representation.</summary>
    /// <returns>The XMLTV clump-index text.</returns>
    public string ToXmlTvString()
    {
        return Index + "/" + Count;
    }

    private static bool TryParseNonNegativeInt(string value, int start, int length, out int result)
    {
        result = 0;

        for (var index = start; index < start + length; index++)
        {
            var digit = value[index] - '0';
            if (digit is < 0 or > 9)
            {
                return false;
            }

            if (result > (int.MaxValue - digit) / 10)
            {
                return false;
            }

            result = result * 10 + digit;
        }

        return true;
    }
}
