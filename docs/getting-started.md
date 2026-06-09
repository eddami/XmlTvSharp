# Getting Started

Install the package from NuGet:

```bash
dotnet add package XmlTvSharp
```

XmlTvSharp supports .NET 8 and newer, including .NET 10. Older compatible runtimes can use the .NET Standard 2.0 build.

## Read a Document

Use `XmlTvReader.ReadAsync` when you want the complete guide in memory:

```csharp
using XmlTvSharp;
using XmlTvSharp.Models;

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

## Write a Document

Create an `XmlTvDocument`, add channels and programmes, then write it:

```csharp
using XmlTvSharp;
using XmlTvSharp.Models;

var document = new XmlTvDocument();

document.Channels.Add(new XmlTvChannel("channel-one", "Channel One"));
document.Programmes.Add(
    new XmlTvProgramme(
        XmlTvDateTime.Parse("20260605120000 +0000"),
        "channel-one",
        "News"));

await XmlTvWriter.WriteAsync(document, "guide.xml");
```

## Namespaces

Most model types live under `XmlTvSharp.Models`:

```csharp
using XmlTvSharp;
using XmlTvSharp.Models;
```
