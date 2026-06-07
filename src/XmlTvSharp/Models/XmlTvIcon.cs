namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>icon</c> element.
/// </summary>
public sealed record XmlTvIcon
{
    /// <summary>
    ///     Initializes an icon reference.
    /// </summary>
    /// <param name="source">The non-empty <c>src</c> attribute.</param>
    /// <param name="width">The optional positive width in pixels.</param>
    /// <param name="height">The optional positive height in pixels.</param>
    public XmlTvIcon(string source, int? width = null, int? height = null)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        Source = XmlTvArgument.NotWhiteSpace(source, nameof(source));
        Width = width;
        Height = height;
    }

    /// <summary>Gets the icon source URI as supplied by the feed.</summary>
    public string Source { get; }

    /// <summary>Gets the optional width in pixels.</summary>
    public int? Width { get; }

    /// <summary>Gets the optional height in pixels.</summary>
    public int? Height { get; }
}
