namespace XmlTvSharp.Models;

/// <summary>
///     Specifies the least-significant component present in an XMLTV date/time value.
/// </summary>
public enum XmlTvDateTimePrecision
{
    /// <summary>The value contains a year.</summary>
    Year = 0,

    /// <summary>The value contains a year and month.</summary>
    Month = 1,

    /// <summary>The value contains a calendar date.</summary>
    Day = 2,

    /// <summary>The value contains a date and hour.</summary>
    Hour = 3,

    /// <summary>The value contains a date, hour, and minute.</summary>
    Minute = 4,

    /// <summary>The value contains a complete date and time through seconds.</summary>
    Second = 5
}
