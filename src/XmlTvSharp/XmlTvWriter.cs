using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Writing;

namespace XmlTvSharp;

/// <summary>
///     Writes XMLTV documents and top-level XMLTV elements to XML streams.
/// </summary>
/// <remarks>
///     The writer is forward-only and not thread-safe. Call <see cref="StartAsync" /> once, write all channels before
///     programmes, then call <see cref="CompleteAsync" /> once. Static <c>WriteAsync</c> methods use the same lifecycle
///     internally.
/// </remarks>
public sealed class XmlTvWriter : IDisposable
{
    private readonly HashSet<string> _channelIds = new(StringComparer.Ordinal);
    private readonly XmlTvElementWriter _elementWriter;
    private readonly XmlTvWriterOptions _options;
    private readonly XmlWriter _writer;
    private bool _completed;
    private bool _disposed;
    private bool _started;
    private bool _wroteProgramme;

    /// <summary>Initializes a writer for a file path.</summary>
    /// <param name="path">The XMLTV file path.</param>
    /// <param name="options">Optional writer options.</param>
    /// <remarks>The file is owned by this writer. Stream output uses UTF-8 without a byte-order mark.</remarks>
    public XmlTvWriter(string path, XmlTvWriterOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be empty or whitespace.", nameof(path));
        }

