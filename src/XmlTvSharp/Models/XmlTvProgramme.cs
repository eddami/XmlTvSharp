using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>programme</c> element.
/// </summary>
public sealed class XmlTvProgramme : IXmlTvElement
{
    private static readonly XmlTvClumpIndex DefaultClumpIndex = new(0, 1);

    /// <summary>Initializes a programme with its required start time, channel identifier, and first title.</summary>
    /// <param name="start">The programme start time.</param>
    /// <param name="channelId">The referenced channel identifier.</param>
    /// <param name="title">The first programme title.</param>
    public XmlTvProgramme(XmlTvDateTime start, string channelId, XmlTvLocalizedText title)
    {
        Start = start ?? throw new ArgumentNullException(nameof(start));
        ChannelId = XmlTvArgument.NotWhiteSpace(channelId, nameof(channelId));
        Titles.Add(title ?? throw new ArgumentNullException(nameof(title)));
    }

    /// <summary>Initializes a programme with its required start time, channel identifier, and titles.</summary>
    /// <param name="start">The programme start time.</param>
    /// <param name="channelId">The referenced channel identifier.</param>
    /// <param name="titles">The programme titles in XML order.</param>
    public XmlTvProgramme(XmlTvDateTime start, string channelId, IEnumerable<XmlTvLocalizedText> titles)
    {
        Start = start ?? throw new ArgumentNullException(nameof(start));
        ChannelId = XmlTvArgument.NotWhiteSpace(channelId, nameof(channelId));
        XmlTvCollections.AddRequiredItems(Titles, titles, nameof(titles));
    }

    /// <summary>Initializes a programme with an unlocalized first title.</summary>
    /// <param name="start">The programme start time.</param>
    /// <param name="channelId">The referenced channel identifier.</param>
    /// <param name="title">The first programme title.</param>
    public XmlTvProgramme(XmlTvDateTime start, string channelId, string title)
        : this(start, channelId, new XmlTvLocalizedText(title))
    {
    }

    /// <summary>Initializes a programme from an exact .NET start time and an unlocalized first title.</summary>
    /// <param name="start">The programme start time.</param>
    /// <param name="channelId">The referenced channel identifier.</param>
    /// <param name="title">The first programme title.</param>
    public XmlTvProgramme(DateTimeOffset start, string channelId, string title)
        : this(XmlTvDateTime.FromDateTimeOffset(start), channelId, title)
    {
    }

    /// <summary>Initializes a programme from an exact .NET start time and titles.</summary>
    /// <param name="start">The programme start time.</param>
    /// <param name="channelId">The referenced channel identifier.</param>
    /// <param name="titles">The programme titles in XML order.</param>
    public XmlTvProgramme(DateTimeOffset start, string channelId, IEnumerable<XmlTvLocalizedText> titles)
        : this(XmlTvDateTime.FromDateTimeOffset(start), channelId, titles)
    {
    }

    /// <summary>Gets or sets the required programme start time.</summary>
    public XmlTvDateTime Start { get; set; }

    /// <summary>Gets or sets the optional programme stop time.</summary>
    public XmlTvDateTime? Stop { get; set; }

    /// <summary>Gets or sets the optional Programme Delivery Control start time.</summary>
    public XmlTvDateTime? PdcStart { get; set; }

    /// <summary>Gets or sets the optional Video Programming System start time.</summary>
    public XmlTvDateTime? VpsStart { get; set; }

    /// <summary>Gets or sets the optional ShowView code.</summary>
    public string? ShowView { get; set; }

    /// <summary>Gets or sets the optional VideoPlus code.</summary>
    public string? VideoPlus { get; set; }

    /// <summary>Gets or sets the channel identifier referenced by the programme.</summary>
    public string ChannelId { get; set; }

    /// <summary>Gets or sets the explicitly supplied clump index, or <see langword="null" /> when omitted.</summary>
    public XmlTvClumpIndex? ClumpIndex { get; set; }

    /// <summary>Gets the explicit clump index, or XMLTV's default <c>0/1</c> when omitted.</summary>
    public XmlTvClumpIndex EffectiveClumpIndex => ClumpIndex ?? DefaultClumpIndex;

    /// <summary>Gets the required titles in XML order.</summary>
    public Collection<XmlTvLocalizedText> Titles { get; } = [];

    /// <summary>Gets the subtitles in XML order.</summary>
    public Collection<XmlTvLocalizedText> SubTitles { get; } = [];

    /// <summary>Gets the descriptions in XML order.</summary>
    public Collection<XmlTvLocalizedText> Descriptions { get; } = [];

    /// <summary>Gets or sets the optional programme credits.</summary>
    public XmlTvCredits? Credits { get; set; }

    /// <summary>Gets or sets the optional programme date.</summary>
    public XmlTvDateTime? Date { get; set; }

    /// <summary>Gets the categories in XML order.</summary>
    public Collection<XmlTvLocalizedText> Categories { get; } = [];

    /// <summary>Gets the keywords in XML order.</summary>
    public Collection<XmlTvLocalizedText> Keywords { get; } = [];

    /// <summary>Gets or sets the optional programme language.</summary>
    public XmlTvLocalizedText? Language { get; set; }

    /// <summary>Gets or sets the optional original programme language.</summary>
    public XmlTvLocalizedText? OriginalLanguage { get; set; }

    /// <summary>Gets or sets the optional programme length.</summary>
    public XmlTvDuration? Length { get; set; }

    /// <summary>Gets the programme icons in XML order.</summary>
    public Collection<XmlTvIcon> Icons { get; } = [];

    /// <summary>Gets the programme URLs in XML order.</summary>
    public Collection<XmlTvUrl> Urls { get; } = [];

    /// <summary>Gets the countries in XML order.</summary>
    public Collection<XmlTvLocalizedText> Countries { get; } = [];

    /// <summary>Gets the episode numbers in XML order; multiple numbering systems and duplicate systems are preserved.</summary>
    public Collection<XmlTvEpisodeNumber> EpisodeNumbers { get; } = [];

    /// <summary>Gets or sets the optional video metadata.</summary>
    public XmlTvVideo? Video { get; set; }

    /// <summary>Gets or sets the optional audio metadata.</summary>
    public XmlTvAudio? Audio { get; set; }

    /// <summary>Gets or sets metadata about a previous showing.</summary>
    public XmlTvPreviouslyShown? PreviouslyShown { get; set; }

    /// <summary>Gets or sets the optional premiere announcement text.</summary>
    public XmlTvLocalizedText? Premiere { get; set; }

    /// <summary>Gets or sets the optional last-chance announcement text.</summary>
    public XmlTvLocalizedText? LastChance { get; set; }

    /// <summary>Gets or sets whether the programme contains the XMLTV <c>new</c> marker.</summary>
    public bool IsNew { get; set; }

    /// <summary>Gets subtitle metadata entries in XML order.</summary>
    public Collection<XmlTvSubtitles> Subtitles { get; } = [];

    /// <summary>Gets content ratings in XML order.</summary>
    public Collection<XmlTvRating> Ratings { get; } = [];

    /// <summary>Gets star ratings in XML order.</summary>
    public Collection<XmlTvStarRating> StarRatings { get; } = [];

    /// <summary>Gets reviews in XML order.</summary>
    public Collection<XmlTvReview> Reviews { get; } = [];

    /// <summary>Gets images in XML order.</summary>
    public Collection<XmlTvImage> Images { get; } = [];

    /// <summary>Gets or sets programme data defined by supported XMLTV dialects.</summary>
    public XmlTvProgrammeExtensions? Extensions { get; set; }
}
