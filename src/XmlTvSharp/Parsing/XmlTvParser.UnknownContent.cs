using System.Xml;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private Task HandleUnknownTopLevelElementAsync(CancellationToken cancellationToken)
    {
        return HandleUnknownElementAsync(XmlTvNames.Elements.Tv, cancellationToken);
    }

    private Task HandleUnknownNestedElementAsync(string parentElement, CancellationToken cancellationToken)
    {
        return HandleUnknownElementAsync(parentElement, cancellationToken);
    }

    private async Task HandleUnknownElementAsync(string parentElement, CancellationToken cancellationToken)
    {
        if (ShouldIgnoreUnknownElement(_reader.Name))
        {
            await SkipAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        throw ReadError($"Unknown element '{_reader.Name}' inside '{parentElement}'.");
    }

    private bool ShouldIgnoreUnknownElement(string name)
    {
        return name.StartsWith(XmlTvNames.Tokens.XExtensionPrefix, StringComparison.Ordinal)
            ? _options.XExtensionHandling == XmlTvUnknownContentHandling.Ignore
            : _options.UnknownElementHandling == XmlTvUnknownContentHandling.Ignore;
    }

    private bool ShouldIgnoreUnknownAttribute(string name)
    {
        return name.StartsWith(XmlTvNames.Tokens.XExtensionPrefix, StringComparison.Ordinal)
            ? _options.XExtensionHandling == XmlTvUnknownContentHandling.Ignore
            : _options.UnknownAttributeHandling == XmlTvUnknownContentHandling.Ignore;
    }

    private XmlTvReadException ReadError(string message)
    {
        if (_reader is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            return new XmlTvReadException($"{message} Line {lineInfo.LineNumber}, position {lineInfo.LinePosition}.");
        }

        return new XmlTvReadException(message);
    }

    private XmlTvReadException ReadError(string message, Exception innerException)
    {
        if (_reader is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            return new XmlTvReadException(
                $"{message} Line {lineInfo.LineNumber}, position {lineInfo.LinePosition}.",
                innerException);
        }

        return new XmlTvReadException(message, innerException);
    }
}
