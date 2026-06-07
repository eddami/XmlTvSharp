namespace XmlTvSharp;

/// <summary>
///     Specifies how a reader handles XML content outside the supported XMLTV model.
/// </summary>
public enum XmlTvUnknownContentHandling
{
    /// <summary>Ignore the unknown content.</summary>
    Ignore = 0,

    /// <summary>Throw an <see cref="XmlTvReadException" /> when unknown content is encountered.</summary>
    Disallow = 1
}
