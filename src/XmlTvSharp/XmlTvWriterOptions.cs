namespace XmlTvSharp;

/// <summary>
///     Configures XMLTV writing behavior.
/// </summary>
/// <remarks>
///     Options are immutable. Use object initializers or <c>with</c> expressions to create customized instances.
/// </remarks>
public sealed record XmlTvWriterOptions
{
    internal static readonly XmlTvWriterOptions Default = new();

    /// <summary>Gets the compatibility profile used for non-DTD XMLTV fields.</summary>
    /// <remarks>
    ///     The default <see cref="XmlTvCompatibilityProfile.Standard" /> profile emits only XMLTV DTD content. Other
    ///     profiles may emit supported dialect fields.
    /// </remarks>
    public XmlTvCompatibilityProfile CompatibilityProfile { get; init; } = XmlTvCompatibilityProfile.Standard;

    /// <summary>Gets whether generated XML should be indented for readability.</summary>
    public bool Indent { get; init; }

    /// <summary>Gets whether to omit the XML declaration.</summary>
    public bool OmitXmlDeclaration { get; init; }
}
