using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private async Task<XmlTvMetadata> ReadRootMetadataAsync(CancellationToken cancellationToken)
    {
        if (_metadata is { } metadata)
        {
            return metadata;
        }

        while (await ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                continue;
            }

            if (_reader.Name != XmlTvNames.Elements.Tv)
            {
                throw ReadError("Expected XMLTV root element 'tv'.");
            }

            ValidateAttributes(
                XmlTvNames.Attributes.Date,
                XmlTvNames.Attributes.SourceInfoUrl,
                XmlTvNames.Attributes.SourceInfoName,
                XmlTvNames.Attributes.SourceDataUrl,
                XmlTvNames.Attributes.GeneratorInfoName,
                XmlTvNames.Attributes.GeneratorInfoUrl);

            var rootMetadata = new XmlTvMetadata
            {
                Date = ReadOptionalDateTimeAttribute(XmlTvNames.Attributes.Date),
                SourceInfoUrl = _reader.GetAttribute(XmlTvNames.Attributes.SourceInfoUrl),
                SourceInfoName = _reader.GetAttribute(XmlTvNames.Attributes.SourceInfoName),
                SourceDataUrl = _reader.GetAttribute(XmlTvNames.Attributes.SourceDataUrl),
                GeneratorInfoName = _reader.GetAttribute(XmlTvNames.Attributes.GeneratorInfoName),
                GeneratorInfoUrl = _reader.GetAttribute(XmlTvNames.Attributes.GeneratorInfoUrl)
            };

            if (_reader.IsEmptyElement)
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                _finished = true;
            }
            else
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
            }

            _metadata = rootMetadata;
            return rootMetadata;
        }

        throw ReadError("XMLTV document is empty.");
    }
}
