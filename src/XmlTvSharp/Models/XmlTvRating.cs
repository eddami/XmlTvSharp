using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>rating</c> element.
/// </summary>
public sealed class XmlTvRating
{
    /// <summary>Initializes a content rating.</summary>
    /// <param name="value">The provider-defined rating value.</param>
    /// <param name="system">The optional rating system.</param>
    public XmlTvRating(string value, string? system = null)
    {
        Value = XmlTvArgument.NotEmpty(value, nameof(value));
        System = system;
    }

    /// <summary>Gets or sets the provider-defined rating value.</summary>
    public string Value { get; set; }

    /// <summary>Gets or sets the optional rating system.</summary>
    public string? System { get; set; }

    /// <summary>Gets the rating icons in XML order.</summary>
    public Collection<XmlTvIcon> Icons { get; } = [];
}
