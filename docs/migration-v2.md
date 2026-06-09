# Migrating to v2

v2 replaces the old reader-only API with a typed document model, explicit reader options, explicit read filters, and XMLTV date/time values.

## Package Install

The package name is unchanged:

```bash
dotnet add package XmlTvSharp
```

## Reading a Complete File

v1 used `ReadAllAsync`:

```csharp
var settings = new XmlTvReaderSettings();
var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings, cancellationToken);
```

v2 uses `ReadAsync`:

```csharp
var options = new XmlTvReaderOptions();
XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml", options, cancellationToken);
```

The result is now an `XmlTvDocument` with `Metadata`, `Channels`, and `Programmes`.

## Reading One Element at a Time

v1 used `ReadAsync` on the reader:

```csharp
while ((element = await reader.ReadAsync(cancellationToken)) != null)
{
    // Process channel or programme.
}
```

v2 uses `ReadElementAsync`:

```csharp
using var reader = new XmlTvReader("guide.xml");

XmlTvMetadata metadata = await reader.ReadMetadataAsync(cancellationToken);

while (await reader.ReadElementAsync(cancellationToken) is { } element)
{
    switch (element)
    {
        case XmlTvChannel channel:
            break;

        case XmlTvProgramme programme:
            break;
    }
}
```

Root `<tv>` metadata is read separately with `ReadMetadataAsync`.

## Reader Settings

`XmlTvReaderSettings` was replaced by two concepts:

- `XmlTvReaderOptions` for parser behavior.
- `XmlTvReadFilter` for branch filtering.

## Channel and Programme Filtering

v1 exposed predicate settings such as `FilterByChannelId` and `FilterByProgrammeChannelId`.

v2 uses an explicit filter builder for stream-skippable branch selection:

```csharp
XmlTvReadFilter filter = XmlTvReadFilter
    .Create()
    .IncludeChannels(channels => channels.WithIds("channel-one"))
    .IncludeProgrammes(programmes => programmes.ForChannels("channel-one"))
    .Build();

XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml", filter: filter);
```

v2 does not provide predicate-based time filtering. For custom logic, use forward-only reading and filter after each programme is parsed.

## Ignoring Channels or Programmes

v1 had `IgnoreChannels` and `IgnoreProgrammes`.

v2 uses `XmlTvReadFilter`:

```csharp
// Read programmes only.
XmlTvReadFilter filter = XmlTvReadFilter
    .Create()
    .IncludeProgrammes()
    .Build();
```

Building a filter without included branches reads root metadata only:

```csharp
XmlTvReadFilter metadataOnly = XmlTvReadFilter.Create().Build();
```

A `null` filter reads all supported channels and programmes.

## Date and Time

v1 converted programme times with a configured `TimeZone`.

v2 preserves XMLTV date/time values with `XmlTvDateTime`:

```csharp
XmlTvDateTime start = programme.Start;
string sourceValue = start.ToXmlTvString();
```

Convert to `DateTimeOffset` only when the value represents an exact instant:

```csharp
DateTimeOffset instant = programme.Start.ToDateTimeOffset();
```

Named timezone tokens require an explicit resolver. See [Date and Time](./date-time.md).

## Default Language

v1 had `DefaultLanguage`.

v2 preserves the language value supplied by the feed. Missing language stays missing:

```csharp
XmlTvLocalizedText title = programme.Titles[0];
string? language = title.Language;
```

Apply application defaults in your own code when you need them.

## Outer XML

v1 had `IncludeOuterXml`.

v2 does not put raw outer XML on models. Use the typed model plus `XmlTvWriter` when you need to reconstruct output.

## Unknown Content

Configure unknown elements and attributes through `XmlTvReaderOptions`:

```csharp
var options = new XmlTvReaderOptions
{
    UnknownElementHandling = XmlTvUnknownContentHandling.Ignore,
    UnknownAttributeHandling = XmlTvUnknownContentHandling.Ignore
};
```

The default behavior favors correctness and reports unsupported content.
