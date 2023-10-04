namespace XmlTvSharp;

/// <summary>
/// Represents the result of parsing XML data containing TV program information in XMLTV format.
/// </summary>
public class XmlTvResult
{
    /// <summary>
    /// Gets the list of TV channels parsed from the XML data.
    /// </summary>
    public List<XmlTvChannel> Channels { get; set; } = new();

    /// <summary>
    /// Gets the list of TV programs parsed from the XML data.
    /// </summary>
    public List<XmlTvProgramme> Programmes { get; set; } = new();
}