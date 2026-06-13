# Reader Options

Use `XmlTvReaderOptions` to configure how input is interpreted.

```csharp
var options = new XmlTvReaderOptions
{
    CompatibilityProfile = XmlTvCompatibilityProfile.Standard,
    UnknownElementHandling = XmlTvUnknownContentHandling.Disallow,
    UnknownAttributeHandling = XmlTvUnknownContentHandling.Disallow
};

XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml", options);
```

## Compatibility Profile

`CompatibilityProfile` selects dialect behavior.

The default is `XmlTvCompatibilityProfile.Standard`. See [Compatibility Profiles](./compatibility-profiles.md).

## Unknown Elements and Attributes

`UnknownElementHandling` controls unsupported elements.

`UnknownAttributeHandling` controls unsupported attributes.

The default is `Disallow`, which raises `XmlTvReadException` when unsupported content is encountered.

Use `Ignore` when you intentionally want to skip unknown content from feeds that contain provider-specific additions.

Unknown-content handling does not relax validation for recognized XMLTV content.
Required values such as programme titles and required attributes are still
validated.

## Reader Exceptions

Read failures are reported as `XmlTvReadException`. When line information is available, the exception includes `LineNumber` and `LinePosition`.
