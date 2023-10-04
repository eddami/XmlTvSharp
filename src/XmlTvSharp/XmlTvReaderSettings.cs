namespace XmlTvSharp;

/// <summary>
/// Represents the settings for customizing the behavior of the <see cref="XmlTvReader"/>.
/// </summary>
public class XmlTvReaderSettings
{
    /// <summary>
    /// Gets or sets a function to filter channels by their IDs during parsing.
    /// </summary>
    public Func<string, bool>? FilterByChannelId { get; set; }

    /// <summary>
    /// Gets or sets a function to filter programmes by their start and stop times during parsing.
    /// </summary>
    public Func<DateTimeOffset, DateTimeOffset, bool>? FilterByProgrammeTime { get; set; }

    /// <summary>
    /// Gets or sets the default language to use if language information is not available in the XML data. Default is "en" (English).
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets the time zone to convert programme start and stop times during parsing. Default is UTC.
    /// </summary>
    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;

    /// <summary>
    /// Gets or sets a value indicating whether to ignore channel elements during parsing. Default is false.
    /// </summary>
    public bool IgnoreChannels { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to ignore programme elements during parsing. Default is false.
    /// </summary>
    public bool IgnoreProgrammes { get; set; } = false;
}