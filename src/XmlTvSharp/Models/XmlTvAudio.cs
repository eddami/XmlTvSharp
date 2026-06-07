namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>audio</c> element.
/// </summary>
/// <param name="IsPresent">Whether audio is present, or <see langword="null" /> when the <c>present</c> child is omitted.</param>
/// <param name="Stereo">The provider-supplied stereo description, or <see langword="null" /> when omitted.</param>
public sealed record class XmlTvAudio(
    bool? IsPresent = null,
    string? Stereo = null);
