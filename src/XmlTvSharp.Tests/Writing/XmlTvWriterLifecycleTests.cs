using System.Text;

namespace XmlTvSharp.Tests.Writing;

public sealed class XmlTvWriterLifecycleTests
{
    [Fact]
    public async Task WriteChannelAsync_BeforeStart_ThrowsInvalidOperationException()
    {
        using var writer = CreateWriter();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            writer.WriteChannelAsync(new XmlTvChannel("channel", "Channel"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WriteProgrammeAsync_BeforeStart_ThrowsInvalidOperationException()
    {
        using var writer = CreateWriter();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            writer.WriteProgrammeAsync(new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel",
                "Title"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CompleteAsync_BeforeStart_ThrowsInvalidOperationException()
    {
        using var writer = CreateWriter();

        await Assert.ThrowsAsync<InvalidOperationException>(() => writer.CompleteAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task StartAsync_SecondCall_ThrowsInvalidOperationException()
    {
        using var writer = CreateWriter();

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() => writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CompleteAsync_SecondCall_ThrowsInvalidOperationException()
    {
        using var writer = CreateWriter();

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() => writer.CompleteAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WriteChannelAsync_AfterProgramme_ThrowsInvalidOperationException()
    {
        using var writer = CreateWriter();

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.WriteProgrammeAsync(new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel",
            "Title"), TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            writer.WriteChannelAsync(new XmlTvChannel("channel", "Channel"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WriteProgrammeAsync_AfterComplete_ThrowsInvalidOperationException()
    {
        using var writer = CreateWriter();

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await writer.CompleteAsync(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            writer.WriteProgrammeAsync(new XmlTvProgramme(XmlTvDateTime.Parse("20260605120000 +0000"), "channel",
                "Title"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task StartAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        using var writer = CreateWriter();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            writer.StartAsync(cancellationToken: cancellationTokenSource.Token));
    }

    [Fact]
    public async Task WriteAsync_WithCancelledToken_ThrowsBeforeCreatingFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xml");
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        try
        {
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                XmlTvWriter.WriteAsync(new XmlTvDocument(), path, cancellationTokenSource.Token));

            Assert.False(File.Exists(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Constructor_InvalidCompatibilityProfile_ThrowsArgumentOutOfRangeException()
    {
        var options = new XmlTvWriterOptions
        {
            CompatibilityProfile = (XmlTvCompatibilityProfile)999
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new XmlTvWriter(new StringWriter(new StringBuilder()), options));
    }

    [Fact]
    public async Task WriteAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        var writer = CreateWriter();
        writer.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CompleteAsync_RawWriterFailure_ThrowsXmlTvWriteExceptionWithInnerException()
    {
        using var writer = new XmlTvWriter(new ThrowingFlushTextWriter());

        await writer.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        var exception = await Assert.ThrowsAsync<XmlTvWriteException>(() => writer.CompleteAsync(TestContext.Current.CancellationToken));
        Assert.Contains("complete", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.IsType<IOException>(exception.InnerException);
    }

    private static XmlTvWriter CreateWriter()
    {
        return new XmlTvWriter(new StringWriter(new StringBuilder()));
    }

    private sealed class ThrowingFlushTextWriter : StringWriter
    {
        public override Task FlushAsync()
        {
            return Task.FromException(new IOException("Flush failed."));
        }
    }
}
