namespace XmlTvSharp;

/// <summary>
///     Configures XMLTV reader behavior.
/// </summary>
/// <remarks>
///     Known XMLTV elements are accepted without enforcing DTD child ordering, but malformed values and duplicate
///     singleton elements fail. Unknown standard content and XMLTV <c>x-</c> extension content are controlled
///     separately.
/// </remarks>
public sealed record XmlTvReaderOptions
{
    /// <summary>Gets the compatibility profile used by the reader.</summary>
    public XmlTvCompatibilityProfile CompatibilityProfile { get; init; } = XmlTvCompatibilityProfile.Standard;

    /// <summary>Gets how unknown non-<c>x-</c> elements are handled.</summary>
    public XmlTvUnknownContentHandling UnknownElementHandling { get; init; } = XmlTvUnknownContentHandling.Disallow;

    /// <summary>Gets how unknown non-<c>x-</c> attributes are handled.</summary>
    public XmlTvUnknownContentHandling UnknownAttributeHandling { get; init; } = XmlTvUnknownContentHandling.Disallow;

    /// <summary>Gets how XMLTV <c>x-</c> extension elements and attributes are handled.</summary>
    public XmlTvUnknownContentHandling XExtensionHandling { get; init; } = XmlTvUnknownContentHandling.Ignore;
}
