# Reading

XmlTvSharp provides two reading styles:

- Complete document reads with `XmlTvReader.ReadAsync`.
- Forward-only reads with `XmlTvReader`.

## Complete Document

Use the complete-document API when you want channels and programmes collected into one model:

```csharp
XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml");
```

The returned document contains root metadata, channels, and programmes:

```csharp
var metadata = document.Metadata;
var channels = document.Channels;
var programmes = document.Programmes;
```

## Forward-Only Reading

Use `ReadMetadataAsync` and `ReadElementAsync` when you want to process top-level elements one at a time:

```csharp
using var reader = new XmlTvReader("guide.xml");

XmlTvMetadata metadata = await reader.ReadMetadataAsync();

while (await reader.ReadElementAsync() is { } element)
{
    switch (element)
    {
        case XmlTvChannel channel:
            Console.WriteLine(channel.Id);
            break;

        case XmlTvProgramme programme:
            Console.WriteLine(programme.ChannelId);
            break;
    }
}
```

`ReadElementAsync` returns supported top-level child elements. Root `<tv>` metadata is read through `ReadMetadataAsync`.

## Filtering

Use a read filter when you want to skip unsupported branches before models are materialized. See [Filtering While Reading](./filtering.md).

## Options

Use `XmlTvReaderOptions` for compatibility profiles and unknown-content handling. See [Reader Options](./reader-options.md).
