namespace XmlTvSharp.Models;

/// <summary>
///     Represents text with an optional XML language tag.
/// </summary>
public sealed record XmlTvLocalizedText
{
    /// <summary>
    ///     Initializes localized text.
    /// </summary>
    /// <param name="value">The text content. Empty text is preserved.</param>
    /// <param name="language">The optional <c>lang</c> attribute value.</param>
    public XmlTvLocalizedText(string value, string? language = null)
    {
        Value = XmlTvArgument.NotNull(value, nameof(value));
        Language = language;
    }

    /// <summary>Gets the text content.</summary>
    public string Value { get; }

    /// <summary>Gets the optional XML language tag.</summary>
    public string? Language { get; }
}
