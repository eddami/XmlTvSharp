using XmlTvSharp.Parsing;

namespace XmlTvSharp;

/// <summary>
///     Selects which top-level XMLTV elements are read from a stream.
/// </summary>
/// <remarks>
///     A <see langword="null" /> filter reads all supported top-level child elements. A filter created with
///     <see cref="Create" /> reads only the included branches; calling <c>Create().Build()</c> without includes reads
///     root metadata only.
/// </remarks>
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
