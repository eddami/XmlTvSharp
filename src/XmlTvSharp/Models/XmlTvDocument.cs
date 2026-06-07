using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents a complete XMLTV <c>tv</c> document with root metadata, channels, and programmes.
/// </summary>
public sealed class XmlTvDocument
{
    /// <summary>Initializes a document with empty root metadata.</summary>
    public XmlTvDocument()
        : this(new XmlTvMetadata())
    {
    }

    /// <summary>Initializes a document with the supplied root metadata.</summary>
    /// <param name="metadata">The XMLTV root metadata.</param>
    public XmlTvDocument(XmlTvMetadata metadata)
    {
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    /// <summary>Gets the metadata attributes from the XMLTV <c>tv</c> root element.</summary>
    public XmlTvMetadata Metadata { get; }

    /// <summary>Gets the channels in document order.</summary>
    public Collection<XmlTvChannel> Channels { get; } = [];

    /// <summary>Gets the programmes in document order.</summary>
    public Collection<XmlTvProgramme> Programmes { get; } = [];
}
