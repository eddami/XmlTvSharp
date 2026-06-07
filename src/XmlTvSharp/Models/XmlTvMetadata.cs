namespace XmlTvSharp.Models;

/// <summary>
///     Represents metadata attributes from the XMLTV <c>tv</c> root element.
/// </summary>
/// <remarks>
///     Complete documents expose this metadata through <see cref="XmlTvDocument.Metadata" />. Streaming readers expose it
///     through <see cref="XmlTvSharp.XmlTvReader.ReadMetadataAsync" /> before reading channel and programme elements.
/// </remarks>
public sealed class XmlTvMetadata
{
    /// <summary>Gets or sets the optional document generation date from the <c>date</c> attribute.</summary>
    public XmlTvDateTime? Date { get; set; }

    /// <summary>Gets or sets the optional URL describing the source.</summary>
    public string? SourceInfoUrl { get; set; }

    /// <summary>Gets or sets the optional human-readable source name.</summary>
    public string? SourceInfoName { get; set; }

    /// <summary>Gets or sets the optional URL from which the source data was obtained.</summary>
    public string? SourceDataUrl { get; set; }

    /// <summary>Gets or sets the optional name of the generating application.</summary>
    public string? GeneratorInfoName { get; set; }

    /// <summary>Gets or sets the optional URL describing the generating application.</summary>
    public string? GeneratorInfoUrl { get; set; }
}
