using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Writing;

internal sealed partial class XmlTvElementWriter
{
    internal async Task WriteStartAsync(XmlTvMetadata metadata)
    {
        await _writer.WriteStartDocumentAsync().ConfigureAwait(false);
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Tv, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Date, metadata.Date?.ToXmlTvString())
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.SourceInfoUrl, metadata.SourceInfoUrl)
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.SourceInfoName, metadata.SourceInfoName)
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.SourceDataUrl, metadata.SourceDataUrl)
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.GeneratorInfoName, metadata.GeneratorInfoName)
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.GeneratorInfoUrl, metadata.GeneratorInfoUrl)
            .ConfigureAwait(false);
    }

    internal async Task WriteCompleteAsync()
    {
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
        await _writer.WriteEndDocumentAsync().ConfigureAwait(false);
    }
}
