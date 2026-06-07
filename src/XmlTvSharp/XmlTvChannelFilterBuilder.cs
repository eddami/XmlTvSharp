using XmlTvSharp.Parsing;

namespace XmlTvSharp;

/// <summary>
///     Builds channel element constraints.
/// </summary>
public sealed class XmlTvChannelFilterBuilder
{
    private HashSet<string>? _ids;

    internal XmlTvChannelFilterBuilder()
    {
    }

    /// <summary>Includes channel elements with one of the supplied IDs.</summary>
    /// <param name="ids">The channel IDs.</param>
    /// <returns>This builder.</returns>
    public XmlTvChannelFilterBuilder WithIds(IEnumerable<string> ids)
    {
        EnsureIdsNotConfigured();
        _ids = SnapshotIds(ids, nameof(ids));
        return this;
    }

    /// <summary>Includes channel elements with one of the supplied IDs.</summary>
    /// <param name="ids">The channel IDs.</param>
    /// <returns>This builder.</returns>
    public XmlTvChannelFilterBuilder WithIds(params string[] ids)
    {
        return WithIds((IEnumerable<string>)ids);
    }

    internal XmlTvChannelReadFilter Build()
    {
        return new XmlTvChannelReadFilter(_ids);
    }

    private void EnsureIdsNotConfigured()
    {
        if (_ids is not null)
        {
            throw new InvalidOperationException($"{nameof(WithIds)} cannot be called more than once.");
        }
    }

    internal static HashSet<string> SnapshotIds(IEnumerable<string> ids, string parameterName)
    {
        if (ids is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var id in ids)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("IDs cannot be null, empty, or whitespace.", parameterName);
            }

            set.Add(id);
        }

        if (set.Count == 0)
        {
            throw new ArgumentException("At least one ID is required.", parameterName);
        }

        return set;
    }
}
