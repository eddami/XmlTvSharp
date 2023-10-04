[![Build](https://github.com/eddami/XmlTvSharp/actions/workflows/release.yml/badge.svg)](https://github.com/eddami/XmlTvSharp/actions/workflows/release.yml)
[![GitHub tag (with filter)](https://img.shields.io/github/v/tag/eddami/XmlTvSharp)](https://github.com/eddami/XmlTvSharp/tags)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/eddami/XmlTvSharp/blob/main/LICENSE)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/XmlTvSharp)](https://www.nuget.org/packages/XmlTvSharp)
[![Nuget](https://img.shields.io/nuget/dt/XmlTvSharp)](https://www.nuget.org/packages/XmlTvSharp)

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

### Basic Usage: Reading All XMLTV Elements

```csharp
// Specify the path to the XML file containing TV program information
var xmlFilePath = "path/to/your/xmltv/file.xml";

// Cancellation token
var cancellationToken = new CancellationToken();

// Customize the parsing behaviour
var settings = new XmlTvReaderSettings();

// Read all TV channels and programs asynchronously
var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings, cancellationToken);

// Access the parsed TV channels and programs
var channels = result.Channels;
var programs = result.Programmes;
```

### Basic Usage: Reading XMLTV Elements Sequentially

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
    FilterByProgrammeTime = null,
    DefaultLanguage = "en",
    TimeZone = TimeZoneInfo.Utc,
    IgnoreChannels = false,
    IgnoreProgrammes = false
};
```

- `FilterByChannelId`: A function to filter channels by their IDs.
- `FilterByProgrammeTime`: A function to filter programs by their start and stop times.
- `DefaultLanguage`: Default language to use if language information is not available in the XML data.
- `TimeZone`: Time zone to convert program start and stop times. Default is UTC.
- `IgnoreChannels`: Set to true to ignore channel elements during parsing.
- `IgnoreProgrammes`: Set to true to ignore program elements during parsing.

**Example Usage:**

```csharp
var settings = new XmlTvReaderSettings
{
    FilterByChannelId = channelId => channelId.StartsWith("custom_"),
    FilterByProgrammeTime = (startTime, endTime) => startTime.DayOfWeek == DayOfWeek.Monday && endTime.Hour < 18,
    DefaultLanguage = "fr", // Set default language to French
    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), // Set time zone to EST
    IgnoreChannels = false, // Do not ignore channel elements
    IgnoreProgrammes = true // Ignore program elements during parsing
};
```

## Contributing

We welcome your contributions to this project. If you find a bug, have a feature request, or want to contribute in any
other way, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License - see
the [LICENSE](https://github.com/eddami/XmlTvSharp/blob/main/LICENSE) file for details.