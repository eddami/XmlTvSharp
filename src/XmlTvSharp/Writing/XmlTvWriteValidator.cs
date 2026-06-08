using System.Collections.ObjectModel;
using XmlTvSharp.Models;

namespace XmlTvSharp.Writing;

internal static class XmlTvWriteValidator
{
    internal static void ValidateMetadata(XmlTvMetadata metadata)
    {
        if (metadata is null)
        {
            throw new ArgumentNullException(nameof(metadata));
        }
    }

    internal static void ValidateChannel(XmlTvChannel channel)
    {
        if (channel is null)
        {
            throw new ArgumentNullException(nameof(channel));
        }

        RequireText(channel.Id, "Channel id is required.");
        RequireItems(channel.DisplayNames, "Channel requires at least one display-name.");
        RequireNoNullItems(channel.DisplayNames, "Channel display-name collection cannot contain null items.");
        RequireNoNullItems(channel.Icons, "Channel icon collection cannot contain null items.");
        RequireNoNullItems(channel.Urls, "Channel url collection cannot contain null items.");

        foreach (var displayName in channel.DisplayNames)
        {
            RequireLocalizedText(displayName, "Channel display-name cannot be empty.");
        }
    }

    internal static void ValidateProgramme(XmlTvProgramme programme)
    {
        if (programme is null)
        {
            throw new ArgumentNullException(nameof(programme));
        }

        if (programme.Start is null)
        {
            throw new XmlTvWriteException("Programme start is required.");
        }

        RequireText(programme.ChannelId, "Programme channel is required.");
        RequireItems(programme.Titles, "Programme requires at least one title.");
        RequireNoNullItems(programme.Titles, "Programme title collection cannot contain null items.");
        RequireNoNullItems(programme.SubTitles, "Programme sub-title collection cannot contain null items.");
        RequireNoNullItems(programme.Descriptions, "Programme desc collection cannot contain null items.");
        RequireNoNullItems(programme.Categories, "Programme category collection cannot contain null items.");
        RequireNoNullItems(programme.Keywords, "Programme keyword collection cannot contain null items.");
        RequireNoNullItems(programme.Icons, "Programme icon collection cannot contain null items.");
        RequireNoNullItems(programme.Urls, "Programme url collection cannot contain null items.");
        RequireNoNullItems(programme.Countries, "Programme country collection cannot contain null items.");
        RequireNoNullItems(programme.EpisodeNumbers, "Programme episode-num collection cannot contain null items.");
        RequireNoNullItems(programme.Subtitles, "Programme subtitles collection cannot contain null items.");
        RequireNoNullItems(programme.Ratings, "Programme rating collection cannot contain null items.");
        RequireNoNullItems(programme.StarRatings, "Programme star-rating collection cannot contain null items.");
        RequireNoNullItems(programme.Reviews, "Programme review collection cannot contain null items.");
        RequireNoNullItems(programme.Images, "Programme image collection cannot contain null items.");

        foreach (var title in programme.Titles)
        {
            RequireLocalizedText(title, "Programme title cannot be empty.");
        }

        foreach (var subTitle in programme.SubTitles)
        {
            RequireLocalizedText(subTitle, "Programme sub-title cannot be empty.");
        }

        foreach (var description in programme.Descriptions)
        {
            RequireLocalizedText(description, "Programme desc cannot be empty.");
        }

        foreach (var category in programme.Categories)
        {
            RequireLocalizedText(category, "Programme category cannot be empty.");
        }

        foreach (var keyword in programme.Keywords)
        {
            RequireLocalizedText(keyword, "Programme keyword cannot be empty.");
        }

        foreach (var country in programme.Countries)
        {
            RequireLocalizedText(country, "Programme country cannot be empty.");
        }

        if (programme.Language is not null)
        {
            RequireLocalizedText(programme.Language, "Programme language cannot be empty.");
        }

        if (programme.OriginalLanguage is not null)
        {
            RequireLocalizedText(programme.OriginalLanguage, "Programme orig-language cannot be empty.");
        }

        if (programme.Credits is not null)
        {
            ValidateCredits(programme.Credits);
        }

        if (programme.Video is { } video)
        {
            RequireOptionalText(video.Aspect, "Programme video-aspect cannot be empty.");
            RequireOptionalText(video.Quality, "Programme video-quality cannot be empty.");
        }

        if (programme.Audio is { } audio)
        {
            RequireOptionalText(audio.Stereo, "Programme audio-stereo cannot be empty.");
        }

        foreach (var subtitles in programme.Subtitles)
        {
            ValidateSubtitles(subtitles);
        }

        foreach (var rating in programme.Ratings)
        {
            ValidateRating(rating);
        }

        foreach (var starRating in programme.StarRatings)
        {
            ValidateStarRating(starRating);
        }
    }

