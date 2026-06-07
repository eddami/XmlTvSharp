namespace XmlTvSharp.Parsing;

internal sealed class XmlTvChannelReadFilter
{
    private readonly HashSet<string>? _ids;

    internal XmlTvChannelReadFilter(HashSet<string>? ids)
    {
        _ids = ids;
    }

    internal bool HasIdConstraint => _ids is not null;

    internal bool Accepts(string id)
    {
        return _ids is null || _ids.Contains(id);
    }
}
