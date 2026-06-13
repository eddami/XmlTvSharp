# Models

XmlTvSharp models follow XMLTV element names where practical.

The main document model is `XmlTvDocument`:

```csharp
var document = new XmlTvDocument();

document.Metadata.GeneratorInfoName = "My App";
document.Channels.Add(new XmlTvChannel("channel-one", "Channel One"));
document.Programmes.Add(
    new XmlTvProgramme(
        XmlTvDateTime.Parse("20260605120000 +0000"),
        "channel-one",
        "News"));
```

## Document

`XmlTvDocument` contains:

- `Metadata` for root `<tv>` attributes.
- `Channels` for `<channel>` elements.
- `Programmes` for `<programme>` elements.

## Channels

Channels require:

- `Id`
- at least one `DisplayNames` item

Optional repeated values such as `Icons` and `Urls` are exposed as mutable collections.

## Programmes

Programmes require:

- `Start`
- `ChannelId`
- at least one `Titles` item

Programmes expose optional values such as `Stop`, `Date`, `Credits`, `Ratings`, `EpisodeNumbers`, and `Extensions`.

## Collections

Repeated elements are exposed as non-null mutable collections:

```csharp
programme.Categories.Add(new XmlTvLocalizedText("News"));
programme.Urls.Add(new XmlTvUrl("https://example.test/programme"));
```

The model is editable. The writer validates required fields when writing.

An empty collection means no repeated elements are present. For elements where
XMLTV permits empty text, an item with `Value == string.Empty` means the element
is present with empty text:

```csharp
// No desc element.
programme.Descriptions.Clear();

// Present empty desc element.
programme.Descriptions.Add(new XmlTvLocalizedText(""));
```

## Value Types

Common leaf values have dedicated models:

- `XmlTvLocalizedText`
- `XmlTvDateTime`
- `XmlTvDuration`
- `XmlTvIcon`
- `XmlTvUrl`
- `XmlTvEpisodeNumber`
- `XmlTvRating`
- `XmlTvImage`