        _options = ValidateOptions(options);
        _writer = XmlTvWriterFactory.Create(path, _options);
        _elementWriter = new XmlTvElementWriter(_writer, _options);
    }

    /// <summary>Initializes a writer for a stream.</summary>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="leaveOpen">Whether to leave <paramref name="stream" /> open when this writer is disposed.</param>
    /// <remarks>Stream output uses UTF-8 without a byte-order mark.</remarks>
    public XmlTvWriter(
        Stream stream,
        XmlTvWriterOptions? options = null,
        bool leaveOpen = false)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        _options = ValidateOptions(options);
        _writer = XmlTvWriterFactory.Create(stream, _options, leaveOpen);
        _elementWriter = new XmlTvElementWriter(_writer, _options);
    }

    /// <summary>Initializes a writer for a text writer.</summary>
    /// <param name="textWriter">The XMLTV text writer.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="leaveOpen">Whether to leave <paramref name="textWriter" /> open when this writer is disposed.</param>
    /// <remarks>The supplied <paramref name="textWriter" /> controls the output encoding.</remarks>
    public XmlTvWriter(
        TextWriter textWriter,
        XmlTvWriterOptions? options = null,
        bool leaveOpen = false)
    {
        if (textWriter is null)
        {
            throw new ArgumentNullException(nameof(textWriter));
        }

        _options = ValidateOptions(options);
        _writer = XmlTvWriterFactory.Create(textWriter, _options, leaveOpen);
        _elementWriter = new XmlTvElementWriter(_writer, _options);
    }

    /// <summary>Releases the underlying XML writer.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Dispose();
        _disposed = true;
    }

    /// <summary>Writes a complete XMLTV document to a file path.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="path">The XMLTV file path.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>Channels are emitted before programmes, regardless of collection insertion order.</remarks>
    public static Task WriteAsync(
        XmlTvDocument document,
        string path,
        CancellationToken cancellationToken)
    {
        return WriteAsync(document, path, null, cancellationToken);
    }

    /// <summary>Writes a complete XMLTV document to a file path.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="path">The XMLTV file path.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>Channels are emitted before programmes, regardless of collection insertion order.</remarks>
    public static async Task WriteAsync(
        XmlTvDocument document,
        string path,
        XmlTvWriterOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var writer = new XmlTvWriter(path, options);
        await writer.WriteDocumentAsync(document, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Writes a complete XMLTV document to a stream.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>The stream remains open after the write completes.</remarks>
    public static Task WriteAsync(
        XmlTvDocument document,
        Stream stream,
        CancellationToken cancellationToken)
    {
        return WriteAsync(document, stream, null, true, cancellationToken);
    }

    /// <summary>Writes a complete XMLTV document to a stream.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>The stream remains open after the write completes.</remarks>
    public static Task WriteAsync(
        XmlTvDocument document,
        Stream stream,
        XmlTvWriterOptions? options,
        CancellationToken cancellationToken)
    {
        return WriteAsync(document, stream, options, true, cancellationToken);
    }

    /// <summary>Writes a complete XMLTV document to a stream.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="leaveOpen">Whether to leave <paramref name="stream" /> open after the write completes.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>Stream output uses UTF-8 without a byte-order mark.</remarks>
    public static async Task WriteAsync(
        XmlTvDocument document,
        Stream stream,
        XmlTvWriterOptions? options = null,
        bool leaveOpen = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var writer = new XmlTvWriter(stream, options, leaveOpen);
        await writer.WriteDocumentAsync(document, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Writes a complete XMLTV document to a text writer.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="textWriter">The XMLTV text writer.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>The text writer remains open after the write completes.</remarks>
    public static Task WriteAsync(
        XmlTvDocument document,
        TextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return WriteAsync(document, textWriter, null, true, cancellationToken);
    }

    /// <summary>Writes a complete XMLTV document to a text writer.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="textWriter">The XMLTV text writer.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>The text writer remains open after the write completes.</remarks>
    public static Task WriteAsync(
        XmlTvDocument document,
        TextWriter textWriter,
        XmlTvWriterOptions? options,
        CancellationToken cancellationToken)
    {
        return WriteAsync(document, textWriter, options, true, cancellationToken);
    }

    /// <summary>Writes a complete XMLTV document to a text writer.</summary>
    /// <param name="document">The XMLTV document.</param>
    /// <param name="textWriter">The XMLTV text writer.</param>
    /// <param name="options">Optional writer options.</param>
    /// <param name="leaveOpen">Whether to leave <paramref name="textWriter" /> open after the write completes.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>The supplied <paramref name="textWriter" /> controls the output encoding.</remarks>
    public static async Task WriteAsync(
        XmlTvDocument document,
        TextWriter textWriter,
        XmlTvWriterOptions? options = null,
        bool leaveOpen = true,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var writer = new XmlTvWriter(textWriter, options, leaveOpen);
        await writer.WriteDocumentAsync(document, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Starts an XMLTV document.</summary>
    /// <param name="metadata">Optional XMLTV root metadata.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>
    ///     This method writes the XML declaration and opening <c>tv</c> element. Passing <see langword="null" /> writes
    ///     an empty metadata set. It must be called exactly once before writing channels or programmes.
    /// </remarks>
    public async Task StartAsync(
        XmlTvMetadata? metadata = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (_started)
        {
            throw new InvalidOperationException("The XMLTV document has already been started.");
        }

        if (_completed)
        {
            throw new InvalidOperationException("The XMLTV document has already been completed.");
        }

        metadata ??= new XmlTvMetadata();
        XmlTvWriteValidator.ValidateMetadata(metadata);
        try
        {
            await _elementWriter.WriteStartAsync(metadata).ConfigureAwait(false);
        }
        catch (Exception exception) when (IsRawWriteFailure(exception))
        {
            throw new XmlTvWriteException("Failed to start the XMLTV document.", exception);
        }

        _started = true;
    }

    /// <summary>Writes a top-level XMLTV channel element.</summary>
    /// <param name="channel">The channel to write.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>
    ///     Channels must be written after <see cref="StartAsync" /> and before the first programme. Channel IDs must be
    ///     unique within the current writer.
    /// </remarks>
    public async Task WriteChannelAsync(
        XmlTvChannel channel,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        EnsureStarted();

        if (_wroteProgramme)
        {
            throw new InvalidOperationException("Channels must be written before programmes.");
        }

        try
        {
            XmlTvWriteValidator.ValidateChannel(channel);
        }
        catch (XmlTvWriteException exception)
        {
            throw new XmlTvWriteException($"Cannot write channel '{channel.Id}': {exception.Message}", exception);
        }

        if (!_channelIds.Add(channel.Id))
        {
            throw new XmlTvWriteException($"Duplicate channel id '{channel.Id}'.");
        }

        try
        {
            await _elementWriter.WriteChannelAsync(channel).ConfigureAwait(false);
        }
        catch (Exception exception) when (IsRawWriteFailure(exception))
        {
            throw new XmlTvWriteException($"Failed to write channel '{channel.Id}'.", exception);
        }
    }

    /// <summary>Writes a top-level XMLTV programme element.</summary>
    /// <param name="programme">The programme to write.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>
    ///     Programmes may be written one at a time after <see cref="StartAsync" />. The writer does not retain
    ///     programmes or validate that <see cref="XmlTvProgramme.ChannelId" /> refers to a written channel.
    /// </remarks>
    public async Task WriteProgrammeAsync(
        XmlTvProgramme programme,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        EnsureStarted();

        try
        {
            XmlTvWriteValidator.ValidateProgramme(programme);
        }
        catch (XmlTvWriteException exception)
        {
            throw new XmlTvWriteException(
                $"Cannot write programme '{programme.Start?.ToXmlTvString()}' on channel '{programme.ChannelId}': {exception.Message}",
                exception);
        }

        try
        {
            await _elementWriter.WriteProgrammeAsync(programme).ConfigureAwait(false);
        }
        catch (Exception exception) when (IsRawWriteFailure(exception))
        {
            throw new XmlTvWriteException(
                $"Failed to write programme '{programme.Start.ToXmlTvString()}' on channel '{programme.ChannelId}'.",
                exception);
        }

        _wroteProgramme = true;
    }

    /// <summary>Completes the XMLTV document.</summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <remarks>This method closes the root <c>tv</c> element and flushes the underlying XML writer.</remarks>
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        EnsureStarted();

        try
        {
            await _elementWriter.WriteCompleteAsync().ConfigureAwait(false);
            await _writer.FlushAsync().ConfigureAwait(false);
        }
        catch (Exception exception) when (IsRawWriteFailure(exception))
        {
            throw new XmlTvWriteException("Failed to complete the XMLTV document.", exception);
        }

        _completed = true;
    }

    /// <summary>Flushes XML written so far without completing the XMLTV document.</summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous flush operation.</returns>
    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await _writer.FlushAsync().ConfigureAwait(false);
        }
        catch (Exception exception) when (IsRawWriteFailure(exception))
        {
            throw new XmlTvWriteException("Failed to flush the XMLTV writer.", exception);
        }
    }

    private async Task WriteDocumentAsync(XmlTvDocument document, CancellationToken cancellationToken)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        await StartAsync(document.Metadata, cancellationToken).ConfigureAwait(false);

        foreach (var channel in document.Channels)
        {
            await WriteChannelAsync(channel, cancellationToken).ConfigureAwait(false);
        }

        foreach (var programme in document.Programmes)
        {
            await WriteProgrammeAsync(programme, cancellationToken).ConfigureAwait(false);
        }

        await CompleteAsync(cancellationToken).ConfigureAwait(false);
    }

    private void EnsureStarted()
    {
        if (!_started)
        {
            throw new InvalidOperationException("The XMLTV document has not been started.");
        }

        if (_completed)
        {
            throw new InvalidOperationException("The XMLTV document has already been completed.");
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(XmlTvWriter));
        }
    }

    private static XmlTvWriterOptions ValidateOptions(XmlTvWriterOptions? options)
    {
        var writerOptions = options ?? XmlTvWriterOptions.Default;

        if (!Enum.IsDefined(typeof(XmlTvCompatibilityProfile), writerOptions.CompatibilityProfile))
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                writerOptions.CompatibilityProfile,
                "Unsupported XMLTV compatibility profile.");
        }

        return writerOptions;
    }

    private static bool IsRawWriteFailure(Exception exception)
    {
        return exception is IOException or InvalidOperationException or ObjectDisposedException or XmlException;
    }
}
