using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>star-rating</c> element.
/// </summary>
public sealed class XmlTvStarRating
{
    /// <summary>Initializes a star rating.</summary>
    /// <param name="value">The provider-defined star-rating value.</param>
    /// <param name="system">The optional rating system.</param>
    public XmlTvStarRating(string value, string? system = null)
    {
        Value = XmlTvArgument.NotEmpty(value, nameof(value));
        System = system;
    }

    /// <summary>Gets or sets the provider-defined star-rating value.</summary>
    public string Value { get; set; }

    /// <summary>Gets or sets the optional rating system.</summary>
    public string? System { get; set; }

    /// <summary>Gets the rating icons in XML order.</summary>
    public Collection<XmlTvIcon> Icons { get; } = [];
}
