using System.Globalization;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Writing;

internal sealed partial class XmlTvElementWriter
{
    private async Task WriteIconAsync(XmlTvIcon icon)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Icon, null).ConfigureAwait(false);
        await _writer.WriteAttributeStringAsync(null, XmlTvNames.Attributes.Src, null, icon.Source)
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Width, icon.Width).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Height, icon.Height).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteLocalizedTextElementAsync(string elementName, XmlTvLocalizedText text)
    {
        await _writer.WriteStartElementAsync(null, elementName, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Lang, text.Language).ConfigureAwait(false);
        if (text.Value.Length > 0)
        {
            await _writer.WriteStringAsync(text.Value).ConfigureAwait(false);
        }

        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteDurationAsync(XmlTvDuration duration)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Length, null).ConfigureAwait(false);
        await _writer
            .WriteAttributeStringAsync(null, XmlTvNames.Attributes.Units, null, ToDurationUnitToken(duration.Unit))
            .ConfigureAwait(false);
        await _writer.WriteStringAsync(duration.Value.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteEmptyElementAsync(string elementName)
    {
        await _writer.WriteStartElementAsync(null, elementName, null).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteEpisodeNumberAsync(XmlTvEpisodeNumber episodeNumber)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.EpisodeNumber, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.System, episodeNumber.System).ConfigureAwait(false);
        await _writer.WriteStringAsync(episodeNumber.Value).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteUrlAsync(XmlTvUrl url)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Url, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.System, url.System).ConfigureAwait(false);
        await _writer.WriteStringAsync(url.Value).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WritePreviouslyShownAsync(XmlTvPreviouslyShown previouslyShown)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.PreviouslyShown, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Start, previouslyShown.Start?.ToXmlTvString())
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Channel, previouslyShown.ChannelId)
            .ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private Task WriteOptionalAttributeAsync(string name, int? value)
    {
        return WriteOptionalAttributeAsync(name, value?.ToString(CultureInfo.InvariantCulture));
    }

    private async Task WriteOptionalAttributeAsync(string name, string? value)
    {
        if (value is not null)
        {
            await _writer.WriteAttributeStringAsync(null, name, null, value).ConfigureAwait(false);
        }
    }

    private Task WriteOptionalTextElementAsync(string elementName, string? value)
    {
        return value is null ? Task.CompletedTask : WriteTextElementAsync(elementName, value);
    }

    private async Task WriteTextElementAsync(string elementName, string value)
    {
        await _writer.WriteStartElementAsync(null, elementName, null).ConfigureAwait(false);
        await _writer.WriteStringAsync(value).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private Task WriteYesNoElementAsync(string elementName, bool? value)
    {
        return value is null
            ? Task.CompletedTask
            : WriteTextElementAsync(elementName, ToYesNoToken(value)!);
    }

    private static string ToDurationUnitToken(XmlTvDurationUnit unit)
    {
        return unit switch
        {
            XmlTvDurationUnit.Seconds => XmlTvNames.Tokens.Seconds,
            XmlTvDurationUnit.Minutes => XmlTvNames.Tokens.Minutes,
            XmlTvDurationUnit.Hours => XmlTvNames.Tokens.Hours,
            _ => throw new XmlTvWriteException("Length has an invalid unit.")
        };
    }

    private static string? ToImageOrientationToken(XmlTvImageOrientation? orientation)
    {
        return orientation switch
        {
            null => null,
            XmlTvImageOrientation.Portrait => XmlTvNames.Tokens.Portrait,
            XmlTvImageOrientation.Landscape => XmlTvNames.Tokens.Landscape,
            _ => throw new XmlTvWriteException("Image has an invalid orientation.")
        };
    }

    private static string? ToImageSizeToken(XmlTvImageSize? size)
    {
        return size switch
        {
            null => null,
            XmlTvImageSize.Small => XmlTvNames.Tokens.ImageSmall,
            XmlTvImageSize.Medium => XmlTvNames.Tokens.ImageMedium,
            XmlTvImageSize.Large => XmlTvNames.Tokens.ImageLarge,
            _ => throw new XmlTvWriteException("Image has an invalid size.")
        };
    }

    private static string? ToImageTypeToken(XmlTvImageType? type)
    {
        return type switch
        {
            null => null,
            XmlTvImageType.Poster => XmlTvNames.Tokens.Poster,
            XmlTvImageType.Backdrop => XmlTvNames.Tokens.Backdrop,
            XmlTvImageType.Still => XmlTvNames.Tokens.Still,
            XmlTvImageType.Person => XmlTvNames.Tokens.Person,
            XmlTvImageType.Character => XmlTvNames.Tokens.Character,
            _ => throw new XmlTvWriteException("Image has an invalid type.")
        };
    }

    private static string ToReviewTypeToken(XmlTvReviewType type)
    {
        return type switch
        {
            XmlTvReviewType.Text => XmlTvNames.Tokens.ReviewText,
            XmlTvReviewType.Url => XmlTvNames.Tokens.ReviewUrl,
            _ => throw new XmlTvWriteException("Review has an invalid type.")
        };
    }

    private static string? ToSubtitleTypeToken(XmlTvSubtitleType? type)
    {
        return type switch
        {
            null => null,
            XmlTvSubtitleType.Teletext => XmlTvNames.Tokens.Teletext,
            XmlTvSubtitleType.Onscreen => XmlTvNames.Tokens.Onscreen,
            XmlTvSubtitleType.DeafSigned => XmlTvNames.Tokens.DeafSigned,
            _ => throw new XmlTvWriteException("Subtitles has an invalid type.")
        };
    }

    private static string? ToYesNoToken(bool? value)
    {
        return value switch
        {
            null => null,
            true => XmlTvNames.Tokens.Yes,
            false => XmlTvNames.Tokens.No
        };
    }
}
