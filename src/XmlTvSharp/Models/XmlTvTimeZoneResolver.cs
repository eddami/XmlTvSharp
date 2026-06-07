namespace XmlTvSharp.Models;

/// <summary>
///     Resolves a named XMLTV timezone token to the UTC offset that applies to a date/time value.
/// </summary>
/// <param name="token">The named timezone token.</param>
/// <param name="value">The XMLTV date/time whose applicable offset is requested.</param>
/// <param name="offset">The resolved whole-minute UTC offset between -14:00 and +14:00.</param>
/// <returns><see langword="true" /> when the token was resolved; otherwise, <see langword="false" />.</returns>
public delegate bool XmlTvTimeZoneResolver(
    string token,
    XmlTvDateTime value,
    out TimeSpan offset);
