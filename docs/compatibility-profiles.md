# Compatibility Profiles

Compatibility profiles enable supported dialect behavior.

The default profile is `XmlTvCompatibilityProfile.Standard`. Use a different profile only when your feed or target consumer expects a known dialect.

## Reader Profile

```csharp
var options = new XmlTvReaderOptions
{
    CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
};

XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml", options);
```

## Writer Profile

```csharp
var options = new XmlTvWriterOptions
{
    CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
};

await XmlTvWriter.WriteAsync(document, "guide.xml", options);
```

## Jellyfin

The Jellyfin profile supports Jellyfin-specific programme extension data:

```csharp
using XmlTvSharp.Models.Extensions;

var programme = new XmlTvProgramme(
    XmlTvDateTime.Parse("20260605120000 +0000"),
    "channel-one",
    "Live News")
{
    Extensions = new XmlTvProgrammeExtensions
    {
        Jellyfin = new XmlTvJellyfinProgrammeExtensions
        {
            IsLive = true
        }
    }
};
```

Extension containers are optional. Standard programmes do not need to allocate or assign them.
