using System.Text;
using System.Xml;

namespace XmlTvSharp.Writing;

internal static class XmlTvWriterFactory
{
    internal static XmlWriter Create(string path, XmlTvWriterOptions options)
    {
        return XmlWriter.Create(path, CreateSettings(options, true));
    }

    internal static XmlWriter Create(Stream stream, XmlTvWriterOptions options, bool leaveOpen)
    {
        return XmlWriter.Create(stream, CreateSettings(options, !leaveOpen));
    }

    internal static XmlWriter Create(TextWriter textWriter, XmlTvWriterOptions options, bool leaveOpen)
    {
        return XmlWriter.Create(textWriter, CreateSettings(options, !leaveOpen));
    }

    private static XmlWriterSettings CreateSettings(XmlTvWriterOptions options, bool closeOutput)
    {
        return new XmlWriterSettings
        {
            Async = true,
            Encoding = new UTF8Encoding(false),
            Indent = options.Indent,
            OmitXmlDeclaration = options.OmitXmlDeclaration,
            CloseOutput = closeOutput,
            ConformanceLevel = ConformanceLevel.Document,
            CheckCharacters = true
        };
    }
}