    private static void ValidateRating(XmlTvRating rating)
    {
        RequireText(rating.Value, "Rating value is required.");
        RequireNoNullItems(rating.Icons, "Rating icon collection cannot contain null items.");
    }

    private static void ValidateStarRating(XmlTvStarRating rating)
    {
        RequireText(rating.Value, "Star-rating value is required.");
        RequireNoNullItems(rating.Icons, "Star-rating icon collection cannot contain null items.");
    }

    private static void ValidateSubtitles(XmlTvSubtitles subtitles)
    {
        if (subtitles.Language is not null)
        {
            RequireLocalizedText(subtitles.Language, "Subtitles language cannot be empty.");
        }
    }

    private static void ValidateActorCredit(XmlTvActorCredit credit)
    {
        ValidateCreditContent(credit.Content);
    }

    private static void ValidateCredit(XmlTvCredit credit)
    {
        ValidateCreditContent(credit.Content);
    }

    private static void ValidateCreditCollection(Collection<XmlTvCredit> credits, string message)
    {
        RequireNoNullItems(credits, message);

        foreach (var credit in credits)
        {
            ValidateCredit(credit);
        }
    }

    private static void ValidateCredits(XmlTvCredits credits)
    {
        ValidateCreditCollection(credits.Directors, "Credits director collection cannot contain null items.");
        RequireNoNullItems(credits.Actors, "Credits actor collection cannot contain null items.");
        foreach (var actor in credits.Actors)
        {
            ValidateActorCredit(actor);
        }

        ValidateCreditCollection(credits.Writers, "Credits writer collection cannot contain null items.");
        ValidateCreditCollection(credits.Adapters, "Credits adapter collection cannot contain null items.");
        ValidateCreditCollection(credits.Producers, "Credits producer collection cannot contain null items.");
        ValidateCreditCollection(credits.Composers, "Credits composer collection cannot contain null items.");
        ValidateCreditCollection(credits.Editors, "Credits editor collection cannot contain null items.");
        ValidateCreditCollection(credits.Presenters, "Credits presenter collection cannot contain null items.");
        ValidateCreditCollection(credits.Commentators, "Credits commentator collection cannot contain null items.");
        ValidateCreditCollection(credits.Guests, "Credits guest collection cannot contain null items.");
    }

    private static void ValidateCreditContent(Collection<XmlTvCreditContent> content)
    {
        RequireNoNullItems(content, "Credit content collection cannot contain null items.");
    }

    private static void RequireItems<T>(Collection<T> items, string message)
    {
        if (items.Count == 0)
        {
            throw new XmlTvWriteException(message);
        }
    }

    private static void RequireNoNullItems<T>(Collection<T> items, string message)
    {
        foreach (var item in items)
        {
            if (item is null)
            {
                throw new XmlTvWriteException(message);
            }
        }
    }

    private static void RequireLocalizedText(XmlTvLocalizedText value, string message)
    {
        if (value.Value.Length == 0)
        {
            throw new XmlTvWriteException(message);
        }
    }

    private static void RequireOptionalText(string? value, string message)
    {
        if (value is not null && string.IsNullOrWhiteSpace(value))
        {
            throw new XmlTvWriteException(message);
        }
    }

    private static void RequireText(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new XmlTvWriteException(message);
        }
    }
}
