using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Models.Extensions;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private async Task<XmlTvProgramme> ReadProgrammeAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(
            XmlTvNames.Attributes.Start,
            XmlTvNames.Attributes.Stop,
            XmlTvNames.Attributes.PdcStart,
            XmlTvNames.Attributes.VpsStart,
            XmlTvNames.Attributes.ShowView,
            XmlTvNames.Attributes.VideoPlus,
            XmlTvNames.Attributes.Channel,
            XmlTvNames.Attributes.ClumpIndex);

        var start = ReadRequiredDateTimeAttribute(XmlTvNames.Attributes.Start);
        var channelId = RequiredAttribute(XmlTvNames.Attributes.Channel);
        var stop = ReadOptionalDateTimeAttribute(XmlTvNames.Attributes.Stop);
        var pdcStart = ReadOptionalDateTimeAttribute(XmlTvNames.Attributes.PdcStart);
        var vpsStart = ReadOptionalDateTimeAttribute(XmlTvNames.Attributes.VpsStart);
        var showView = _reader.GetAttribute(XmlTvNames.Attributes.ShowView);
        var videoPlus = _reader.GetAttribute(XmlTvNames.Attributes.VideoPlus);
        var clumpIndex = ReadOptionalClumpIndexAttribute(XmlTvNames.Attributes.ClumpIndex);

        var titles = new List<XmlTvLocalizedText>();
        var subTitles = new List<XmlTvLocalizedText>();
        var descriptions = new List<XmlTvLocalizedText>();
        var categories = new List<XmlTvLocalizedText>();
        var keywords = new List<XmlTvLocalizedText>();
        var icons = new List<XmlTvIcon>();
        var urls = new List<XmlTvUrl>();
        var countries = new List<XmlTvLocalizedText>();
        var episodeNumbers = new List<XmlTvEpisodeNumber>();
        var subtitles = new List<XmlTvSubtitles>();
        var ratings = new List<XmlTvRating>();
        var starRatings = new List<XmlTvStarRating>();
        var reviews = new List<XmlTvReview>();
        var images = new List<XmlTvImage>();
        XmlTvCredits? credits = null;
        XmlTvDateTime? date = null;
        XmlTvLocalizedText? language = null;
        XmlTvLocalizedText? originalLanguage = null;
        XmlTvDuration? length = null;
        XmlTvVideo? video = null;
        XmlTvAudio? audio = null;
        XmlTvPreviouslyShown? previouslyShown = null;
        XmlTvLocalizedText? premiere = null;
        XmlTvLocalizedText? lastChance = null;
        var isNew = false;
        var isLive = false;
        var hasCredits = false;
        var hasDate = false;
        var hasLanguage = false;
        var hasOriginalLanguage = false;
        var hasLength = false;
        var hasVideo = false;
        var hasAudio = false;
        var hasPreviouslyShown = false;
        var hasPremiere = false;
        var hasLastChance = false;
        var hasNew = false;
        var hasLive = false;

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            throw ReadError("Programme requires at least one title element.");
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Programme })
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
                case XmlTvNames.Elements.Title:
                    titles.Add(await ReadLocalizedTextAsync(XmlTvNames.Elements.Title, false, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.SubTitle:
                    subTitles.Add(await ReadLocalizedTextAsync(XmlTvNames.Elements.SubTitle, false, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Desc:
                    descriptions.Add(await ReadLocalizedTextAsync(XmlTvNames.Elements.Desc, true, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Credits:
                    EnsureNotSeen(ref hasCredits, XmlTvNames.Elements.Credits);
                    credits = await ReadCreditsAsync(cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Date:
                    EnsureNotSeen(ref hasDate, XmlTvNames.Elements.Date);
                    date = await ReadDateTimeElementAsync(XmlTvNames.Elements.Date, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Category:
                    categories.Add(await ReadLocalizedTextAsync(XmlTvNames.Elements.Category, false, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Keyword:
                    keywords.Add(await ReadLocalizedTextAsync(XmlTvNames.Elements.Keyword, false, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Language:
                    EnsureNotSeen(ref hasLanguage, XmlTvNames.Elements.Language);
                    language = await ReadLocalizedTextAsync(XmlTvNames.Elements.Language, false, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.OriginalLanguage:
                    EnsureNotSeen(ref hasOriginalLanguage, XmlTvNames.Elements.OriginalLanguage);
                    originalLanguage =
                        await ReadLocalizedTextAsync(XmlTvNames.Elements.OriginalLanguage, false, cancellationToken)
                            .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Length:
                    EnsureNotSeen(ref hasLength, XmlTvNames.Elements.Length);
                    length = await ReadDurationAsync(cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Icon:
                    if (ReadIcon() is { } icon)
                    {
                        icons.Add(icon);
                    }

                    await ReadEmptyElementTailAsync(XmlTvNames.Elements.Icon, cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Url:
                    urls.Add(await ReadUrlAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Country:
                    countries.Add(await ReadLocalizedTextAsync(XmlTvNames.Elements.Country, false, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.EpisodeNumber:
                    episodeNumbers.Add(await ReadEpisodeNumberAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Video:
                    EnsureNotSeen(ref hasVideo, XmlTvNames.Elements.Video);
                    video = await ReadVideoAsync(cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Audio:
                    EnsureNotSeen(ref hasAudio, XmlTvNames.Elements.Audio);
                    audio = await ReadAudioAsync(cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.PreviouslyShown:
                    EnsureNotSeen(ref hasPreviouslyShown, XmlTvNames.Elements.PreviouslyShown);
                    previouslyShown = ReadPreviouslyShown();
                    await ReadEmptyElementTailAsync(XmlTvNames.Elements.PreviouslyShown, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Premiere:
                    EnsureNotSeen(ref hasPremiere, XmlTvNames.Elements.Premiere);
                    premiere = await ReadLocalizedTextAsync(XmlTvNames.Elements.Premiere, true, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.LastChance:
                    EnsureNotSeen(ref hasLastChance, XmlTvNames.Elements.LastChance);
                    lastChance = await ReadLocalizedTextAsync(XmlTvNames.Elements.LastChance, true, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.New:
                    EnsureNotSeen(ref hasNew, XmlTvNames.Elements.New);
                    ValidateAttributes();
                    isNew = true;
                    await ReadEmptyElementTailAsync(XmlTvNames.Elements.New, cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Live when _options.CompatibilityProfile == XmlTvCompatibilityProfile.Jellyfin:
                    EnsureNotSeen(ref hasLive, XmlTvNames.Elements.Live);
                    ValidateAttributes();
                    isLive = true;
                    await ReadEmptyElementTailAsync(XmlTvNames.Elements.Live, cancellationToken).ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Subtitles:
                    subtitles.Add(await ReadSubtitlesAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Rating:
                    ratings.Add(await ReadRatingAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.StarRating:
                    starRatings.Add(await ReadStarRatingAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Review:
                    reviews.Add(await ReadReviewAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Image:
                    images.Add(await ReadImageAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(XmlTvNames.Elements.Programme, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }

        if (titles.Count == 0)
        {
            throw ReadError("Programme requires at least one title element.");
        }

        var programme = new XmlTvProgramme(start, channelId, titles)
        {
            Stop = stop,
            PdcStart = pdcStart,
            VpsStart = vpsStart,
            ShowView = showView,
            VideoPlus = videoPlus,
            ClumpIndex = clumpIndex,
            Credits = credits,
            Date = date,
            Language = language,
            OriginalLanguage = originalLanguage,
            Length = length,
            Video = video,
            Audio = audio,
            PreviouslyShown = previouslyShown,
            Premiere = premiere,
            LastChance = lastChance,
            IsNew = isNew
        };

        AddRange(programme.SubTitles, subTitles);
        AddRange(programme.Descriptions, descriptions);
        AddRange(programme.Categories, categories);
        AddRange(programme.Keywords, keywords);
        AddRange(programme.Icons, icons);
        AddRange(programme.Urls, urls);
        AddRange(programme.Countries, countries);
        AddRange(programme.EpisodeNumbers, episodeNumbers);
        AddRange(programme.Subtitles, subtitles);
        AddRange(programme.Ratings, ratings);
        AddRange(programme.StarRatings, starRatings);
        AddRange(programme.Reviews, reviews);
        AddRange(programme.Images, images);
        if (isLive)
        {
            programme.Extensions = new XmlTvProgrammeExtensions
            {
                Jellyfin = new XmlTvJellyfinProgrammeExtensions { IsLive = true }
            };
        }

        return programme;
    }
}
