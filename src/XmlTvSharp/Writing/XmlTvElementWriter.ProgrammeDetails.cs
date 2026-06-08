using System.Collections.ObjectModel;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Writing;

internal sealed partial class XmlTvElementWriter
{
    private async Task WriteAudioAsync(XmlTvAudio audio)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Audio, null).ConfigureAwait(false);
        await WriteYesNoElementAsync(XmlTvNames.Elements.Present, audio.IsPresent).ConfigureAwait(false);
        await WriteOptionalTextElementAsync(XmlTvNames.Elements.Stereo, audio.Stereo).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteImageAsync(XmlTvImage image)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Image, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Type, ToImageTypeToken(image.Type))
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Size, ToImageSizeToken(image.Size))
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Orientation, ToImageOrientationToken(image.Orientation))
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.System, image.System).ConfigureAwait(false);
        await _writer.WriteStringAsync(image.Value).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteRatingAsync(
        string elementName,
        string? system,
        string value,
        Collection<XmlTvIcon> icons)
    {
        await _writer.WriteStartElementAsync(null, elementName, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.System, system).ConfigureAwait(false);
        await WriteTextElementAsync(XmlTvNames.Elements.Value, value).ConfigureAwait(false);

        foreach (var icon in icons)
        {
            await WriteIconAsync(icon).ConfigureAwait(false);
        }

        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteReviewAsync(XmlTvReview review)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Review, null).ConfigureAwait(false);
        await _writer.WriteAttributeStringAsync(null, XmlTvNames.Attributes.Type, null, ToReviewTypeToken(review.Type))
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Source, review.Source).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Reviewer, review.Reviewer).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Lang, review.Language).ConfigureAwait(false);
        await _writer.WriteStringAsync(review.Value).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteSubtitlesAsync(XmlTvSubtitles subtitles)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Subtitles, null).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Type, ToSubtitleTypeToken(subtitles.Type))
            .ConfigureAwait(false);

        if (subtitles.Language is not null)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Language, subtitles.Language)
                .ConfigureAwait(false);
        }

        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private async Task WriteVideoAsync(XmlTvVideo video)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Video, null).ConfigureAwait(false);
        await WriteYesNoElementAsync(XmlTvNames.Elements.Present, video.IsPresent).ConfigureAwait(false);
        await WriteYesNoElementAsync(XmlTvNames.Elements.Colour, video.IsColour).ConfigureAwait(false);
        await WriteOptionalTextElementAsync(XmlTvNames.Elements.Aspect, video.Aspect).ConfigureAwait(false);
        await WriteOptionalTextElementAsync(XmlTvNames.Elements.Quality, video.Quality).ConfigureAwait(false);
        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }
}
