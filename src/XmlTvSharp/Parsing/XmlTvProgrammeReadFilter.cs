namespace XmlTvSharp.Parsing;

internal sealed class XmlTvProgrammeReadFilter
{
    private readonly HashSet<string>? _channelIds;

    internal XmlTvProgrammeReadFilter(HashSet<string>? channelIds)
    {
        _channelIds = channelIds;
    }

    internal bool HasChannelConstraint => _channelIds is not null;

    internal bool AcceptsChannel(string channelId)
    {
        return _channelIds is null || _channelIds.Contains(channelId);
    }
}
