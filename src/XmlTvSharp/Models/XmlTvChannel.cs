using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>channel</c> element.
/// </summary>
public sealed class XmlTvChannel : IXmlTvElement
{
    /// <summary>Initializes a channel with its required identifier and first display name.</summary>
    /// <param name="id">The channel identifier referenced by programmes.</param>
    /// <param name="displayName">The first display name.</param>
    public XmlTvChannel(string id, XmlTvLocalizedText displayName)
    {
        Id = XmlTvArgument.NotWhiteSpace(id, nameof(id));
        DisplayNames.Add(displayName ?? throw new ArgumentNullException(nameof(displayName)));
    }

    /// <summary>Initializes a channel with its required identifier and display names.</summary>
    /// <param name="id">The channel identifier referenced by programmes.</param>
    /// <param name="displayNames">The display names in XML order.</param>
    public XmlTvChannel(string id, IEnumerable<XmlTvLocalizedText> displayNames)
    {
        Id = XmlTvArgument.NotWhiteSpace(id, nameof(id));
        XmlTvCollections.AddRequiredItems(DisplayNames, displayNames, nameof(displayNames));
    }

    /// <summary>Initializes a channel with an unlocalized first display name.</summary>
    /// <param name="id">The channel identifier referenced by programmes.</param>
    /// <param name="displayName">The first display name.</param>
    public XmlTvChannel(string id, string displayName)
        : this(id, new XmlTvLocalizedText(displayName))
    {
    }

    /// <summary>Gets or sets the channel identifier referenced by programmes.</summary>
    public string Id { get; set; }

    /// <summary>Gets the required display names in XML order.</summary>
    public Collection<XmlTvLocalizedText> DisplayNames { get; } = [];

    /// <summary>Gets the channel icons in XML order.</summary>
    public Collection<XmlTvIcon> Icons { get; } = [];

    /// <summary>Gets the channel URLs in XML order.</summary>
    public Collection<XmlTvUrl> Urls { get; } = [];
}
