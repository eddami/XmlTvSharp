using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents a non-actor XMLTV credit while preserving mixed text, image, and URL content order.
/// </summary>
public sealed class XmlTvCredit
{
    /// <summary>Initializes an empty credit.</summary>
    public XmlTvCredit()
    {
    }

    /// <summary>Initializes a credit containing a name.</summary>
    /// <param name="name">The credited name.</param>
    public XmlTvCredit(string name)
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
}
