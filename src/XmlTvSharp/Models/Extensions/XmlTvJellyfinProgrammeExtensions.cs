namespace XmlTvSharp.Models.Extensions;

/// <summary>
///     Represents supported programme fields from the XMLTV dialect consumed by Jellyfin.
/// </summary>
public sealed class XmlTvJellyfinProgrammeExtensions
{
    /// <summary>Gets or sets whether the Jellyfin-compatible <c>live</c> marker is present.</summary>
    public bool IsLive { get; set; }
}
