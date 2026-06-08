using System.Xml;

namespace XmlTvSharp.Writing;

internal sealed partial class XmlTvElementWriter
{
    private readonly XmlTvWriterOptions _options;
    private readonly XmlWriter _writer;

    internal XmlTvElementWriter(XmlWriter writer, XmlTvWriterOptions options)
    {
        _writer = writer;
        _options = options;
    }
}
