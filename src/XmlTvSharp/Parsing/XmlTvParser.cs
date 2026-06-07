using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private readonly XmlTvReadFilter? _filter;
    private readonly XmlTvReaderOptions _options;
    private readonly XmlReader _reader;
    private bool _finished;
    private XmlTvMetadata? _metadata;

    internal XmlTvParser(XmlReader reader, XmlTvReaderOptions? options, XmlTvReadFilter? filter)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _options = options ?? new XmlTvReaderOptions();
        _filter = filter;
    }

    internal async Task<XmlTvDocument> ReadDocumentAsync(CancellationToken cancellationToken)
    {
        var metadata = await ReadMetadataAsync(cancellationToken).ConfigureAwait(false);
        var document = new XmlTvDocument(metadata);

        while (await ReadElementCoreAsync(cancellationToken).ConfigureAwait(false) is { } element)
        {
            if (element is XmlTvChannel channel)
            {
                document.Channels.Add(channel);
            }
            else if (element is XmlTvProgramme programme)
            {
                document.Programmes.Add(programme);
            }
        }

        return document;
    }

    internal Task<XmlTvMetadata> ReadMetadataAsync(CancellationToken cancellationToken)
    {
        if (_metadata is { } metadata)
        {
            return Task.FromResult(metadata);
        }

        return ReadRootMetadataAsync(cancellationToken);
    }

    internal Task<IXmlTvElement?> ReadElementAsync(CancellationToken cancellationToken)
    {
        return _metadata is null
            ? ReadElementAfterMetadataAsync(cancellationToken)
            : ReadElementCoreAsync(cancellationToken);
    }

    private async Task<IXmlTvElement?> ReadElementAfterMetadataAsync(CancellationToken cancellationToken)
    {
        await ReadRootMetadataAsync(cancellationToken).ConfigureAwait(false);
        return await ReadElementCoreAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<IXmlTvElement?> ReadElementCoreAsync(CancellationToken cancellationToken)
    {
        if (_finished)
        {
            return null;
        }

        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Tv })
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                _finished = true;
                return null;
            }

            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                if (!await ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    _finished = true;
                    return null;
                }

                continue;
            }

            switch (_reader.Name)
            {
                case XmlTvNames.Elements.Channel:
                    if (!ShouldReadChannel())
                    {
                        // Filtering happens before model construction so large rejected subtrees only advance the XML cursor.
                        await SkipAsync(cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    return await ReadChannelAsync(cancellationToken).ConfigureAwait(false);
                case XmlTvNames.Elements.Programme:
                    if (!ShouldReadProgramme())
                    {
                        // Programme filters are limited to channel identity so skipping never parses nested content.
                        await SkipAsync(cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    return await ReadProgrammeAsync(cancellationToken).ConfigureAwait(false);
                default:
                    await HandleUnknownTopLevelElementAsync(cancellationToken).ConfigureAwait(false);
                    continue;
            }
        }
    }

    private async Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _reader.ReadAsync().ConfigureAwait(false);
    }

    private async Task SkipAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _reader.SkipAsync().ConfigureAwait(false);
    }

    private bool ShouldReadChannel()
    {
        if (_filter is null)
        {
            return true;
        }

        if (_filter.Channels is not { } channelFilter)
        {
            return false;
        }

        if (!channelFilter.HasIdConstraint)
        {
            return true;
        }

        var id = RequiredAttribute(XmlTvNames.Attributes.Id);
        return channelFilter.Accepts(id);
    }

    private bool ShouldReadProgramme()
    {
        if (_filter is null)
        {
            return true;
        }

        if (_filter.Programmes is not { } programmeFilter)
        {
            return false;
        }

        if (!programmeFilter.HasChannelConstraint)
        {
            return true;
        }

        var channelId = RequiredAttribute(XmlTvNames.Attributes.Channel);
        return programmeFilter.AcceptsChannel(channelId);
    }
}
