namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>review</c> element.
/// </summary>
public sealed record class XmlTvReview
{
    /// <summary>Initializes a review.</summary>
    /// <param name="value">The review text or URL, according to <paramref name="type" />.</param>
    /// <param name="type">The review content type.</param>
    /// <param name="source">The optional review source.</param>
    /// <param name="reviewer">The optional reviewer name.</param>
    /// <param name="language">The optional XML language tag.</param>
    public XmlTvReview(
        string value,
        XmlTvReviewType type,
        string? source = null,
        string? reviewer = null,
        string? language = null)
    {
        Value = XmlTvArgument.NotEmpty(value, nameof(value));

        if (!Enum.IsDefined(typeof(XmlTvReviewType), type))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }

        Type = type;
        Source = source;
        Reviewer = reviewer;
        Language = language;
    }

    /// <summary>Gets the review text or URL.</summary>
    public string Value { get; }

    /// <summary>Gets the review content type.</summary>
    public XmlTvReviewType Type { get; }

    /// <summary>Gets the optional review source.</summary>
    public string? Source { get; }

    /// <summary>Gets the optional reviewer name.</summary>
    public string? Reviewer { get; }

    /// <summary>Gets the optional XML language tag.</summary>
    public string? Language { get; }
}
