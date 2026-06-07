using System.Xml;

namespace XmlTvSharp.Parsing;

internal static class XmlTvReaderFactory
{
    internal static XmlReader Create(string path)
    {
        return XmlReader.Create(path, CreateSettings());
    }

    internal static XmlReader Create(Stream stream, bool leaveOpen)
    {
        return XmlReader.Create(stream, CreateSettings(!leaveOpen));
    }

    internal static XmlReader Create(TextReader reader, bool leaveOpen)
    {
        return XmlReader.Create(reader, CreateSettings(!leaveOpen));
    }

    private static XmlReaderSettings CreateSettings(bool closeInput = true)
    {
        return new XmlReaderSettings
        {
            Async = true,
            CloseInput = closeInput,
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreComments = true,
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = true,
            XmlResolver = null
        };
    }
}
