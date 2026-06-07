using XmlTvSharp.Parsing;

namespace XmlTvSharp;

/// <summary>
///     Builds a top-level XMLTV element filter.
/// </summary>
public sealed class XmlTvElementFilterBuilder
{
    private XmlTvChannelReadFilter? _channels;
    private XmlTvProgrammeReadFilter? _programmes;

    internal XmlTvElementFilterBuilder()
    {
    }

    /// <summary>Includes all channel elements.</summary>
    /// <returns>This builder.</returns>
    public XmlTvElementFilterBuilder IncludeChannels()
    {
        EnsureBranchNotConfigured(_channels, nameof(IncludeChannels));
        _channels = new XmlTvChannelReadFilter(null);
        return this;
    }

    /// <summary>Includes channel elements accepted by the configured channel filter.</summary>
    /// <param name="configure">The channel filter configuration.</param>
    /// <returns>This builder.</returns>
    public XmlTvElementFilterBuilder IncludeChannels(Action<XmlTvChannelFilterBuilder> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        EnsureBranchNotConfigured(_channels, nameof(IncludeChannels));
        var builder = new XmlTvChannelFilterBuilder();
        configure(builder);
        _channels = builder.Build();
        return this;
    }

    /// <summary>Includes all programme elements.</summary>
    /// <returns>This builder.</returns>
    public XmlTvElementFilterBuilder IncludeProgrammes()
    {
        EnsureBranchNotConfigured(_programmes, nameof(IncludeProgrammes));
        _programmes = new XmlTvProgrammeReadFilter(null);
        return this;
    }

    /// <summary>Includes programme elements accepted by the configured programme filter.</summary>
    /// <param name="configure">The programme filter configuration.</param>
    /// <returns>This builder.</returns>
    public XmlTvElementFilterBuilder IncludeProgrammes(Action<XmlTvProgrammeFilterBuilder> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        EnsureBranchNotConfigured(_programmes, nameof(IncludeProgrammes));
        var builder = new XmlTvProgrammeFilterBuilder();
        configure(builder);
        _programmes = builder.Build();
        return this;
    }

    /// <summary>Builds the immutable read filter.</summary>
    /// <returns>The read filter.</returns>
    public XmlTvReadFilter Build()
    {
        return new XmlTvReadFilter(_channels, _programmes);
    }

    private static void EnsureBranchNotConfigured<T>(T? branch, string methodName)
        where T : class
    {
        if (branch is not null)
        {
            throw new InvalidOperationException($"{methodName} cannot be called more than once.");
        }
    }
}
