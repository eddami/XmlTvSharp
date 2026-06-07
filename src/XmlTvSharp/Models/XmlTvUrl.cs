namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>url</c> element.
/// </summary>
public sealed record XmlTvUrl
{
    /// <summary>
    ///     Initializes a URL.
    /// </summary>
    /// <param name="value">The URL text as supplied by the feed.</param>
    /// <param name="system">The optional provider-defined URL system.</param>
    public XmlTvUrl(string value, string? system = null)
    {
        Value = XmlTvArgument.NotEmpty(value, nameof(value));
        System = system;
    }

    /// <summary>Gets the URL text as supplied by the feed.</summary>
    public string Value { get; }

    /// <summary>Gets the optional provider-defined URL system.</summary>
    public string? System { get; }
}
