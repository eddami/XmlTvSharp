[![Build](https://github.com/eddami/XmlTvSharp/actions/workflows/build.yml/badge.svg)](https://github.com/eddami/XmlTvSharp/actions/workflows/build.yml)
[![Release](https://github.com/eddami/XmlTvSharp/actions/workflows/release.yml/badge.svg)](https://github.com/eddami/XmlTvSharp/actions/workflows/release.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/eddami/XmlTvSharp/blob/main/LICENSE)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/XmlTvSharp)](https://www.nuget.org/packages/XmlTvSharp)
[![Nuget](https://img.shields.io/nuget/dt/XmlTvSharp)](https://www.nuget.org/packages/XmlTvSharp)
[![Coverage Status](https://coveralls.io/repos/github/eddami/XmlTvSharp/badge.svg?branch=main)](https://coveralls.io/github/eddami/XmlTvSharp?branch=main)

# XmlTvSharp

A high-performance, asynchronous XMLTV parser for TV program data.

## Installation

You can install this library via NuGet Package Manager:

```bash
Install-Package XmlTvSharp
```

## Benchmark

We tested the library using an XMLTV file featuring 19,804 channels and 1,979,805 programmes.

```
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 7.0.109
[Host]     : .NET 7.0.9 (7.0.923.32301), X64 RyuJIT AVX2
DefaultJob : .NET 7.0.9 (7.0.923.32301), X64 RyuJIT AVX2


|       Method |    Mean |   Error |  StdDev |
|------------- |--------:|--------:|--------:|
| ReadAllAsync | 20.65 s | 0.293 s | 0.229 s |
```

## Usage

### Reading All XMLTV Elements

```csharp
// Specify the path to the XML file containing TV program information
var xmlFilePath = "path/to/your/xmltv/file.xml";

// Cancellation token
var cancellationToken = new CancellationToken();

// Customize the parsing behaviour
var settings = new XmlTvReaderSettings();

// Read all TV channels and programmes asynchronously
var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings, cancellationToken);

// Access the parsed TV channels and programmes
var channels = result.Channels;
var programmes = result.Programmes;
```

### Reading XMLTV Elements Sequentially

```csharp
// Specify the path to the XML file containing TV program information
var xmlFilePath = "path/to/your/xmltv/file.xml";

// Cancellation token
var cancellationToken = new CancellationToken();

// Customize the parsing behaviour
var settings = new XmlTvReaderSettings();

using var reader = new XmlTvReader(xmlFilePath, settings);

IXmlTvElement element;
// Read XMLTV elements sequentially asynchronously
while ((element = await reader.ReadAsync(cancellationToken)) != null)
{
    if (element is XmlTvChannel channel)
    {
        // Process the parsed channel element
    }
    else if (element is XmlTvProgramme programme)
    {
        // Process the parsed programme element
    }
}
```

### XmlTvReaderSettings

XmlTvReaderSettings allows customization of the parsing behavior. Here are the default values:

```csharp
var settings = new XmlTvReaderSettings
{
    FilterByChannelId = null,
    FilterByProgrammeChannelId = null,
    FilterByProgrammeTime = null,
    DefaultLanguage = "en",
    TimeZone = TimeZoneInfo.Utc,
    IgnoreChannels = false,
    IgnoreProgrammes = false,
    IncludeOuterXml = false
};
```

- `FilterByChannelId`: A function to filter all elements by their channel IDs.
- `FilterByProgrammeChannelId`: A function to filter programme elements by their channel IDs.

**Note:** When both `FilterByChannelId` and `FilterByProgrammeChannelId` are set, `FilterByProgrammeChannelId` takes
precedence over `FilterByChannelId` for filtering programme elements by their channel IDs.

- `FilterByProgrammeTime`: A function to filter programmes by their start and stop times.
- `DefaultLanguage`: Default language to use if language information is not available in the XML data.
- `TimeZone`: Time zone to convert programme start and stop times. Default is UTC.
- `IgnoreChannels`: Set to true to ignore channel elements during parsing.
- `IgnoreProgrammes`: Set to true to ignore programme elements during parsing.
- `IncludeOuterXml`: Set to true to include the outer XML of elements during parsing.

**Warning:** Setting `IncludeOuterXml` to `true` will cause the parser to allocate an extra `XmlReader` instance,
potentially impacting performance.

**Example Usage:**

```csharp
var settings = new XmlTvReaderSettings
{
    FilterByChannelId = channelId => channelId.StartsWith("custom_"),
    FilterByProgrammeChannelId = channelId => channelId.StartsWith("custom_programme_"),
    FilterByProgrammeTime = (startTime, endTime) => startTime.DayOfWeek == DayOfWeek.Monday && endTime.Hour < 18,
    DefaultLanguage = "fr", // Set default language to French
    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), // Set time zone to EST
    IgnoreChannels = false, // Do not ignore channel elements
    IgnoreProgrammes = true, // Ignore programme elements during parsing
    IncludeOuterXml = true // Include outer XML of elements during parsing
};
```

## Contributing

We welcome your contributions to this project. If you find a bug, have a feature request, or want to contribute in any
other way, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License - see
the [LICENSE](https://github.com/eddami/XmlTvSharp/blob/main/LICENSE) file for details.