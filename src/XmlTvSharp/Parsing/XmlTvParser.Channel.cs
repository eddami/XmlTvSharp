using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private async Task<XmlTvChannel> ReadChannelAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.Id);

        var id = RequiredAttribute(XmlTvNames.Attributes.Id);
        var displayNames = new List<XmlTvLocalizedText>();
        List<XmlTvIcon>? icons = null;
        List<XmlTvUrl>? urls = null;

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            throw ReadError("Channel requires at least one display-name element.");
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Channel })
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                break;
            }

            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                if (!await ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    break;
                }

                continue;
            }

            switch (_reader.Name)
            {
                case XmlTvNames.Elements.DisplayName:
                    displayNames.Add(
                        await ReadLocalizedTextAsync(XmlTvNames.Elements.DisplayName, false, cancellationToken)
                            .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Icon:
                    if (ReadIcon() is { } icon)
                    {
                        (icons ??= []).Add(icon);
                    }

                    await ReadEmptyElementTailAsync(XmlTvNames.Elements.Icon, cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Url:
                    (urls ??= []).Add(await ReadUrlAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                default:
                    await HandleUnknownElementAsync(XmlTvNames.Elements.Channel, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }

        if (displayNames.Count == 0)
        {
            throw ReadError("Channel requires at least one display-name element.");
        }

        var channel = new XmlTvChannel(id, displayNames);
        AddRange(channel.Icons, icons);
        AddRange(channel.Urls, urls);

        return channel;
    }
}
