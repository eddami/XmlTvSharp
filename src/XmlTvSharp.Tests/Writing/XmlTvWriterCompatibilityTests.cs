using System.Xml.Linq;

namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterCompatibilityTests
{
    [Fact]
    public async Task WriteProgrammeAsync_StandardProfile_OmitsJellyfinLiveMarker()
    {
        var output = new StringWriter();
        var programme = CreateLiveProgramme();
        using var writer = new XmlTvWriter(output);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(programme, TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        var programmeElement = Assert.Single(XDocument.Parse(output.ToString()).Root!.Elements("programme"));
        Assert.Empty(programmeElement.Elements("live"));
    }

    [Fact]
    public async Task WriteProgrammeAsync_JellyfinProfile_WritesLiveMarker()
    {
        var output = new StringWriter();
        var options = new XmlTvWriterOptions
        {
            CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
        };
        using var writer = new XmlTvWriter(output, options);

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(CreateLiveProgramme(), TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        var programmeElement = Assert.Single(XDocument.Parse(output.ToString()).Root!.Elements("programme"));
        var live = Assert.Single(programmeElement.Elements("live"));
        Assert.False(live.HasAttributes);
        Assert.Empty(live.Elements());
        Assert.Empty(live.Value);
    }

    [Fact]
    public async Task WriteAsync_JellyfinProfile_RoundTripsLiveMarker()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <live />
                             </programme>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
        };
        var writerOptions = new XmlTvWriterOptions
        {
            CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
        };
        var document = await XmlTvReader.ReadAsync(new StringReader(xml), options, TestContext.Current.CancellationToken);
        var output = new StringWriter();

        await XmlTvWriter.WriteAsync(document, output, writerOptions, TestContext.Current.CancellationToken);
        var roundTripped = await XmlTvReader.ReadAsync(new StringReader(output.ToString()), options, TestContext.Current.CancellationToken);

        var programme = Assert.Single(roundTripped.Programmes);
        Assert.True(programme.Extensions?.Jellyfin?.IsLive);
    }

    private static XmlTvProgramme CreateLiveProgramme()
    {
        var programme = new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel-one", "News");
        programme.Extensions = new XmlTvProgrammeExtensions
        {
            Jellyfin = new XmlTvJellyfinProgrammeExtensions
            {
                IsLive = true
            }
        };

        return programme;
    }
}
