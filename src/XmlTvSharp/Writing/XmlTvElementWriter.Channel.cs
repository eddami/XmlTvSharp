using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Writing;

internal sealed partial class XmlTvElementWriter
{
    internal async Task WriteChannelAsync(XmlTvChannel channel)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Channel, null).ConfigureAwait(false);
        await _writer.WriteAttributeStringAsync(null, XmlTvNames.Attributes.Id, null, channel.Id).ConfigureAwait(false);

        foreach (var displayName in channel.DisplayNames)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.DisplayName, displayName).ConfigureAwait(false);
        }

        foreach (var icon in channel.Icons)
        {
            await WriteIconAsync(icon).ConfigureAwait(false);
        }

        foreach (var url in channel.Urls)
        {
            await WriteUrlAsync(url).ConfigureAwait(false);
        }

        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }
}
