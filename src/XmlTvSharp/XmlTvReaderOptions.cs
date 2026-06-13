namespace XmlTvSharp;

/// <summary>
///     Configures XMLTV reader behavior.
/// </summary>
/// <remarks>
///     Known XMLTV elements are accepted without enforcing DTD child ordering, but malformed values and duplicate
///     singleton elements fail. Unknown content is controlled separately for elements and attributes. Options are
///     immutable; use object initializers or <c>with</c> expressions to customize them.
/// </remarks>
public sealed record XmlTvReaderOptions
{
    /// <summary>Gets the compatibility profile used by the reader.</summary>
    /// <remarks>
    ///     The standard profile recognizes XMLTV DTD content only. Dialect fields require their matching compatibility
    ///     profile or must be handled through the unknown-content options.
    /// </remarks>
    public XmlTvCompatibilityProfile CompatibilityProfile { get; init; } = XmlTvCompatibilityProfile.Standard;

    /// <summary>Gets how unknown elements are handled.</summary>
    public XmlTvUnknownContentHandling UnknownElementHandling { get; init; } = XmlTvUnknownContentHandling.Disallow;

    /// <summary>Gets how unknown attributes are handled.</summary>
    public XmlTvUnknownContentHandling UnknownAttributeHandling { get; init; } = XmlTvUnknownContentHandling.Disallow;
}
