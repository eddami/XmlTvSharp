namespace XmlTvSharp;

public class XmlTvChannel : IXmlTvElement
{
    public string Id { get; set; } = string.Empty;
    public Dictionary<string, string?> DisplayNames { get; set; } = new();
    public List<XmlTvIcon>? Icons { get; set; }
    public List<XmlTvUrl>? Urls { get; set; }
}