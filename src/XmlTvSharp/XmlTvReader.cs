using System.Globalization;
using System.Xml;

namespace XmlTvSharp;

/// <summary>
/// Represents a reader for parsing XML data containing TV program information in XMLTV format.
/// </summary>
public class XmlTvReader : IDisposable
{
    private static readonly string[] StartStopFormats = { "yyyyMMddHmmss zzz", "yyyyMMddHmmss" };
    private static readonly string[] DateFormats = { "yyyyMMdd", "yyyy" };

    private readonly XmlReader _reader;
    private readonly ParsingContext _context;

    /// <summary>
    /// Initializes a new instance of the XmlTvReader class with the specified XML file path and optional settings.
    /// </summary>
    /// <param name="path">The path to the XML file containing TV program information in XMLTV format.</param>
    /// <param name="settings">Optional settings for customizing the XMLTV parsing behavior.</param>
    public XmlTvReader(string path, XmlTvReaderSettings? settings = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        _reader = CreateXmlReader(path);

        _context = new ParsingContext(settings ?? new XmlTvReaderSettings());
    }

    /// <summary>
    /// Initializes a new instance of the XmlTvReader class with the specified stream and optional settings.
    /// </summary>
    /// <param name="stream">The stream containing TV program information in XMLTV format.</param>
    /// <param name="settings">Optional settings for customizing the XMLTV parsing behavior.</param>
    public XmlTvReader(Stream stream, XmlTvReaderSettings? settings = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        _reader = CreateXmlReader(stream);

        _context = new ParsingContext(settings ?? new XmlTvReaderSettings());
    }

    /// <summary>
    /// Initializes a new instance of the XmlTvReader class with the specified TextReader and optional settings.
    /// </summary>
    /// <param name="textReader">The TextReader containing TV program information in XMLTV format.</param>
    /// <param name="settings">Optional settings for customizing the XMLTV parsing behavior.</param>
    public XmlTvReader(TextReader textReader, XmlTvReaderSettings? settings = default)
    {
        if (textReader == null) throw new ArgumentNullException(nameof(textReader));

        _reader = CreateXmlReader(textReader);

        _context = new ParsingContext(settings ?? new XmlTvReaderSettings());
    }

    /// <summary>
    /// Reads all XMLTV elements from the specified XML file path asynchronously and returns the parsed result.
    /// </summary>
    /// <param name="path">The path to the XML file containing TV program information in XMLTV format.</param>
    /// <param name="settings">Optional settings for customizing the XMLTV parsing behavior.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>An instance of XmlTvResult containing the parsed TV channels and programs.</returns>
    public static Task<XmlTvResult> ReadAllAsync(string path, XmlTvReaderSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var reader = CreateXmlReader(path);

        return InternalReadAllAsync(reader, settings, cancellationToken);
    }

    /// <summary>
    /// Reads all XMLTV elements from the specified stream asynchronously and returns the parsed result.
    /// </summary>
    /// <param name="stream">The stream containing TV program information in XMLTV format.</param>
    /// <param name="settings">Optional settings for customizing the XMLTV parsing behavior.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>An instance of XmlTvResult containing the parsed TV channels and programs.</returns>
    public static Task<XmlTvResult> ReadAllAsync(Stream stream, XmlTvReaderSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var reader = CreateXmlReader(stream);

        return InternalReadAllAsync(reader, settings, cancellationToken);
    }

    /// <summary>
    /// Reads all XMLTV elements from the specified TextReader asynchronously and returns the parsed result.
    /// </summary>
    /// <param name="textReader">The TextReader containing TV program information in XMLTV format.</param>
    /// <param name="settings">Optional settings for customizing the XMLTV parsing behavior.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>An instance of XmlTvResult containing the parsed TV channels and programs.</returns>
    public static Task<XmlTvResult> ReadAllAsync(TextReader textReader, XmlTvReaderSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        if (textReader == null) throw new ArgumentNullException(nameof(textReader));

        var reader = CreateXmlReader(textReader);

        return InternalReadAllAsync(reader, settings, cancellationToken);
    }

