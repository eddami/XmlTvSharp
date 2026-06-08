using System.Collections.ObjectModel;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Writing;

internal sealed partial class XmlTvElementWriter
{
    private async Task WriteActorCreditAsync(XmlTvActorCredit credit)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Actor, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Role, credit.Role).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Guest, ToYesNoToken(credit.IsGuest))
            .ConfigureAwait(false);
        await WriteCreditContentAsync(credit.Content).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteCreditsAsync(XmlTvCredits credits)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Credits, null).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Director, credits.Directors).ConfigureAwait(false);

        foreach (var actor in credits.Actors)
        {
            await WriteActorCreditAsync(actor).ConfigureAwait(false);
        }

        await WriteCreditCollectionAsync(XmlTvNames.Elements.Writer, credits.Writers).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Adapter, credits.Adapters).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Producer, credits.Producers).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Composer, credits.Composers).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Editor, credits.Editors).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Presenter, credits.Presenters).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Commentator, credits.Commentators).ConfigureAwait(false);
        await WriteCreditCollectionAsync(XmlTvNames.Elements.Guest, credits.Guests).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteCreditAsync(string elementName, XmlTvCredit credit)
    {
        await _writer.WriteStartElementAsync(null, elementName, null).ConfigureAwait(false);
        await WriteCreditContentAsync(credit.Content).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteCreditCollectionAsync(string elementName, Collection<XmlTvCredit> credits)
    {
        foreach (var credit in credits)
        {
            await WriteCreditAsync(elementName, credit).ConfigureAwait(false);
        }
    }

    private async Task WriteCreditContentAsync(Collection<XmlTvCreditContent> content)
    {
        foreach (var item in content)
        {
            switch (item)
            {
                case XmlTvCreditText text:
                    await _writer.WriteStringAsync(text.Value).ConfigureAwait(false);
                    break;
                case XmlTvCreditImage image:
                    await WriteImageAsync(image.Value).ConfigureAwait(false);
                    break;
                case XmlTvCreditUrl url:
                    await WriteUrlAsync(url.Value).ConfigureAwait(false);
                    break;
                default:
                    throw new XmlTvWriteException("Credit content contains an unsupported item.");
            }
        }
    }
}
