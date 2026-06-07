namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>subtitles</c> element.
/// </summary>
public sealed record class XmlTvSubtitles
{
    /// <summary>Initializes subtitle metadata.</summary>
    /// <param name="type">The optional subtitle delivery type.</param>
    /// <param name="language">The optional subtitle language.</param>
    public XmlTvSubtitles(
        XmlTvSubtitleType? type = null,
        XmlTvLocalizedText? language = null)
    {
        if (type is not null && !Enum.IsDefined(typeof(XmlTvSubtitleType), type.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }

        Type = type;
        Language = language;
    }

    /// <summary>Gets the optional subtitle delivery type.</summary>
    public XmlTvSubtitleType? Type { get; }

    /// <summary>Gets the optional subtitle language.</summary>
    public XmlTvLocalizedText? Language { get; }
}
