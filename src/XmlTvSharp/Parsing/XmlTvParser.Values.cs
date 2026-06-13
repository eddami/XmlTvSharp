using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Xml;

namespace XmlTvSharp.Parsing;

internal sealed partial class XmlTvParser
{
    private string RequiredAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw ReadError($"Required attribute '{name}' is missing or empty on '{_reader.Name}'.");
        }

        return value;
    }

    private void ValidateAttributes(params string[] allowedNames)
    {
        if (!_reader.HasAttributes)
        {
            return;
        }

        var elementName = _reader.Name;
        while (_reader.MoveToNextAttribute())
        {
            if (_reader.NamespaceURI == XmlTvNames.Namespaces.Xmlns)
            {
                continue;
            }

            if (Array.IndexOf(allowedNames, _reader.Name) >= 0)
            {
                continue;
            }

            if (_options.UnknownAttributeHandling == XmlTvUnknownContentHandling.Ignore)
            {
                continue;
            }

            throw ReadError($"Unknown attribute '{_reader.Name}' on '{elementName}'.");
        }

        _reader.MoveToElement();
    }

    private void EnsureNotSeen(ref bool seen, string elementName)
    {
        if (seen)
        {
            throw ReadError($"Element '{elementName}' cannot appear more than once here.");
        }

        seen = true;
    }

    private XmlTvDateTime ReadRequiredDateTimeAttribute(string name)
    {
        var value = RequiredAttribute(name);
        try
        {
            return XmlTvDateTime.Parse(value);
        }
        catch (FormatException exception)
        {
            throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV date/time.", exception);
        }
    }

    private XmlTvDateTime? ReadOptionalDateTimeAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        if (value is null)
        {
            return null;
        }

        try
        {
            return XmlTvDateTime.Parse(value);
        }
        catch (FormatException exception)
        {
            throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV date/time.", exception);
        }
    }

    private XmlTvClumpIndex? ReadOptionalClumpIndexAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        if (value is null)
        {
            return null;
        }

        try
        {
            return XmlTvClumpIndex.Parse(value);
        }
        catch (FormatException exception)
        {
            throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV clump index.", exception);
        }
    }

    private async Task<XmlTvDateTime> ReadDateTimeElementAsync(string elementName, CancellationToken cancellationToken)
    {
        ValidateAttributes();

        var value = await ReadElementTextAsync(elementName, cancellationToken).ConfigureAwait(false);
        try
        {
            return XmlTvDateTime.Parse(value);
        }
        catch (FormatException exception)
        {
            throw ReadError($"Element '{elementName}' is not a valid XMLTV date/time.", exception);
        }
    }

    private async Task<XmlTvLocalizedText> ReadLocalizedTextAsync(
        string elementName,
        bool allowEmpty,
        CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.Lang);

        var language = _reader.GetAttribute(XmlTvNames.Attributes.Lang);
        var value = await ReadElementTextAsync(elementName, cancellationToken).ConfigureAwait(false);
        if (!allowEmpty && value.Length == 0)
        {
            throw ReadError($"Element '{elementName}' cannot be empty.");
        }

        return new XmlTvLocalizedText(value, language);
    }

    private async Task<XmlTvUrl> ReadUrlAsync(CancellationToken cancellationToken)
    {
        ValidateAttributes(XmlTvNames.Attributes.System);

        var system = _reader.GetAttribute(XmlTvNames.Attributes.System);
        var value = await ReadElementTextAsync(XmlTvNames.Elements.Url, cancellationToken).ConfigureAwait(false);
        try
        {
            return new XmlTvUrl(value, system);
        }
        catch (ArgumentException exception)
        {
            throw ReadError("Element 'url' cannot be empty.", exception);
        }
    }

    private XmlTvIcon? ReadIcon()
    {
        ValidateAttributes(
            XmlTvNames.Attributes.Src,
            XmlTvNames.Attributes.Width,
            XmlTvNames.Attributes.Height);

        var source = _reader.GetAttribute(XmlTvNames.Attributes.Src);
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        try
        {
            return new XmlTvIcon(
                source,
                ReadOptionalPositiveIntAttribute(XmlTvNames.Attributes.Width),
                ReadOptionalPositiveIntAttribute(XmlTvNames.Attributes.Height));
        }
        catch (XmlTvReadException)
        {
            return null;
        }
    }

    private int? ReadOptionalPositiveIntAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        if (value is null)
        {
            return null;
        }

        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) || parsed <= 0)
        {
            throw ReadError($"Attribute '{name}' on '{_reader.Name}' must be a positive integer.");
        }

        return parsed;
    }

    private XmlTvDurationUnit ReadRequiredDurationUnitAttribute(string name)
    {
        return RequiredAttribute(name) switch
        {
            XmlTvNames.Tokens.Seconds => XmlTvDurationUnit.Seconds,
            XmlTvNames.Tokens.Minutes => XmlTvDurationUnit.Minutes,
            XmlTvNames.Tokens.Hours => XmlTvDurationUnit.Hours,
            _ => throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV duration unit.")
        };
    }

    private XmlTvSubtitleType? ReadOptionalSubtitleTypeAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        return value switch
        {
            null => null,
            XmlTvNames.Tokens.Teletext => XmlTvSubtitleType.Teletext,
            XmlTvNames.Tokens.Onscreen => XmlTvSubtitleType.Onscreen,
            XmlTvNames.Tokens.DeafSigned => XmlTvSubtitleType.DeafSigned,
            _ => throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV subtitle type.")
        };
    }

    private XmlTvReviewType ReadRequiredReviewTypeAttribute(string name)
    {
        return RequiredAttribute(name) switch
        {
            XmlTvNames.Tokens.ReviewText => XmlTvReviewType.Text,
            XmlTvNames.Tokens.ReviewUrl => XmlTvReviewType.Url,
            _ => throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV review type.")
        };
    }

    private XmlTvImageType? ReadOptionalImageTypeAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        return value switch
        {
            null => null,
            XmlTvNames.Tokens.Poster => XmlTvImageType.Poster,
            XmlTvNames.Tokens.Backdrop => XmlTvImageType.Backdrop,
            XmlTvNames.Tokens.Still => XmlTvImageType.Still,
            XmlTvNames.Tokens.Person => XmlTvImageType.Person,
            XmlTvNames.Tokens.Character => XmlTvImageType.Character,
            _ => throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV image type.")
        };
    }

    private XmlTvImageSize? ReadOptionalImageSizeAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        return value switch
        {
            null => null,
            XmlTvNames.Tokens.ImageSmall => XmlTvImageSize.Small,
            XmlTvNames.Tokens.ImageMedium => XmlTvImageSize.Medium,
            XmlTvNames.Tokens.ImageLarge => XmlTvImageSize.Large,
            _ => throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV image size.")
        };
    }

    private XmlTvImageOrientation? ReadOptionalImageOrientationAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        return value switch
        {
            null => null,
            XmlTvNames.Tokens.Portrait => XmlTvImageOrientation.Portrait,
            XmlTvNames.Tokens.Landscape => XmlTvImageOrientation.Landscape,
            _ => throw ReadError($"Attribute '{name}' on '{_reader.Name}' is not a valid XMLTV image orientation.")
        };
    }

    private bool? ReadOptionalYesNoAttribute(string name)
    {
        var value = _reader.GetAttribute(name);
        return value switch
        {
            null => null,
            XmlTvNames.Tokens.Yes => true,
            XmlTvNames.Tokens.No => false,
            _ => throw ReadError($"Attribute '{name}' on '{_reader.Name}' must be 'yes' or 'no'.")
        };
    }

    private async Task<bool> ReadYesNoElementAsync(string elementName, CancellationToken cancellationToken)
    {
        ValidateAttributes();

        var value = await ReadElementTextAsync(elementName, cancellationToken).ConfigureAwait(false);
        return value switch
        {
            XmlTvNames.Tokens.Yes => true,
            XmlTvNames.Tokens.No => false,
            _ => throw ReadError($"Element '{elementName}' must contain 'yes' or 'no'.")
        };
    }

    private async Task<string> ReadElementTextAsync(string elementName, CancellationToken cancellationToken)
    {
        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return string.Empty;
        }

        try
        {
            var value = await _reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return value;
        }
        catch (XmlException exception)
        {
            throw ReadError($"Element '{elementName}' has invalid text content.", exception);
        }
        catch (InvalidOperationException exception)
        {
            throw ReadError($"Element '{elementName}' has invalid text content.", exception);
        }
    }

    private async Task ReadEmptyElementTailAsync(string elementName, CancellationToken cancellationToken)
    {
        if (_reader.IsEmptyElement)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        await ReadAsync(cancellationToken).ConfigureAwait(false);
        if (_reader is { NodeType: XmlNodeType.EndElement, Name: var name } && name == elementName)
        {
            await ReadAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        throw ReadError($"Element '{elementName}' must be empty.");
    }

    private static void AddRange<T>(Collection<T> target, IEnumerable<T>? source)
    {
        if (source is null)
        {
            return;
        }

        foreach (var item in source)
        {
            target.Add(item);
        }
    }
}
