# Filtering

Filters select top-level branches before models are materialized.

Use filters for simple stream-skippable decisions such as channel IDs and programme channel IDs. For arbitrary logic, read elements one at a time and apply your own code.

## Include Channels

```csharp
XmlTvReadFilter filter = XmlTvReadFilter
    .Create()
    .IncludeChannels(channels => channels.WithIds("channel-one", "channel-two"))
    .Build();

using var reader = new XmlTvReader("guide.xml", filter: filter);
```

## Include Programmes

```csharp
XmlTvReadFilter filter = XmlTvReadFilter
    .Create()
    .IncludeProgrammes(programmes => programmes.ForChannels("channel-one"))
    .Build();

using var reader = new XmlTvReader("guide.xml", filter: filter);
```

## Metadata Only

Building a filter without included branches reads root metadata only:

```csharp
XmlTvReadFilter filter = XmlTvReadFilter.Create().Build();

XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml", filter: filter);
```

## Date Expressions

XmlTvSharp does not provide programme time range filtering. Date/time values can have partial precision and named timezone tokens, so a general time comparison API would either be expensive or misleading.

Use forward-only reading when you need custom programme filtering:

```csharp
using var reader = new XmlTvReader("guide.xml");

while (await reader.ReadElementAsync() is { } element)
{
    if (element is not XmlTvProgramme programme)
    {
        continue;
    }

    // Apply application-specific filtering here.
}
```
