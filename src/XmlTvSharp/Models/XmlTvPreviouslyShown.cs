namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>previously-shown</c> element.
/// </summary>
/// <param name="Start">The optional start time of the previous showing.</param>
/// <param name="ChannelId">The optional channel identifier of the previous showing.</param>
public sealed record XmlTvPreviouslyShown(
    XmlTvDateTime? Start = null,
    string? ChannelId = null);
