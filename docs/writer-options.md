# Writer Options

Use `XmlTvWriterOptions` to configure XML output and compatibility behavior.

```csharp
var options = new XmlTvWriterOptions
{
    Indent = true,
    OmitXmlDeclaration = true,
    CompatibilityProfile = XmlTvCompatibilityProfile.Standard
};

await XmlTvWriter.WriteAsync(document, "guide.xml", options);
```

## Indent

`Indent` controls whether generated XML is formatted with indentation.

## XML Declaration

`OmitXmlDeclaration` controls whether the XML declaration is written.

## Compatibility Profile

`CompatibilityProfile` controls profile-specific output. See [Compatibility Profiles](./compatibility-profiles.md).

## Encoding

When writing to a `Stream` or file path, XmlTvSharp writes UTF-8 without a byte-order mark.

When writing to a `TextWriter`, the supplied writer controls the output encoding.

## Writer Exceptions

Write failures are reported as `XmlTvWriteException`. Model validation failures are reported before the invalid element is emitted when possible.
