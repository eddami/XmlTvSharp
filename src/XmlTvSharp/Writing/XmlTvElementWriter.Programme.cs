using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Writing;

internal sealed partial class XmlTvElementWriter
{
    internal async Task WriteProgrammeAsync(XmlTvProgramme programme)
    {
        await _writer.WriteStartElementAsync(null, XmlTvNames.Elements.Programme, null).ConfigureAwait(false);
        await _writer
            .WriteAttributeStringAsync(null, XmlTvNames.Attributes.Start, null, programme.Start.ToXmlTvString())
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.Stop, programme.Stop?.ToXmlTvString())
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.PdcStart, programme.PdcStart?.ToXmlTvString())
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.VpsStart, programme.VpsStart?.ToXmlTvString())
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.ShowView, programme.ShowView).ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.VideoPlus, programme.VideoPlus).ConfigureAwait(false);
        await _writer.WriteAttributeStringAsync(null, XmlTvNames.Attributes.Channel, null, programme.ChannelId)
            .ConfigureAwait(false);
        await WriteOptionalAttributeAsync(XmlTvNames.Attributes.ClumpIndex, programme.ClumpIndex?.ToXmlTvString())
            .ConfigureAwait(false);

        foreach (var title in programme.Titles)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Title, title).ConfigureAwait(false);
        }

        foreach (var subTitle in programme.SubTitles)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.SubTitle, subTitle).ConfigureAwait(false);
        }

        foreach (var description in programme.Descriptions)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Desc, description).ConfigureAwait(false);
        }

        if (programme.Credits is not null)
        {
            await WriteCreditsAsync(programme.Credits).ConfigureAwait(false);
        }

        if (programme.Date is not null)
        {
            await WriteTextElementAsync(XmlTvNames.Elements.Date, programme.Date.ToXmlTvString()).ConfigureAwait(false);
        }

        foreach (var category in programme.Categories)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Category, category).ConfigureAwait(false);
        }

        foreach (var keyword in programme.Keywords)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Keyword, keyword).ConfigureAwait(false);
        }

        if (programme.Language is not null)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Language, programme.Language)
                .ConfigureAwait(false);
        }

        if (programme.OriginalLanguage is not null)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.OriginalLanguage, programme.OriginalLanguage)
                .ConfigureAwait(false);
        }

        if (programme.Length is { } length)
        {
            await WriteDurationAsync(length).ConfigureAwait(false);
        }

        foreach (var icon in programme.Icons)
        {
            await WriteIconAsync(icon).ConfigureAwait(false);
        }

        foreach (var url in programme.Urls)
        {
            await WriteUrlAsync(url).ConfigureAwait(false);
        }

        foreach (var country in programme.Countries)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Country, country).ConfigureAwait(false);
        }

        foreach (var episodeNumber in programme.EpisodeNumbers)
        {
            await WriteEpisodeNumberAsync(episodeNumber).ConfigureAwait(false);
        }

        if (programme.Video is not null)
        {
            await WriteVideoAsync(programme.Video).ConfigureAwait(false);
        }

        if (programme.Audio is not null)
        {
            await WriteAudioAsync(programme.Audio).ConfigureAwait(false);
        }

        if (programme.PreviouslyShown is not null)
        {
            await WritePreviouslyShownAsync(programme.PreviouslyShown).ConfigureAwait(false);
        }

        if (programme.Premiere is not null)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.Premiere, programme.Premiere)
                .ConfigureAwait(false);
        }

        if (programme.LastChance is not null)
        {
            await WriteLocalizedTextElementAsync(XmlTvNames.Elements.LastChance, programme.LastChance)
                .ConfigureAwait(false);
        }

        if (programme.IsNew)
        {
            await WriteEmptyElementAsync(XmlTvNames.Elements.New).ConfigureAwait(false);
        }

        if (_options.CompatibilityProfile == XmlTvCompatibilityProfile.Jellyfin &&
            programme.Extensions?.Jellyfin is { IsLive: true })
        {
            await WriteEmptyElementAsync(XmlTvNames.Elements.Live).ConfigureAwait(false);
        }

        foreach (var subtitles in programme.Subtitles)
        {
            await WriteSubtitlesAsync(subtitles).ConfigureAwait(false);
        }

        foreach (var rating in programme.Ratings)
        {
            await WriteRatingAsync(XmlTvNames.Elements.Rating, rating.System, rating.Value, rating.Icons)
                .ConfigureAwait(false);
        }

        foreach (var starRating in programme.StarRatings)
        {
            await WriteRatingAsync(XmlTvNames.Elements.StarRating, starRating.System, starRating.Value,
                    starRating.Icons)
                .ConfigureAwait(false);
        }

        foreach (var review in programme.Reviews)
        {
            await WriteReviewAsync(review).ConfigureAwait(false);
        }

        foreach (var image in programme.Images)
        {
            await WriteImageAsync(image).ConfigureAwait(false);
        }

        await _writer.WriteEndElementAsync().ConfigureAwait(false);
    }
}
