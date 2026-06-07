using XmlTvSharp.Parsing;

namespace XmlTvSharp;

/// <summary>
///     Builds programme element constraints.
/// </summary>
public sealed class XmlTvProgrammeFilterBuilder
{
    private HashSet<string>? _channelIds;

    internal XmlTvProgrammeFilterBuilder()
    {
    }

    /// <summary>Includes programme elements carried by one of the supplied channel IDs.</summary>
    /// <param name="channelIds">The programme channel IDs.</param>
    /// <returns>This builder.</returns>
    public XmlTvProgrammeFilterBuilder ForChannels(IEnumerable<string> channelIds)
    {
        if (_channelIds is not null)
        {
            throw new InvalidOperationException($"{nameof(ForChannels)} cannot be called more than once.");
        }

        _channelIds = XmlTvChannelFilterBuilder.SnapshotIds(channelIds, nameof(channelIds));
        return this;
    }

    /// <summary>Includes programme elements carried by one of the supplied channel IDs.</summary>
    /// <param name="channelIds">The programme channel IDs.</param>
    /// <returns>This builder.</returns>
    public XmlTvProgrammeFilterBuilder ForChannels(params string[] channelIds)
    {
        return ForChannels((IEnumerable<string>)channelIds);
    }

    internal XmlTvProgrammeReadFilter Build()
    {
        return new XmlTvProgrammeReadFilter(_channelIds);
    }
}
