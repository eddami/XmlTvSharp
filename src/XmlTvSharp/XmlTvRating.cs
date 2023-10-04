namespace XmlTvSharp;

public class XmlTvRating
{
    public string Value { get; set; } = string.Empty;
    public List<XmlTvIcon> Icons { get; set; } = new();
}