    /// <summary>
    /// Reads the next XMLTV element asynchronously from the XML data and returns it.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>
    /// An instance of the interface IXmlTvElement representing the next XMLTV element read from the data.
    /// Returns null if the end of the XML data is reached.
    /// </returns>
    public async Task<IXmlTvElement?> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Settings is { IgnoreChannels: true, IgnoreProgrammes: true })
        {
            return default;
        }

        while (await _reader.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_reader.NodeType == XmlNodeType.Element)
            {
                switch (_reader.Name)
                {
                    case "channel":
                        if (_context.Settings.IgnoreChannels)
                        {
                            break;
                        }

                        await HandleChannelElement(_reader, _context);
                        if (_context.Channel != null)
                        {
                            return _context.Channel;
                        }

                        break;
                    case "programme":
                        if (_context.Settings.IgnoreProgrammes)
                        {
                            return default;
                        }

                        await HandleProgrammeElement(_reader, _context);
                        if (_context.Programme != null)
                        {
                            return _context.Programme;
                        }

                        break;
                }
            }
        }

        return default;
    }

    private static async Task<XmlTvResult> InternalReadAllAsync(XmlReader reader,
        XmlTvReaderSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        var result = new XmlTvResult();

        var context = new ParsingContext(settings ?? new XmlTvReaderSettings());

        if (context.Settings is { IgnoreChannels: true, IgnoreProgrammes: true })
        {
            return result;
        }

        try
        {
            while (await reader.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "channel":
                            if (context.Settings.IgnoreChannels)
                            {
                                break;
                            }

                            await HandleChannelElement(reader, context, result);
                            break;
                        case "programme":
                            if (context.Settings.IgnoreProgrammes)
                            {
                                return result;
                            }

                            await HandleProgrammeElement(reader, context, result);
                            break;
                    }
                }
            }
        }
        finally
        {
            reader.Dispose();
        }

        return result;
    }

    private static async Task HandlePremiereElement(XmlReader reader, ParsingContext context)
    {
        context.Programme!.IsPremiere = true;
        context.Programme.PremiereLanguage = reader.GetAttribute("lang");

        if (!reader.IsEmptyElement)
        {
            await reader.ReadAsync();
            context.Programme.Premiere = await reader.ReadContentAsStringAsync();
        }
    }

    private static void HandleNewElement(ParsingContext context)
    {
        context.Programme!.IsNew = true;
    }

    private static async Task HandleQualityElement(XmlReader reader, ParsingContext context)
    {
        await reader.ReadAsync();
        context.Programme!.Quality = await reader.ReadContentAsStringAsync();
    }

    private static async Task HandleCountryElement(XmlReader reader, ParsingContext context)
    {
        await reader.ReadAsync();
        context.Programme!.Countries ??= new List<string>();
        context.Programme.Countries.Add(await reader.ReadContentAsStringAsync());
    }

    private static async Task HandleCategoryElement(XmlReader reader, ParsingContext context)
    {
        await reader.ReadAsync();
        context.Programme!.Categories ??= new List<string>();
        context.Programme.Categories.Add(await reader.ReadContentAsStringAsync());
    }

    private static async Task HandleLanguageElement(XmlReader reader, ParsingContext context)
    {
        await reader.ReadAsync();
        context.Programme!.Language = await reader.ReadContentAsStringAsync();
    }

    private static async Task HandleEpisodeElement(XmlReader reader, ParsingContext context)
    {
        var system = reader.GetAttribute("system");
        await reader.ReadAsync();
        context.Programme!.Episodes ??= new List<XmlTvEpisode>();
        context.Programme.Episodes.Add(new XmlTvEpisode
        {
            System = system,
            Value = await reader.ReadContentAsStringAsync()
        });
    }

    private static async Task HandleStarRatingElement(XmlReader reader, ParsingContext context)
    {
        if (await reader.ReadAsync())
        {
            if (reader is { NodeType: XmlNodeType.Element, Name: "value", IsEmptyElement: false })
            {
                await reader.ReadAsync();
                context.Programme!.StarRating = await reader.ReadContentAsStringAsync();
            }
        }
    }

    private static async Task HandleRatingElement(XmlReader reader, ParsingContext context)
    {
        var system = reader.GetAttribute("system") ?? string.Empty;
        var rating = new XmlTvRating();

        context.Programme!.Ratings ??= new Dictionary<string, XmlTvRating>();
        context.Programme.Ratings[system] = rating;

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "value" when !reader.IsEmptyElement:
                        await reader.ReadAsync();
                        rating.Value = await reader.ReadContentAsStringAsync();
                        break;
                    case "icon":
                        var icon = ReadIconElement(reader);
                        if (icon != null)
                        {
                            rating.Icons.Add(icon);
                        }

                        break;
                }
            }

            if (reader is { NodeType: XmlNodeType.EndElement, Name: "rating" })
            {
                break;
            }
        }
    }

    private static async Task HandleActorElement(XmlReader reader, ParsingContext context)
    {
        await reader.ReadAsync();
        context.Programme!.Actors ??= new List<string>();
        context.Programme.Actors.Add(await reader.ReadContentAsStringAsync());
    }

    private static void HandlePreviouslyShownElement(XmlReader reader, ParsingContext context)
    {
        context.Programme!.IsPreviouslyShown = true;
        context.Programme.PreviouslyShownChannel = reader.GetAttribute("channel");
        var date = reader.GetAttribute("start");
        if (DateTimeOffset.TryParseExact(date, StartStopFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var prevShownDate))
        {
            context.Programme.PreviouslyShownDate = TimeZoneInfo.ConvertTime(prevShownDate, context.Settings.TimeZone);
        }
    }

    private static async Task HandleDateElement(XmlReader reader, ParsingContext context)
    {
        await reader.ReadAsync();
        var dateStr = await reader.ReadContentAsStringAsync();

        if (DateTimeOffset.TryParseExact(dateStr, DateFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var date))
        {
            context.Programme!.Date = date;
        }
    }

    private static async Task HandleDescriptionElement(XmlReader reader, ParsingContext context)
    {
        var lang = reader.GetAttribute("lang") ?? context.Settings.DefaultLanguage;
        await reader.ReadAsync();
        var description = await reader.ReadContentAsStringAsync();
        context.Programme!.Descriptions ??= new Dictionary<string, string>();
        context.Programme.Descriptions[lang] = description;
    }

    private static async Task HandleSubTitleElement(XmlReader reader, ParsingContext context)
    {
        var lang = reader.GetAttribute("lang") ?? context.Settings.DefaultLanguage;
        await reader.ReadAsync();
        var title = await reader.ReadContentAsStringAsync();
        context.Programme!.SubTitles ??= new Dictionary<string, string>();
        context.Programme.SubTitles[lang] = title;
    }

    private static async Task HandleTitleElement(XmlReader reader, ParsingContext context)
    {
        var lang = reader.GetAttribute("lang") ?? context.Settings.DefaultLanguage;
        await reader.ReadAsync();
        var title = await reader.ReadContentAsStringAsync();
        context.Programme!.Titles[lang] = title;
    }

    private static async Task HandleProgrammeElement(XmlReader reader, ParsingContext context,
        XmlTvResult? result = null)
    {
        var skip = false;

        var channelIdFilter = context.Settings.FilterByProgrammeChannelId ?? context.Settings.FilterByChannelId;
        var timeFilter = context.Settings.FilterByProgrammeTime;

        var startString = reader.GetAttribute("start");
        var stopString = reader.GetAttribute("stop");
        var channelId = reader.GetAttribute("channel");

        if (DateTimeOffset.TryParseExact(startString, StartStopFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var start) &&
            DateTimeOffset.TryParseExact(stopString, StartStopFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var stop) && !string.IsNullOrWhiteSpace(channelId) &&
            (channelIdFilter is null || channelIdFilter(channelId)) &&
            (timeFilter is null || timeFilter(start, stop)))

        {
            var currentProgramme = new XmlTvProgramme
            {
                Start = TimeZoneInfo.ConvertTime(start, context.Settings.TimeZone),
                Stop = TimeZoneInfo.ConvertTime(stop, context.Settings.TimeZone),
                ChannelId = channelId
            };
            result?.Programmes.Add(currentProgramme);
            context.Programme = currentProgramme;

            if (context.Settings.IncludeOuterXml)
            {
                var subTree = reader.ReadSubtree();
                await subTree.ReadAsync();
                currentProgramme.OuterXml = await subTree.ReadOuterXmlAsync();
            }
        }
        else
        {
            skip = true;
        }

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element && !skip)
            {
                switch (reader.Name)
                {
                    case "title" when !reader.IsEmptyElement:
                        await HandleTitleElement(reader, context);
                        break;
                    case "sub-title" when !reader.IsEmptyElement:
                        await HandleSubTitleElement(reader, context);
                        break;
                    case "desc" when !reader.IsEmptyElement:
                        await HandleDescriptionElement(reader, context);
                        break;
                    case "date" when !reader.IsEmptyElement:
                        await HandleDateElement(reader, context);
                        break;
                    case "previously-shown":
                        HandlePreviouslyShownElement(reader, context);
                        break;
                    case "actor" when !reader.IsEmptyElement:
                        await HandleActorElement(reader, context);
                        break;
                    case "credits":
                        await HandleCreditsElement(reader, context);
                        break;
                    case "rating":
                        await HandleRatingElement(reader, context);
                        break;
                    case "star-rating":
                        await HandleStarRatingElement(reader, context);
                        break;
                    case "episode-num" when !reader.IsEmptyElement:
                        await HandleEpisodeElement(reader, context);
                        break;
                    case "language" when !reader.IsEmptyElement:
                        await HandleLanguageElement(reader, context);
                        break;
                    case "category" when !reader.IsEmptyElement:
                        await HandleCategoryElement(reader, context);
                        break;
                    case "country" when !reader.IsEmptyElement:
                        await HandleCountryElement(reader, context);
                        break;
                    case "quality" when !reader.IsEmptyElement:
                        await HandleQualityElement(reader, context);
                        break;
                    case "new":
                        HandleNewElement(context);
                        break;
                    case "premiere":
                        await HandlePremiereElement(reader, context);
                        break;
                    case "icon":
                        var icon = ReadIconElement(reader);
                        if (icon != null)
                        {
                            context.Programme!.Icons ??= new List<XmlTvIcon>();
                            context.Programme.Icons.Add(icon);
                        }

                        break;
                    case "url" when !reader.IsEmptyElement:
                        var url = await ReadUrlElement(reader);
                        context.Programme!.Urls ??= new List<XmlTvUrl>();
                        context.Programme.Urls.Add(url);

                        break;
                }
            }

            if (reader is { NodeType: XmlNodeType.EndElement, Name: "programme" })
            {
                return;
            }
        }
    }

    private static async Task HandleChannelElement(XmlReader reader, ParsingContext context, XmlTvResult? result = null)
    {
        var skip = false;
        var channelFilter = context.Settings.FilterByChannelId;

        var id = reader.GetAttribute("id");
        if (!string.IsNullOrWhiteSpace(id) && (channelFilter is null || channelFilter(id)))
        {
            var currentChannel = new XmlTvChannel
            {
                Id = id
            };
            result?.Channels.Add(currentChannel);
            context.Channel = currentChannel;

            if (context.Settings.IncludeOuterXml)
            {
                var subTree = reader.ReadSubtree();
                await subTree.ReadAsync();
                currentChannel.OuterXml = await subTree.ReadOuterXmlAsync();
            }
        }
        else
        {
            skip = true;
        }

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element && !skip)
            {
                switch (reader.Name)
                {
                    case "display-name" when !reader.IsEmptyElement:
                        await HandleDisplayNameElement(reader, context);
                        break;
                    case "icon":
                        var icon = ReadIconElement(reader);
                        if (icon != null)
                        {
                            context.Channel!.Icons ??= new List<XmlTvIcon>();
                            context.Channel.Icons.Add(icon);
                        }

                        break;
                    case "url" when !reader.IsEmptyElement:
                        context.Channel!.Urls ??= new List<XmlTvUrl>();
                        var url = await ReadUrlElement(reader);
                        context.Channel.Urls.Add(url);

                        break;
                }
            }

            if (reader is { NodeType: XmlNodeType.EndElement, Name: "channel" })
            {
                return;
            }
        }
    }

    private static async Task HandleDisplayNameElement(XmlReader reader, ParsingContext context)
    {
        var lang = reader.GetAttribute("lang") ?? context.Settings.DefaultLanguage;
        await reader.ReadAsync();
        var displayName = await reader.ReadContentAsStringAsync();

        context.Channel!.DisplayNames[lang] = displayName;
    }

    private static XmlTvIcon? ReadIconElement(XmlReader reader)
    {
        var iconSrc = reader.GetAttribute("src");
        var heightStr = reader.GetAttribute("height");
        var widthStr = reader.GetAttribute("width");

        int? height = int.TryParse(heightStr, out var h) ? h : null;
        int? width = int.TryParse(widthStr, out var w) ? w : null;

        if (string.IsNullOrWhiteSpace(iconSrc))
        {
            return default;
        }

        return new XmlTvIcon
        {
            Source = iconSrc,
            Height = height,
            Width = width
        };
    }

    private static async Task<XmlTvUrl> ReadUrlElement(XmlReader reader)
    {
        var system = reader.GetAttribute("system");
        await reader.ReadAsync();
        return new XmlTvUrl
        {
            System = system,
            Value = await reader.ReadContentAsStringAsync()
        };
    }

    private static async Task HandleCreditsElement(XmlReader reader, ParsingContext context)
    {
        var credits = new XmlTvCredits();
        context.Programme!.Credits = credits;

        while (await reader.ReadAsync())
        {
            if (reader is { NodeType: XmlNodeType.Element, IsEmptyElement: false })
            {
                switch (reader.Name)
                {
                    case "director":
                        await reader.ReadAsync();
                        credits.Directors ??= new List<string>();
                        credits.Directors.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "actor":
                        var actorRole = reader.GetAttribute("role");
                        await reader.ReadAsync();
                        var actorName = await reader.ReadContentAsStringAsync();
                        credits.Actors ??= new List<XmlTvPerson>();
                        credits.Actors.Add(new XmlTvPerson
                        {
                            Role = actorRole,
                            Name = actorName
                        });
                        break;
                    case "writer":
                        await reader.ReadAsync();
                        credits.Writers ??= new List<string>();
                        credits.Writers.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "adapter":
                        await reader.ReadAsync();
                        credits.Adapters ??= new List<string>();
                        credits.Adapters.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "producer":
                        await reader.ReadAsync();
                        credits.Producers ??= new List<string>();
                        credits.Producers.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "composer":
                        await reader.ReadAsync();
                        credits.Composers ??= new List<string>();
                        credits.Composers.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "editor":
                        await reader.ReadAsync();
                        credits.Editors ??= new List<string>();
                        credits.Editors.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "presenter":
                        await reader.ReadAsync();
                        credits.Presenters ??= new List<string>();
                        credits.Presenters.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "commentator":
                        await reader.ReadAsync();
                        credits.Commentators ??= new List<string>();
                        credits.Commentators.Add(await reader.ReadContentAsStringAsync());
                        break;
                    case "guest":
                        var guestRole = reader.GetAttribute("role") ?? string.Empty;
                        await reader.ReadAsync();
                        var guestName = await reader.ReadContentAsStringAsync();
                        credits.Guests ??= new List<XmlTvPerson>();
                        credits.Guests.Add(new XmlTvPerson
                        {
                            Role = guestRole,
                            Name = guestName
                        });
                        break;
                }
            }

            if (reader is { NodeType: XmlNodeType.EndElement, Name: "credits" })
            {
                return;
            }
        }
    }

    private class ParsingContext
    {
        public ParsingContext(XmlTvReaderSettings settings)
        {
            Settings = settings;
        }

        public XmlTvProgramme? Programme { get; set; }
        public XmlTvChannel? Channel { get; set; }
        public XmlTvReaderSettings Settings { get; set; }
    }

    private static XmlReader CreateXmlReader(string path)
    {
        return XmlReader.Create(path, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreWhitespace = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            Async = true
        });
    }

    private static XmlReader CreateXmlReader(Stream stream)
    {
        return XmlReader.Create(stream, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreWhitespace = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            Async = true
        });
    }

    private static XmlReader CreateXmlReader(TextReader reader)
    {
        return XmlReader.Create(reader, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreWhitespace = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            Async = true
        });
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _reader.Dispose();
        }
    }
}