namespace XmlTvSharp.Tests.Reading;

public sealed class XmlTvReaderDuplicateSingletonTests
{
    [Fact]
    public async Task ReadAsync_AllowsOutOfOrderProgrammeSingletons()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <date>20260605</date>
                               <title>News</title>
                               <language>English</language>
                             </programme>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml));

        var programme = Assert.Single(document.Programmes);
        Assert.Equal("20260605", programme.Date!.ToXmlTvString());
        Assert.Equal(new XmlTvLocalizedText("English"), programme.Language);
    }

    [Fact]
    public async Task ReadAsync_DisallowsDuplicateProgrammeSingleton()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <date>20260605</date>
                               <date>20260606</date>
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_DisallowsDuplicateNewMarker()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <new />
                               <new />
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_DisallowsDuplicateJellyfinLiveMarker()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <live />
                               <live />
                             </programme>
                           </tv>
                           """;
        var options = new XmlTvReaderOptions
        {
            CompatibilityProfile = XmlTvCompatibilityProfile.Jellyfin
        };

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml), options));
    }

    [Fact]
    public async Task ReadAsync_DisallowsDuplicateVideoSingleton()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <video>
                                 <present>yes</present>
                                 <present>no</present>
                               </video>
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_DisallowsDuplicateAudioSingleton()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <audio>
                                 <stereo>stereo</stereo>
                                 <stereo>mono</stereo>
                               </audio>
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_DisallowsDuplicateSubtitlesLanguage()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <subtitles>
                                 <language>English</language>
                                 <language>French</language>
                               </subtitles>
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }

    [Fact]
    public async Task ReadAsync_DisallowsDuplicateRatingValue()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="channel-one">
                               <title>News</title>
                               <rating>
                                 <value>PG</value>
                                 <value>TV-PG</value>
                               </rating>
                             </programme>
                           </tv>
                           """;

        await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml)));
    }
}
