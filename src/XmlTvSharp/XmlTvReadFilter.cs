using XmlTvSharp.Parsing;

namespace XmlTvSharp;

/// <summary>
///     Selects which top-level XMLTV elements are read from a stream.
/// </summary>
public sealed class XmlTvReadFilter
{
    internal XmlTvReadFilter(
        XmlTvChannelReadFilter? channels,
        XmlTvProgrammeReadFilter? programmes)
    {
        Channels = channels;
        Programmes = programmes;
    }

    internal XmlTvChannelReadFilter? Channels { get; }

    internal XmlTvProgrammeReadFilter? Programmes { get; }

    /// <summary>Creates a builder for an explicit top-level XMLTV element selection.</summary>
    /// <returns>A builder used to include channel and programme branches.</returns>
    public static XmlTvElementFilterBuilder Create()
    {
        return new XmlTvElementFilterBuilder();
    }
}
