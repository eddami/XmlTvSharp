using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>actor</c> credit while preserving mixed content order.
/// </summary>
public sealed class XmlTvActorCredit
{
    /// <summary>Initializes an empty actor credit.</summary>
    public XmlTvActorCredit()
    {
    }

    /// <summary>Initializes an actor credit containing a name.</summary>
    /// <param name="name">The actor name.</param>
    public XmlTvActorCredit(string name)
    {
        Content.Add(new XmlTvCreditText(name));
    }

    /// <summary>Gets the ordered mixed content of the credit.</summary>
    public Collection<XmlTvCreditContent> Content { get; } = [];

    /// <summary>Gets all text segments concatenated in content order.</summary>
    public string Text => string.Concat(Content.OfType<XmlTvCreditText>().Select(content => content.Value));

    /// <summary>Enumerates images in content order.</summary>
    public IEnumerable<XmlTvImage> Images => Content.OfType<XmlTvCreditImage>().Select(content => content.Value);

    /// <summary>Enumerates URLs in content order.</summary>
    public IEnumerable<XmlTvUrl> Urls => Content.OfType<XmlTvCreditUrl>().Select(content => content.Value);

    /// <summary>Gets or sets the optional role played by the actor.</summary>
    public string? Role { get; set; }

    /// <summary>Gets or sets whether the actor is a guest, or <see langword="null" /> when the attribute is omitted.</summary>
    public bool? IsGuest { get; set; }
}
