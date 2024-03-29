namespace XmlTvSharp;

public class XmlTvProgramme : IXmlTvElement
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset Stop { get; set; }
    public string ChannelId { get; set; } = string.Empty;
    public Dictionary<string, string> Titles { get; set; } = new();
    public Dictionary<string, string>? SubTitles { get; set; }
    public Dictionary<string, string>? Descriptions { get; set; }
    public List<XmlTvUrl>? Urls { get; set; }
    public List<string>? Actors { get; set; }
    public DateTimeOffset? Date { get; set; }
    public bool IsPreviouslyShown { get; set; }
    public DateTimeOffset? PreviouslyShownDate { get; set; }
    public string? PreviouslyShownChannel { get; set; }
    public List<XmlTvIcon>? Icons { get; set; }
    public XmlTvCredits? Credits { get; set; }
    public Dictionary<string, XmlTvRating>? Ratings { get; set; }
    public string? StarRating { get; set; }
    public List<XmlTvEpisode>? Episodes { get; set; }
    public string? Language { get; set; }
    public List<string>? Categories { get; set; }
    public List<string>? Countries { get; set; }
    public string? Quality { get; set; }
    public bool IsNew { get; set; }
    public bool IsPremiere { get; set; }
    public string? Premiere { get; set; }
    public string? PremiereLanguage { get; set; }
    public string? OuterXml { get; set; }
}