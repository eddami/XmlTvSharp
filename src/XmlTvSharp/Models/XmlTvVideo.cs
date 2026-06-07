namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>video</c> element.
/// </summary>
/// <param name="IsPresent">Whether video is present, or <see langword="null" /> when the <c>present</c> child is omitted.</param>
/// <param name="IsColour">Whether video is in colour, or <see langword="null" /> when the <c>colour</c> child is omitted.</param>
/// <param name="Aspect">The provider-supplied aspect ratio description.</param>
/// <param name="Quality">The provider-supplied video quality description.</param>
public sealed record XmlTvVideo(
    bool? IsPresent = null,
    bool? IsColour = null,
    string? Aspect = null,
    string? Quality = null);
