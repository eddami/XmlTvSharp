using System.Xml;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private async Task HandleUnknownElementAsync(string parentElement, CancellationToken cancellationToken)
    {
        if (_options.UnknownElementHandling == XmlTvUnknownContentHandling.Ignore)
        {
            await SkipAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        throw ReadError($"Unknown element '{_reader.Name}' inside '{parentElement}'.");
    }

    private XmlTvReadException ReadError(string message)
    {
        if (_reader is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            return new XmlTvReadException(
                $"{message} Line {lineInfo.LineNumber}, position {lineInfo.LinePosition}.",
                lineInfo.LineNumber,
                lineInfo.LinePosition);
        }

        return new XmlTvReadException(message);
    }

    private XmlTvReadException ReadError(string message, Exception innerException)
    {
        if (_reader is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            return new XmlTvReadException(
                $"{message} Line {lineInfo.LineNumber}, position {lineInfo.LinePosition}.",
                lineInfo.LineNumber,
                lineInfo.LinePosition,
                innerException);
        }

        return new XmlTvReadException(message, innerException);
    }
}
