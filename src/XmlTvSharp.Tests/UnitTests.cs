using System.Globalization;
using System.Text;

namespace XmlTvSharp.Tests;

public class UnitTests
{
    [Fact]
    public async Task ReadAllAsync_InvalidFilePath_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => XmlTvReader.ReadAllAsync(string.Empty));
        await Assert.ThrowsAsync<ArgumentNullException>(() => XmlTvReader.ReadAllAsync("  "));
    }

    [Fact]
    public async Task ReadAllAsync_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath);

        // Assert
        AssertValidXmlTvResult(result);
    }

    [Fact]
    public async Task ReadAllAsync_ValidXmlTvContentFromStringReader_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var xmlContent = await File.ReadAllTextAsync(xmlFilePath);
        var stringReader = new StringReader(xmlContent);

        // Act
        var result = await XmlTvReader.ReadAllAsync(stringReader);

        // Assert
        AssertValidXmlTvResult(result);
    }

    [Fact]
    public async Task ReadAllAsync_ValidXmlTvContentFromStream_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var xmlContent = await File.ReadAllTextAsync(xmlFilePath);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));

        // Act
        var result = await XmlTvReader.ReadAllAsync(stream);

        // Assert
        AssertValidXmlTvResult(result);
    }

    [Fact]
    public void ReadAsync_InvalidFilePath_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new XmlTvReader(string.Empty));
        Assert.Throws<ArgumentNullException>(() => new XmlTvReader("  "));
    }

    [Fact]
    public async Task ReadAsync_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        using var reader = new XmlTvReader(xmlFilePath);

        // Act
        var channel = await reader.ReadAsync();

        await reader.ReadAsync();
        var programme = await reader.ReadAsync();

        await reader.ReadAsync();
        var nullElement = await reader.ReadAsync();

        // Assert
        Assert.IsType<XmlTvChannel>(channel);
        AssertValidXmlTvChannel((XmlTvChannel)channel);

        Assert.IsType<XmlTvProgramme>(programme);
        AssertValidXmlTvProgramme((XmlTvProgramme)programme);

        Assert.Null(nullElement);
    }
    
    [Fact]
    public async Task ReadAsync_ValidXmlTvContentFromStringReader_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var xmlContent = await File.ReadAllTextAsync(xmlFilePath);
        var stringReader = new StringReader(xmlContent);
        using var reader = new XmlTvReader(stringReader);

        // Act
        var channel = await reader.ReadAsync();

        await reader.ReadAsync();
        var programme = await reader.ReadAsync();

        await reader.ReadAsync();
        var nullElement = await reader.ReadAsync();

        // Assert
        Assert.IsType<XmlTvChannel>(channel);
        AssertValidXmlTvChannel((XmlTvChannel)channel);

        Assert.IsType<XmlTvProgramme>(programme);
        AssertValidXmlTvProgramme((XmlTvProgramme)programme);

        Assert.Null(nullElement);
    }
    
    [Fact]
    public async Task ReadAsync_ValidXmlTvContentFromStream_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var xmlContent = await File.ReadAllTextAsync(xmlFilePath);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));
        using var reader = new XmlTvReader(stream);

        // Act
        var channel = await reader.ReadAsync();

        await reader.ReadAsync();
        var programme = await reader.ReadAsync();

        await reader.ReadAsync();
        var nullElement = await reader.ReadAsync();

        // Assert
        Assert.IsType<XmlTvChannel>(channel);
        AssertValidXmlTvChannel((XmlTvChannel)channel);

        Assert.IsType<XmlTvProgramme>(programme);
        AssertValidXmlTvProgramme((XmlTvProgramme)programme);

        Assert.Null(nullElement);
    }

    [Fact]
    public async Task ReadAllAsync_IgnoreChannels_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var settings = new XmlTvReaderSettings { IgnoreChannels = true };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Channels);
        Assert.NotEmpty(result.Programmes);
    }

    [Fact]
    public async Task ReadAsync_IgnoreChannels_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var settings = new XmlTvReaderSettings { IgnoreChannels = true };
        var reader = new XmlTvReader(xmlFilePath, settings);

        // Assert
        IXmlTvElement? element;
        while ((element = await reader.ReadAsync()) != null)
        {
            Assert.NotNull(element);
            Assert.IsType<XmlTvProgramme>(element);
        }

        Assert.Null(element);
    }

    [Fact]
    public async Task ReadAsync_IgnoreProgrammes_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var settings = new XmlTvReaderSettings { IgnoreProgrammes = true };
        var reader = new XmlTvReader(xmlFilePath, settings);

        // Assert
        IXmlTvElement? element;
        while ((element = await reader.ReadAsync()) != null)
        {
            Assert.NotNull(element);
            Assert.IsType<XmlTvChannel>(element);
        }

        Assert.Null(element);
    }

    [Fact]
    public async Task ReadAsync_IgnoreAll_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var settings = new XmlTvReaderSettings
        {
            IgnoreChannels = true,
            IgnoreProgrammes = true
        };
        var reader = new XmlTvReader(xmlFilePath, settings);

        // Act
        var element = await reader.ReadAsync();

        // Assert
        Assert.Null(element);
    }

    [Fact]
    public async Task ReadAllAsync_IgnoreProgrammes_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var settings = new XmlTvReaderSettings { IgnoreProgrammes = true };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Channels);
        Assert.Empty(result.Programmes);
    }

    [Fact]
    public async Task ReadAllAsync_IgnoreAll_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var settings = new XmlTvReaderSettings
        {
            IgnoreChannels = true,
            IgnoreProgrammes = true
        };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Channels);
        Assert.Empty(result.Programmes);
    }

    [Fact]
    public async Task ReadAllAsync_IncludeOuterXml_ValidXmlTvFile_ParsesSuccessfully()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        var settings = new XmlTvReaderSettings { IncludeOuterXml = true };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        const string channelXml =
            "<channel id=\"channel1\"><display-name lang=\"en\">Channel 1</display-name><icon src=\"channel1_icon.png\" height=\"50\" width=\"50\" /><url system=\"xmltv\">http://example.com/channel1</url></channel>";
        const string programmeXml =
            "<programme start=\"20220522120000\" stop=\"20220522130000\" channel=\"channel1\"><title lang=\"en\">Program 1</title><sub-title lang=\"en\">Subtitle 1</sub-title><desc lang=\"en\">Description of Program 1</desc><date>20220522</date><previously-shown channel=\"channel1\" start=\"20220521120000\" /><actor lang=\"en\">Actor 1</actor><credits><director>Director 1</director><actor role=\"Lead\">Lead Actor 1</actor><writer>Writer 1</writer><adapter>Adapter 1</adapter><producer>Producer 1</producer><composer>Composer 1</composer><editor>Editor 1</editor><presenter>Presenter 1</presenter><commentator>Commentator 1</commentator><guest role=\"Special\">Special Guest 1</guest></credits><rating system=\"MPAA\"><value>PG-13</value><icon src=\"mpaa_icon.png\" /></rating><star-rating><value>4.5</value></star-rating><episode-num system=\"onscreen\">S01E01</episode-num><language>English</language><category>Entertainment</category><country>USA</country><quality>HDTV</quality><new /><premiere lang=\"en\">Premiere of Program 1</premiere><icon src=\"program1_icon.png\" /><url system=\"official\">http://example.com/program1</url></programme>";

        Assert.NotNull(result);
        Assert.Equal(channelXml, result.Channels.FirstOrDefault()?.OuterXml);
        Assert.Equal(programmeXml, result.Programmes.FirstOrDefault()?.OuterXml);
    }

    [Fact]
    public async Task ReadAllAsync_FilterByChannelId_IncludesOnlySpecifiedElements()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        const string channelId = "channel1";

        var settings = new XmlTvReaderSettings
        {
            FilterByChannelId = id => id == channelId
        };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Channels);
        Assert.NotEmpty(result.Programmes);
        Assert.True(result.Channels.All(channel => channel.Id == channelId));
        Assert.True(result.Programmes.All(programme => programme.ChannelId == channelId));
    }

    [Fact]
    public async Task ReadAllAsync_FilterByProgrammeChannelId_IncludesOnlySpecifiedElements()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        const string channelId = "channel1";

        var settings = new XmlTvReaderSettings
        {
            FilterByProgrammeChannelId = id => id == channelId
        };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Channels);
        Assert.NotEmpty(result.Programmes);
        Assert.True(result.Programmes.All(programme => programme.ChannelId == channelId));
    }
    
    [Fact]
    public async Task ReadAllAsync_FilterByProgrammeChannelIdTakesPrecedence()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        const string channelId = "channel1";
        const string programmeChannelId = "channel2";

        var settings = new XmlTvReaderSettings
        {
            FilterByChannelId = id => id == channelId,
            FilterByProgrammeChannelId = id => id == programmeChannelId
        };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Channels.All(channel => channel.Id == channelId));
        Assert.True(result.Programmes.All(program => program.ChannelId == programmeChannelId));
    }

    [Fact]
    public async Task ReadAllAsync_CustomTimezone_ConvertsToSpecifiedTimezone()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";

        var settings = new XmlTvReaderSettings
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC+12")
        };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(12, result.Programmes.FirstOrDefault()?.Start.Offset.Hours);
    }

    [Fact]
    public async Task ReadAllAsync_FilterByProgrammeTime_IncludesOnlyProgrammesInTimeRange()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";

        var startTime = DateTimeOffset.Parse("2022-05-23T13:00:00+00:00");
        var endTime = DateTimeOffset.Parse("2022-05-23T14:00:00+00:00");

        var settings = new XmlTvReaderSettings
        {
            FilterByProgrammeTime = (start, stop) =>
                start >= startTime && stop <= endTime
        };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Programmes.All(program =>
            program.Start >= startTime && program.Stop <= endTime));
    }

    [Fact]
    public async Task ReadAllAsync_CustomDefaultLanguage_UsesSpecifiedLanguage()
    {
        // Arrange
        const string xmlFilePath = "./Data/epg.xml";
        const string customDefaultLanguage = "cn";

        var settings = new XmlTvReaderSettings
        {
            DefaultLanguage = customDefaultLanguage
        };

        // Act
        var result = await XmlTvReader.ReadAllAsync(xmlFilePath, settings);

        // Assert
        Assert.NotNull(result);

        var programme = result.Programmes.ElementAtOrDefault(1);
        Assert.NotNull(programme);
        Assert.Equal(customDefaultLanguage, programme.Titles.FirstOrDefault().Key);
        Assert.Equal(customDefaultLanguage, programme.Descriptions?.FirstOrDefault().Key);
    }

    private void AssertValidXmlTvChannel(XmlTvChannel channel)
    {
        Assert.Equal("channel1", channel.Id);
        Assert.Equal("Channel 1", channel.DisplayNames["en"]);
        Assert.NotNull(channel.Icons);
        Assert.NotEmpty(channel.Icons);
        Assert.Equal("channel1_icon.png", channel.Icons[0].Source);
        Assert.Equal(50, channel.Icons[0].Height);
        Assert.Equal(50, channel.Icons[0].Width);
        Assert.NotNull(channel.Urls);
        Assert.NotEmpty(channel.Urls);
        Assert.Equal("xmltv", channel.Urls[0].System);
        Assert.Equal("http://example.com/channel1", channel.Urls[0].Value);
    }

    private void AssertValidXmlTvProgramme(XmlTvProgramme programme)
    {
        Assert.Equal(
            DateTimeOffset.ParseExact("20220522120000", "yyyyMMddHHmmss", null, DateTimeStyles.AssumeUniversal),
            programme.Start);
        Assert.Equal(
            DateTimeOffset.ParseExact("20220522130000", "yyyyMMddHHmmss", null, DateTimeStyles.AssumeUniversal),
            programme.Stop);
        Assert.Equal("channel1", programme.ChannelId);
        Assert.Equal("Program 1", programme.Titles["en"]);
        Assert.Equal("Subtitle 1", programme.SubTitles?.GetValueOrDefault("en"));
        Assert.Equal("Description of Program 1", programme.Descriptions?.GetValueOrDefault("en"));
        Assert.Equal(DateTimeOffset.ParseExact("20220522", "yyyyMMdd", null, DateTimeStyles.AssumeUniversal),
            programme.Date);
        Assert.True(programme.IsPreviouslyShown);
        Assert.Equal("channel1", programme.PreviouslyShownChannel);
        Assert.Equal(
            DateTimeOffset.ParseExact("20220521120000", "yyyyMMddHHmmss", null, DateTimeStyles.AssumeUniversal),
            programme.PreviouslyShownDate);
        Assert.NotNull(programme.Actors);
        Assert.NotEmpty(programme.Actors);
        Assert.Equal("Actor 1", programme.Actors[0]);
        Assert.NotNull(programme.Credits);
        Assert.NotNull(programme.Credits.Directors);
        Assert.Equal("Director 1", programme.Credits.Directors[0]);

        Assert.NotNull(programme.Credits.Actors);
        Assert.NotEmpty(programme.Credits.Actors);
        Assert.Equal("Lead Actor 1", programme.Credits.Actors[0].Name);
        Assert.Equal("Lead", programme.Credits.Actors[0].Role);

        Assert.NotNull(programme.Credits.Writers);
        Assert.Equal("Writer 1", programme.Credits.Writers[0]);

        Assert.NotNull(programme.Credits.Adapters);
        Assert.Equal("Adapter 1", programme.Credits.Adapters[0]);

        Assert.NotNull(programme.Credits.Producers);
        Assert.Equal("Producer 1", programme.Credits.Producers[0]);

        Assert.NotNull(programme.Credits.Composers);
        Assert.Equal("Composer 1", programme.Credits.Composers[0]);

        Assert.NotNull(programme.Credits.Editors);
        Assert.Equal("Editor 1", programme.Credits.Editors[0]);

        Assert.NotNull(programme.Credits.Presenters);
        Assert.Equal("Presenter 1", programme.Credits.Presenters[0]);

        Assert.NotNull(programme.Credits.Commentators);
        Assert.Equal("Commentator 1", programme.Credits.Commentators[0]);

        Assert.NotNull(programme.Credits.Guests);
        Assert.NotEmpty(programme.Credits.Guests);
        Assert.Equal("Special Guest 1", programme.Credits.Guests[0].Name);
        Assert.Equal("Special", programme.Credits.Guests[0].Role);

        Assert.NotNull(programme.Ratings);
        Assert.True(programme.Ratings.ContainsKey("MPAA"));
        Assert.Equal("PG-13", programme.Ratings["MPAA"].Value);
        Assert.NotNull(programme.Ratings["MPAA"].Icons);
        Assert.NotEmpty(programme.Ratings["MPAA"].Icons);
        Assert.Equal("mpaa_icon.png", programme.Ratings["MPAA"].Icons[0].Source);

        Assert.NotNull(programme.StarRating);
        Assert.Equal("4.5", programme.StarRating);

        Assert.NotNull(programme.Episodes);
        Assert.NotEmpty(programme.Episodes);
        Assert.Equal("onscreen", programme.Episodes[0].System);
        Assert.Equal("S01E01", programme.Episodes[0].Value);

        Assert.Equal("English", programme.Language);

        Assert.NotNull(programme.Categories);
        Assert.NotEmpty(programme.Categories);
        Assert.Equal("Entertainment", programme.Categories[0]);

        Assert.NotNull(programme.Countries);
        Assert.NotEmpty(programme.Countries);
        Assert.Equal("USA", programme.Countries[0]);

        Assert.Equal("HDTV", programme.Quality);

        Assert.True(programme.IsNew);

        Assert.True(programme.IsPremiere);
        Assert.Equal("en", programme.PremiereLanguage);
        Assert.Equal("Premiere of Program 1", programme.Premiere);

        Assert.NotNull(programme.Icons);
        Assert.NotEmpty(programme.Icons);
        Assert.Equal("program1_icon.png", programme.Icons[0].Source);

        Assert.NotNull(programme.Urls);
        Assert.NotEmpty(programme.Urls);
        Assert.Equal("official", programme.Urls[0].System);
        Assert.Equal("http://example.com/program1", programme.Urls[0].Value);
    }

    private void AssertValidXmlTvResult(XmlTvResult result)
    {
        Assert.NotNull(result);
        Assert.NotNull(result.Channels);
        Assert.NotEmpty(result.Channels);
        Assert.NotNull(result.Programmes);
        Assert.NotEmpty(result.Programmes);

        var channel = result.Channels.First();
        AssertValidXmlTvChannel(channel);

        var programme = result.Programmes.First();
        AssertValidXmlTvProgramme(programme);
    }
}