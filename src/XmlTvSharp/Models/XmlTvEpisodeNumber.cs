namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>episode-num</c> element.
/// </summary>
public sealed record XmlTvEpisodeNumber
{
    /// <summary>
    ///     Initializes an episode number.
    /// </summary>
    /// <param name="value">The numbering value in the format identified by <paramref name="system" />.</param>
    /// <param name="system">The optional numbering system. XMLTV defaults an omitted value to <c>onscreen</c>.</param>
    public XmlTvEpisodeNumber(string value, string? system = null)
    {
        Value = XmlTvArgument.NotEmpty(value, nameof(value));
        System = system;
    }

    /// <summary>Gets the episode-number content.</summary>
    public string Value { get; }

    /// <summary>Gets the explicitly supplied <c>system</c> attribute, or <see langword="null" /> when omitted.</summary>
    public string? System { get; }

    /// <summary>Gets the explicit system, or the XMLTV default <c>onscreen</c> when it was omitted.</summary>
    public string EffectiveSystem => System ?? "onscreen";
}
