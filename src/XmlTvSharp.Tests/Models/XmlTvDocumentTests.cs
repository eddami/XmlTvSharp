namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvDocumentTests
{
    [Fact]
    public void Document_CollectionsAreInitializedAndOrdered()
    {
        var document = new XmlTvDocument();
        var first = new XmlTvChannel("one", "One");
        var second = new XmlTvChannel("two", "Two");

        Assert.NotNull(document.Metadata);

        document.Channels.Add(first);
        document.Channels.Add(second);

        Assert.Equal(new[] { first, second }, document.Channels);
        Assert.Empty(document.Programmes);
    }

    [Fact]
    public void Constructor_PreservesMetadataInstance()
    {
        var metadata = new XmlTvMetadata
        {
            SourceInfoName = "Source"
        };

        var document = new XmlTvDocument(metadata);

        Assert.Same(metadata, document.Metadata);
        Assert.Equal("Source", document.Metadata.SourceInfoName);
    }

    [Fact]
    public void Constructor_NullMetadata_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new XmlTvDocument(null!));
    }
}
