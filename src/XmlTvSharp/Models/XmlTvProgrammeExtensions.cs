using XmlTvSharp.Models.Extensions;

namespace XmlTvSharp.Models;

/// <summary>
///     Groups optional programme data defined by XMLTV dialects rather than the core DTD.
/// </summary>
public sealed class XmlTvProgrammeExtensions
{
    /// <summary>Gets or sets supported Jellyfin-compatible programme extensions.</summary>
    public XmlTvJellyfinProgrammeExtensions? Jellyfin { get; set; }
}
