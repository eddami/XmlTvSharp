using System.Globalization;
using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private async Task<XmlTvCredits> ReadCreditsAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes();

        var credits = new XmlTvCredits();

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return credits;
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Credits })
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                return credits;
            }

            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                if (!await ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return credits;
                }

                continue;
            }

            switch (_reader.Name)
            {
                case XmlTvNames.Elements.Director:
                    credits.Directors.Add(await ReadCreditAsync(XmlTvNames.Elements.Director, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Actor:
                    credits.Actors.Add(await ReadActorCreditAsync(cancellationToken).ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Writer:
                    credits.Writers.Add(await ReadCreditAsync(XmlTvNames.Elements.Writer, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Adapter:
                    credits.Adapters.Add(await ReadCreditAsync(XmlTvNames.Elements.Adapter, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Producer:
                    credits.Producers.Add(await ReadCreditAsync(XmlTvNames.Elements.Producer, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Composer:
                    credits.Composers.Add(await ReadCreditAsync(XmlTvNames.Elements.Composer, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Editor:
                    credits.Editors.Add(await ReadCreditAsync(XmlTvNames.Elements.Editor, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Presenter:
                    credits.Presenters.Add(await ReadCreditAsync(XmlTvNames.Elements.Presenter, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Commentator:
                    credits.Commentators.Add(await ReadCreditAsync(XmlTvNames.Elements.Commentator, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                case XmlTvNames.Elements.Guest:
                    credits.Guests.Add(await ReadCreditAsync(XmlTvNames.Elements.Guest, cancellationToken)
                        .ConfigureAwait(false));
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(XmlTvNames.Elements.Credits, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }
    }

    private async Task<XmlTvCredit> ReadCreditAsync(string elementName, CancellationToken cancellationToken)
    {
        ValidateAttributes();

        var credit = new XmlTvCredit();
        await ReadCreditContentAsync(elementName, credit.Content, cancellationToken).ConfigureAwait(false);
        return credit;
    }

    private async Task<XmlTvActorCredit> ReadActorCreditAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(
            XmlTvNames.Attributes.Role,
            XmlTvNames.Attributes.Guest);

        var credit = new XmlTvActorCredit
        {
            Role = _reader.GetAttribute(XmlTvNames.Attributes.Role),
            IsGuest = ReadOptionalYesNoAttribute(XmlTvNames.Attributes.Guest)
        };

        await ReadCreditContentAsync(XmlTvNames.Elements.Actor, credit.Content, cancellationToken)
            .ConfigureAwait(false);
        return credit;
    }

    private async Task ReadCreditContentAsync(
        string elementName,
        ICollection<XmlTvCreditContent> content,
        CancellationToken cancellationToken)
    {
        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: var name } && name == elementName)
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            if (_reader is { NodeType: XmlNodeType.Text or XmlNodeType.CDATA or XmlNodeType.SignificantWhitespace })
            {
                content.Add(new XmlTvCreditText(_reader.Value));
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                if (!await ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return;
                }

                continue;
            }

            switch (_reader.Name)
            {
                case XmlTvNames.Elements.Image:
                    var image = await ReadImageAsync(cancellationToken).ConfigureAwait(false);
                    content.Add(new XmlTvCreditImage(image));
                    continue;
                case XmlTvNames.Elements.Url:
                    var url = await ReadUrlAsync(cancellationToken).ConfigureAwait(false);
                    content.Add(new XmlTvCreditUrl(url));
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(elementName, cancellationToken).ConfigureAwait(false);
                    continue;
            }
        }
    }

    private async Task<XmlTvDuration> ReadDurationAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.Units);

        var unit = ReadRequiredDurationUnitAttribute(XmlTvNames.Attributes.Units);
        var value = await ReadElementTextAsync(XmlTvNames.Elements.Length, cancellationToken).ConfigureAwait(false);
        const NumberStyles durationStyles =
            NumberStyles.AllowLeadingWhite |
            NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint;

        if (!decimal.TryParse(value, durationStyles, CultureInfo.InvariantCulture, out var parsed))
        {
            throw ReadError("Element 'length' must contain a decimal value.");
        }

        try
        {
            return new XmlTvDuration(parsed, unit);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            throw ReadError("Element 'length' must contain a non-negative duration.", exception);
        }
    }

    private async Task<XmlTvEpisodeNumber> ReadEpisodeNumberAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.System);

        var system = _reader.GetAttribute(XmlTvNames.Attributes.System);
        var value = await ReadElementTextAsync(XmlTvNames.Elements.EpisodeNumber, cancellationToken)
            .ConfigureAwait(false);
        try
        {
            return new XmlTvEpisodeNumber(value, system);
        }
        catch (ArgumentException exception)
        {
            throw ReadError("Element 'episode-num' cannot be empty.", exception);
        }
    }

    private XmlTvPreviouslyShown ReadPreviouslyShown()
    {
        ValidateAttributes(
            XmlTvNames.Attributes.Start,
            XmlTvNames.Attributes.Channel);

        var start = ReadOptionalDateTimeAttribute(XmlTvNames.Attributes.Start);
        var channelId = _reader.GetAttribute(XmlTvNames.Attributes.Channel);
        return new XmlTvPreviouslyShown(start, channelId);
    }

    private async Task<XmlTvVideo> ReadVideoAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes();

        bool? isPresent = null;
        bool? isColour = null;
        string? aspect = null;
        string? quality = null;
        var hasPresent = false;
        var hasColour = false;
        var hasAspect = false;
        var hasQuality = false;

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return new XmlTvVideo();
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Video })
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                return new XmlTvVideo(isPresent, isColour, aspect, quality);
            }

            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                if (!await ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return new XmlTvVideo(isPresent, isColour, aspect, quality);
                }

                continue;
            }

            switch (_reader.Name)
            {
                case XmlTvNames.Elements.Present:
                    EnsureNotSeen(ref hasPresent, XmlTvNames.Elements.Present);
                    isPresent = await ReadYesNoElementAsync(XmlTvNames.Elements.Present, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Colour:
                    EnsureNotSeen(ref hasColour, XmlTvNames.Elements.Colour);
                    isColour = await ReadYesNoElementAsync(XmlTvNames.Elements.Colour, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Aspect:
                    EnsureNotSeen(ref hasAspect, XmlTvNames.Elements.Aspect);
                    ValidateAttributes();
                    aspect = await ReadElementTextAsync(XmlTvNames.Elements.Aspect, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Quality:
                    EnsureNotSeen(ref hasQuality, XmlTvNames.Elements.Quality);
                    ValidateAttributes();
                    quality = await ReadElementTextAsync(XmlTvNames.Elements.Quality, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(XmlTvNames.Elements.Video, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }
    }

    private async Task<XmlTvAudio> ReadAudioAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes();

        bool? isPresent = null;
        string? stereo = null;
        var hasPresent = false;
        var hasStereo = false;

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return new XmlTvAudio();
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Audio })
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                return new XmlTvAudio(isPresent, stereo);
            }

            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                if (!await ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return new XmlTvAudio(isPresent, stereo);
                }

                continue;
            }

            switch (_reader.Name)
            {
                case XmlTvNames.Elements.Present:
                    EnsureNotSeen(ref hasPresent, XmlTvNames.Elements.Present);
                    isPresent = await ReadYesNoElementAsync(XmlTvNames.Elements.Present, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Stereo:
                    EnsureNotSeen(ref hasStereo, XmlTvNames.Elements.Stereo);
                    ValidateAttributes();
                    stereo = await ReadElementTextAsync(XmlTvNames.Elements.Stereo, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(XmlTvNames.Elements.Audio, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }
    }

    private async Task<XmlTvSubtitles> ReadSubtitlesAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.Type);

        var type = ReadOptionalSubtitleTypeAttribute(XmlTvNames.Attributes.Type);
        XmlTvLocalizedText? language = null;
        var hasLanguage = false;

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return new XmlTvSubtitles(type);
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Subtitles })
            {
                await ReadAsync(cancellationToken).ConfigureAwait(false);
                return new XmlTvSubtitles(type, language);
            }

            if (_reader is not { NodeType: XmlNodeType.Element })
            {
                if (!await ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    return new XmlTvSubtitles(type, language);
                }

                continue;
            }

            switch (_reader.Name)
            {
                case XmlTvNames.Elements.Language:
                    EnsureNotSeen(ref hasLanguage, XmlTvNames.Elements.Language);
                    language = await ReadLocalizedTextAsync(XmlTvNames.Elements.Language, false, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(XmlTvNames.Elements.Subtitles, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }
    }

    private async Task<XmlTvRating> ReadRatingAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.System);

        var system = _reader.GetAttribute(XmlTvNames.Attributes.System);
        var icons = new List<XmlTvIcon>();
        string? value = null;
        var hasValue = false;

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            throw ReadError("Element 'rating' requires a value child.");
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.Rating })
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
                case XmlTvNames.Elements.Value:
                    EnsureNotSeen(ref hasValue, XmlTvNames.Elements.Value);
                    ValidateAttributes();
                    value = await ReadElementTextAsync(XmlTvNames.Elements.Value, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Icon:
                    icons.Add(ReadIcon());
                    await ReadEmptyElementTailAsync(XmlTvNames.Elements.Icon, cancellationToken).ConfigureAwait(false);
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(XmlTvNames.Elements.Rating, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }

        if (value is null)
        {
            throw ReadError("Element 'rating' requires a value child.");
        }

        XmlTvRating rating;
        try
        {
            rating = new XmlTvRating(value, system);
        }
        catch (ArgumentException exception)
        {
            throw ReadError("Element 'rating' value cannot be empty.", exception);
        }

        foreach (var icon in icons)
        {
            rating.Icons.Add(icon);
        }

        return rating;
    }

    private async Task<XmlTvStarRating> ReadStarRatingAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.System);

        var system = _reader.GetAttribute(XmlTvNames.Attributes.System);
        var icons = new List<XmlTvIcon>();
        string? value = null;
        var hasValue = false;

        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            throw ReadError("Element 'star-rating' requires a value child.");
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        while (true)
        {
            if (_reader is { NodeType: XmlNodeType.EndElement, Name: XmlTvNames.Elements.StarRating })
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
                case XmlTvNames.Elements.Value:
                    EnsureNotSeen(ref hasValue, XmlTvNames.Elements.Value);
                    ValidateAttributes();
                    value = await ReadElementTextAsync(XmlTvNames.Elements.Value, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                case XmlTvNames.Elements.Icon:
                    icons.Add(ReadIcon());
                    await ReadEmptyElementTailAsync(XmlTvNames.Elements.Icon, cancellationToken).ConfigureAwait(false);
                    continue;
                default:
                    await HandleUnknownNestedElementAsync(XmlTvNames.Elements.StarRating, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
            }
        }

        if (value is null)
        {
            throw ReadError("Element 'star-rating' requires a value child.");
        }

        XmlTvStarRating rating;
        try
        {
            rating = new XmlTvStarRating(value, system);
        }
        catch (ArgumentException exception)
        {
            throw ReadError("Element 'star-rating' value cannot be empty.", exception);
        }

        foreach (var icon in icons)
        {
            rating.Icons.Add(icon);
        }

        return rating;
    }

    private async Task<XmlTvReview> ReadReviewAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(
            XmlTvNames.Attributes.Type,
            XmlTvNames.Attributes.Source,
            XmlTvNames.Attributes.Reviewer,
            XmlTvNames.Attributes.Lang);

        var type = ReadRequiredReviewTypeAttribute(XmlTvNames.Attributes.Type);
        var source = _reader.GetAttribute(XmlTvNames.Attributes.Source);
        var reviewer = _reader.GetAttribute(XmlTvNames.Attributes.Reviewer);
        var language = _reader.GetAttribute(XmlTvNames.Attributes.Lang);
        var value = await ReadElementTextAsync(XmlTvNames.Elements.Review, cancellationToken).ConfigureAwait(false);
        try
        {
            return new XmlTvReview(value, type, source, reviewer, language);
        }
        catch (ArgumentException exception)
        {
            throw ReadError("Element 'review' cannot be empty.", exception);
        }
    }

    private async Task<XmlTvImage> ReadImageAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(
            XmlTvNames.Attributes.Type,
            XmlTvNames.Attributes.Size,
            XmlTvNames.Attributes.Orientation,
            XmlTvNames.Attributes.System);

        var type = ReadOptionalImageTypeAttribute(XmlTvNames.Attributes.Type);
        var size = ReadOptionalImageSizeAttribute(XmlTvNames.Attributes.Size);
        var orientation = ReadOptionalImageOrientationAttribute(XmlTvNames.Attributes.Orientation);
        var system = _reader.GetAttribute(XmlTvNames.Attributes.System);
        var value = await ReadElementTextAsync(XmlTvNames.Elements.Image, cancellationToken).ConfigureAwait(false);
        try
        {
            return new XmlTvImage(value, type, size, orientation, system);
        }
        catch (ArgumentException exception)
        {
            throw ReadError("Element 'image' cannot be empty.", exception);
        }
    }
}
