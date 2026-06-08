namespace XmlTvSharp;

/// <summary>
///     Configures XMLTV reader behavior.
/// </summary>
/// <remarks>
///     Known XMLTV elements are accepted without enforcing DTD child ordering, but malformed values and duplicate
///     singleton elements fail. Unknown standard content and XMLTV <c>x-</c> extension content are controlled
///     separately. Options are immutable; use object initializers or <c>with</c> expressions to customize them.
/// </remarks>
public sealed record XmlTvReaderOptions
{
    /// <summary>Gets the compatibility profile used by the reader.</summary>
    /// <remarks>
    ///     The standard profile recognizes XMLTV DTD content only. Dialect fields require their matching compatibility
    ///     profile or must be handled through the unknown-content options.
    /// </remarks>
    public XmlTvCompatibilityProfile CompatibilityProfile { get; init; } = XmlTvCompatibilityProfile.Standard;

    /// <summary>Gets how unknown non-<c>x-</c> elements are handled.</summary>
    public XmlTvUnknownContentHandling UnknownElementHandling { get; init; } = XmlTvUnknownContentHandling.Disallow;

    /// <summary>Gets how unknown non-<c>x-</c> attributes are handled.</summary>
    public XmlTvUnknownContentHandling UnknownAttributeHandling { get; init; } = XmlTvUnknownContentHandling.Disallow;

    /// <summary>Gets how XMLTV <c>x-</c> extension elements and attributes are handled.</summary>
    public XmlTvUnknownContentHandling XExtensionHandling { get; init; } = XmlTvUnknownContentHandling.Ignore;
}
