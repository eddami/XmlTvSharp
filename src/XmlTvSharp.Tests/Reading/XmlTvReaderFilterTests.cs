namespace XmlTvSharp.Tests.Reading;

public sealed class XmlTvReaderFilterTests
{
    [Fact]
    public async Task ReadAsync_IncludesOnlyConfiguredTopLevelBranches()
    {
        const string xml = """
                           <tv>
                             <channel id="one"><display-name>One</display-name></channel>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>News</title>
                             </programme>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create()
            .IncludeChannels()
            .Build();

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), filter: filter, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Single(document.Channels);
        Assert.Empty(document.Programmes);
    }

    [Fact]
    public async Task ReadAsync_FiltersChannelsById()
    {
        const string xml = """
                           <tv>
                             <channel id="one"><display-name>One</display-name></channel>
                             <channel id="two"><display-name>Two</display-name></channel>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create()
            .IncludeChannels(channels => channels.WithIds("two"))
            .Build();

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), filter: filter, cancellationToken: TestContext.Current.CancellationToken);

        var channel = Assert.Single(document.Channels);
        Assert.Equal("two", channel.Id);
    }

    [Fact]
    public async Task ReadAsync_FiltersProgrammesByChannelId()
    {
        const string xml = """
                           <tv>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>One</title>
                             </programme>
                             <programme start="20260605130000 +0000" channel="two">
                               <title>Two</title>
                             </programme>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create()
            .IncludeProgrammes(programmes => programmes.ForChannels("two"))
            .Build();

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), filter: filter, cancellationToken: TestContext.Current.CancellationToken);

        var programme = Assert.Single(document.Programmes);
        Assert.Equal("two", programme.ChannelId);
        Assert.Equal("Two", Assert.Single(programme.Titles).Value);
    }

    [Fact]
    public async Task ReadAsync_SkipsRejectedProgrammeSubtreeWithoutParsingNestedContent()
    {
        const string xml = """
                           <tv>
                             <programme start="not-a-date" channel="skip">
                               <unknown />
                             </programme>
                             <programme start="20260605130000 +0000" channel="keep">
                               <title>Keep</title>
                             </programme>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create()
            .IncludeProgrammes(programmes => programmes.ForChannels("keep"))
            .Build();

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), filter: filter, cancellationToken: TestContext.Current.CancellationToken);

        var programme = Assert.Single(document.Programmes);
        Assert.Equal("keep", programme.ChannelId);
    }

    [Fact]
    public async Task ReadAsync_SkipsRejectedChannelSubtreeWithoutParsingNestedContent()
    {
        const string xml = """
                           <tv>
                             <channel id="skip">
                               <unknown />
                             </channel>
                             <programme start="20260605130000 +0000" channel="keep">
                               <title>Keep</title>
                             </programme>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create()
            .IncludeProgrammes()
            .Build();

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), filter: filter, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Empty(document.Channels);
        Assert.Single(document.Programmes);
    }

    [Fact]
    public async Task ReadElementAsync_ReturnsOnlyFilteredElements()
    {
        const string xml = """
                           <tv>
                             <channel id="one"><display-name>One</display-name></channel>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>One</title>
                             </programme>
                             <programme start="20260605130000 +0000" channel="two">
                               <title>Two</title>
                             </programme>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create()
            .IncludeProgrammes(programmes => programmes.ForChannels("two"))
            .Build();
        using var reader = new XmlTvReader(new StringReader(xml), filter: filter);

        var metadata = await reader.ReadMetadataAsync(TestContext.Current.CancellationToken);
        var element = Assert.IsType<XmlTvProgramme>(await reader.ReadElementAsync(TestContext.Current.CancellationToken));
        var end = await reader.ReadElementAsync(TestContext.Current.CancellationToken);

        Assert.Null(metadata.SourceInfoName);
        Assert.Equal("two", element.ChannelId);
        Assert.Null(end);
    }

    [Fact]
    public async Task ReadElementAsync_ReturnsNoElementsForEmptyExplicitSelection()
    {
        const string xml = """
                           <tv source-info-name="Source">
                             <channel id="one"><display-name>One</display-name></channel>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>One</title>
                             </programme>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create().Build();
        using var reader = new XmlTvReader(new StringReader(xml), filter: filter);

        var metadata = await reader.ReadMetadataAsync(TestContext.Current.CancellationToken);
        var end = await reader.ReadElementAsync(TestContext.Current.CancellationToken);

        Assert.Equal("Source", metadata.SourceInfoName);
        Assert.Null(end);
    }

    [Fact]
    public async Task ReadAsync_ReturnsMetadataOnlyForEmptyExplicitSelection()
    {
        const string xml = """
                           <tv source-info-name="Source">
                             <channel id="one"><display-name>One</display-name></channel>
                             <programme start="20260605120000 +0000" channel="one">
                               <title>One</title>
                             </programme>
                           </tv>
                           """;

        var filter = XmlTvReadFilter.Create().Build();

        var document = await XmlTvReader.ReadAsync(new StringReader(xml), filter: filter, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("Source", document.Metadata.SourceInfoName);
        Assert.Empty(document.Channels);
        Assert.Empty(document.Programmes);
    }

    [Fact]
    public void Build_RejectsDuplicateBranchConfiguration()
    {
        var builder = XmlTvReadFilter.Create()
            .IncludeChannels();

        Assert.Throws<InvalidOperationException>(() => builder.IncludeChannels());
    }

    [Fact]
    public void ForChannels_RejectsDuplicateConfiguration()
    {
        Assert.Throws<InvalidOperationException>(() => XmlTvReadFilter.Create()
            .IncludeProgrammes(programmes =>
            {
                programmes.ForChannels("one");
                programmes.ForChannels("two");
            }));
    }

    [Fact]
    public void WithIds_RejectsEmptyIds()
    {
        Assert.Throws<ArgumentException>(() => XmlTvReadFilter.Create()
            .IncludeChannels(channels => channels.WithIds()));
    }
}
