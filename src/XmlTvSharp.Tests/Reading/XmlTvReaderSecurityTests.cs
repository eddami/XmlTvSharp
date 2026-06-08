using System.Xml;

namespace XmlTvSharp.Tests.Reading;

public sealed class XmlTvReaderSecurityTests
{
    [Fact]
    public async Task ReadAsync_IgnoresExternalDtd()
    {
        const string xml = """
                           <!DOCTYPE tv SYSTEM "file:///definitely-not-existing-xmltv.dtd">
                           <tv>
                             <channel id="one"><display-name>One</display-name></channel>
                           </tv>
                           """;

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), TestContext.Current.CancellationToken);

        Assert.Equal("one", Assert.Single(document.Channels).Id);
    }

    [Fact]
    public async Task ReadAsync_DoesNotResolveExternalEntity()
    {
        const string xml = """
                           <!DOCTYPE tv [
                             <!ENTITY externalTitle SYSTEM "file:///definitely-not-existing-xmltv-title.txt">
                           ]>
                           <tv>
                             <channel id="one"><display-name>&externalTitle;</display-name></channel>
                           </tv>
                           """;

        var exception =
            await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml), TestContext.Current.CancellationToken));

        Assert.IsType<XmlException>(exception.InnerException);
    }

    [Fact]
    public async Task ReadAsync_DoesNotResolveInternalEntity()
    {
        const string xml = """
                           <!DOCTYPE tv [
                             <!ENTITY injectedTitle "Injected">
                           ]>
                           <tv>
                             <channel id="one"><display-name>&injectedTitle;</display-name></channel>
                           </tv>
                           """;

        var exception =
            await Assert.ThrowsAsync<XmlTvReadException>(() => XmlTvReader.ReadAsync(new StringReader(xml), TestContext.Current.CancellationToken));

        Assert.IsType<XmlException>(exception.InnerException);
    }
}
