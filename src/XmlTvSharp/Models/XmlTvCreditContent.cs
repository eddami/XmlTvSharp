namespace XmlTvSharp.Models;

/// <summary>
///     Represents one ordered content item inside an XMLTV credit element.
/// </summary>
/// <remarks>
///     The XMLTV credit content set is intentionally closed to text, image, and URL items.
/// </remarks>
public abstract record XmlTvCreditContent
{
    private protected XmlTvCreditContent()
    {
    }
}

/// <summary>
///     Represents a text segment inside an XMLTV credit element.
/// </summary>
public sealed record XmlTvCreditText : XmlTvCreditContent
{
    /// <summary>Initializes a credit text segment.</summary>
    /// <param name="value">The text segment value.</param>
    public XmlTvCreditText(string value)
    {
        Value = XmlTvArgument.NotNull(value, nameof(value));
    }

    /// <summary>Gets the text segment.</summary>
    public string Value { get; }
}

/// <summary>
///     Represents an <c>image</c> child inside an XMLTV credit element.
/// </summary>
public sealed record XmlTvCreditImage : XmlTvCreditContent
{
    /// <summary>Initializes a credit image item.</summary>
    /// <param name="value">The image.</param>
    public XmlTvCreditImage(XmlTvImage value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets the image.</summary>
    public XmlTvImage Value { get; }
}

/// <summary>
///     Represents a <c>url</c> child inside an XMLTV credit element.
/// </summary>
public sealed record XmlTvCreditUrl : XmlTvCreditContent
{
    /// <summary>Initializes a credit URL item.</summary>
    /// <param name="value">The URL.</param>
    public XmlTvCreditUrl(XmlTvUrl value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets the URL.</summary>
    public XmlTvUrl Value { get; }
}
