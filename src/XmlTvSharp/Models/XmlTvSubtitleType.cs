namespace XmlTvSharp.Models;

/// <summary>
///     Specifies how subtitles are delivered for a programme.
/// </summary>
public enum XmlTvSubtitleType
{
    /// <summary>Subtitles are delivered through teletext.</summary>
    Teletext = 0,

    /// <summary>Subtitles are displayed onscreen.</summary>
    Onscreen = 1,

    /// <summary>Subtitles are provided through signing for deaf viewers.</summary>
    DeafSigned = 2
}
