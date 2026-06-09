[![Build](https://github.com/eddami/XmlTvSharp/actions/workflows/build.yml/badge.svg)](https://github.com/eddami/XmlTvSharp/actions/workflows/build.yml)
[![Release](https://github.com/eddami/XmlTvSharp/actions/workflows/release.yml/badge.svg)](https://github.com/eddami/XmlTvSharp/actions/workflows/release.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/eddami/XmlTvSharp/blob/main/LICENSE)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/XmlTvSharp)](https://www.nuget.org/packages/XmlTvSharp)
[![Nuget](https://img.shields.io/nuget/dt/XmlTvSharp)](https://www.nuget.org/packages/XmlTvSharp)
[![Coverage Status](https://coveralls.io/repos/github/eddami/XmlTvSharp/badge.svg?branch=main)](https://coveralls.io/github/eddami/XmlTvSharp?branch=main)

# XmlTvSharp

A high-performance XMLTV reader and writer for .NET.

## Features

- Read and write XMLTV documents.
- Forward-only access to top-level channels and programmes.
- Preserve XMLTV date/time values with `XmlTvDateTime`.
- Use the standard XMLTV profile or supported compatibility profiles such as Jellyfin.
- Support .NET 8 and newer, with .NET Standard 2.0 compatibility for older runtimes.

## Installation

```bash
dotnet add package XmlTvSharp
```

XmlTvSharp supports .NET 8 and newer, including .NET 10. Older compatible runtimes can use the .NET Standard 2.0 build.

## Reading

```csharp
using XmlTvSharp;
using XmlTvSharp.Models;

// Load the whole XMLTV document when you want channels and programmes in memory.
XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml");

foreach (XmlTvChannel channel in document.Channels)
{
    Console.WriteLine($"{channel.Id}: {channel.DisplayNames[0].Value}");
}

foreach (XmlTvProgramme programme in document.Programmes)
{
    Console.WriteLine($"{programme.Start.ToXmlTvString()} {programme.ChannelId}");
}
```

## Writing

```csharp
using XmlTvSharp;
using XmlTvSharp.Models;

var document = new XmlTvDocument();

// Build a minimal XMLTV document with one channel and one programme.
document.Channels.Add(new XmlTvChannel("channel-one", "Channel One"));
document.Programmes.Add(
    new XmlTvProgramme(
        XmlTvDateTime.Parse("20260605120000 +0000"),
        "channel-one",
        "News"));

await XmlTvWriter.WriteAsync(document, "guide.xml");
```

## Forward-Only Reading

```csharp
using XmlTvSharp;
using XmlTvSharp.Models;

using var reader = new XmlTvReader("guide.xml");

// Read root <tv> metadata once, then read top-level children forward-only.
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

## Additional APIs

- `XmlTvReader` and `XmlTvWriter` provide forward-only read and write APIs.
- `XmlTvReadFilter` includes channel and programme branches by ID.
- `XmlTvReaderOptions` and `XmlTvWriterOptions` configure compatibility profiles and XML output.

## License

This project is licensed under the MIT License. See [LICENSE](https://github.com/eddami/XmlTvSharp/blob/main/LICENSE).
