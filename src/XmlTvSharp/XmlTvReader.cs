using System.Xml;
using XmlTvSharp.Models;
using XmlTvSharp.Parsing;

namespace XmlTvSharp;

/// <summary>
///     Reads complete XMLTV documents and supported top-level XMLTV elements from XML streams.
/// </summary>
/// <remarks>
///     The reader is forward-only and not thread-safe. Use <see cref="ReadAsync(string, CancellationToken)" /> for
///     complete-document reads, or call <see cref="ReadMetadataAsync" /> and <see cref="ReadElementAsync" /> for
///     streaming reads.
/// </remarks>
public sealed class XmlTvReader : IDisposable
{
    private readonly XmlTvParser _parser;
    private readonly XmlReader _reader;
    private bool _disposed;

    /// <summary>Initializes a reader for a file path.</summary>
    /// <param name="path">The XMLTV file path.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="filter">Optional read filter.</param>
    /// <remarks>The file is owned by this reader.</remarks>
    public XmlTvReader(string path, XmlTvReaderOptions? options = null, XmlTvReadFilter? filter = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be empty or whitespace.", nameof(path));
        }

        var readerOptions = ValidateOptions(options);
        _reader = XmlTvReaderFactory.Create(path);
        _parser = new XmlTvParser(_reader, readerOptions, filter);
    }

    /// <summary>Initializes a reader for a stream.</summary>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="filter">Optional read filter.</param>
    /// <param name="leaveOpen">Whether to leave <paramref name="stream" /> open when this reader is disposed.</param>
    public XmlTvReader(
        Stream stream,
        XmlTvReaderOptions? options = null,
        XmlTvReadFilter? filter = null,
        bool leaveOpen = false)
    {
        var readerOptions = ValidateOptions(options);
        _reader = XmlTvReaderFactory.Create(stream ?? throw new ArgumentNullException(nameof(stream)), leaveOpen);
        _parser = new XmlTvParser(_reader, readerOptions, filter);
    }

    /// <summary>Initializes a reader for a text reader.</summary>
    /// <param name="textReader">The XMLTV text reader.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="filter">Optional read filter.</param>
    /// <param name="leaveOpen">Whether to leave <paramref name="textReader" /> open when this reader is disposed.</param>
    public XmlTvReader(
        TextReader textReader,
        XmlTvReaderOptions? options = null,
        XmlTvReadFilter? filter = null,
        bool leaveOpen = false)
    {
        var readerOptions = ValidateOptions(options);
        _reader = XmlTvReaderFactory.Create(textReader ?? throw new ArgumentNullException(nameof(textReader)),
            leaveOpen);
        _parser = new XmlTvParser(_reader, readerOptions, filter);
    }

    /// <summary>Releases the underlying XML reader.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _reader.Dispose();
        _disposed = true;
    }

    /// <summary>Reads a complete XMLTV document from a file path.</summary>
    /// <param name="path">The XMLTV file path.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>A <see langword="null" /> filter reads all supported top-level child elements.</remarks>
    public static Task<XmlTvDocument> ReadAsync(
        string path,
        CancellationToken cancellationToken)
    {
        return ReadAsync(path, null, null, cancellationToken);
    }

    /// <summary>Reads a complete XMLTV document from a file path.</summary>
    /// <param name="path">The XMLTV file path.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>A <see langword="null" /> filter reads all supported top-level child elements.</remarks>
    public static Task<XmlTvDocument> ReadAsync(
        string path,
        XmlTvReaderOptions? options,
        CancellationToken cancellationToken)
    {
        return ReadAsync(path, options, null, cancellationToken);
    }

    /// <summary>Reads a complete XMLTV document from a file path.</summary>
    /// <param name="path">The XMLTV file path.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="filter">Optional read filter.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>A <see langword="null" /> filter reads all supported top-level child elements.</remarks>
    public static async Task<XmlTvDocument> ReadAsync(
        string path,
        XmlTvReaderOptions? options = null,
        XmlTvReadFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var reader = new XmlTvReader(path, options, filter);
        return await reader.ReadDocumentAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Reads a complete XMLTV document from a stream.</summary>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>The stream remains open after the read completes.</remarks>
    public static Task<XmlTvDocument> ReadAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        return ReadAsync(stream, null, null, cancellationToken);
    }

    /// <summary>Reads a complete XMLTV document from a stream.</summary>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>The stream remains open after the read completes.</remarks>
    public static Task<XmlTvDocument> ReadAsync(
        Stream stream,
        XmlTvReaderOptions? options,
        CancellationToken cancellationToken)
    {
        return ReadAsync(stream, options, null, cancellationToken);
    }

    /// <summary>Reads a complete XMLTV document from a stream.</summary>
    /// <param name="stream">The XMLTV stream.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="filter">Optional read filter.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>The stream remains open after the read completes.</remarks>
    public static async Task<XmlTvDocument> ReadAsync(
        Stream stream,
        XmlTvReaderOptions? options = null,
        XmlTvReadFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var reader = new XmlTvReader(stream, options, filter, true);
        return await reader.ReadDocumentAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Reads a complete XMLTV document from a text reader.</summary>
    /// <param name="textReader">The XMLTV text reader.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>The text reader remains open after the read completes.</remarks>
    public static Task<XmlTvDocument> ReadAsync(
        TextReader textReader,
        CancellationToken cancellationToken)
    {
        return ReadAsync(textReader, null, null, cancellationToken);
    }

    /// <summary>Reads a complete XMLTV document from a text reader.</summary>
    /// <param name="textReader">The XMLTV text reader.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>The text reader remains open after the read completes.</remarks>
    public static Task<XmlTvDocument> ReadAsync(
        TextReader textReader,
        XmlTvReaderOptions? options,
        CancellationToken cancellationToken)
    {
        return ReadAsync(textReader, options, null, cancellationToken);
    }

    /// <summary>Reads a complete XMLTV document from a text reader.</summary>
    /// <param name="textReader">The XMLTV text reader.</param>
    /// <param name="options">Optional reader options.</param>
    /// <param name="filter">Optional read filter.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV document.</returns>
    /// <remarks>The text reader remains open after the read completes.</remarks>
    public static async Task<XmlTvDocument> ReadAsync(
        TextReader textReader,
        XmlTvReaderOptions? options = null,
        XmlTvReadFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var reader = new XmlTvReader(textReader, options, filter, true);
        return await reader.ReadDocumentAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Reads metadata from the XMLTV <c>tv</c> root element.</summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The parsed XMLTV root metadata.</returns>
    /// <remarks>
    ///     The metadata is read once and cached. Calling this method more than once returns the same metadata.
    ///     <see cref="ReadElementAsync" /> reads the metadata first when needed, but it does not return metadata as an
    ///     element.
    /// </remarks>
    public Task<XmlTvMetadata> ReadMetadataAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _parser.ReadMetadataAsync(cancellationToken);
    }

    /// <summary>Reads the next supported top-level XMLTV child element.</summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The next channel or programme, or <see langword="null" /> at the end of the document.</returns>
    /// <remarks>
    ///     This method returns only <see cref="XmlTvChannel" /> and <see cref="XmlTvProgramme" /> elements accepted by
    ///     the reader filter. Use <see cref="ReadMetadataAsync" /> for metadata from the XMLTV <c>tv</c> root element.
    /// </remarks>
    public Task<IXmlTvElement?> ReadElementAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _parser.ReadElementAsync(cancellationToken);
    }

    private async Task<XmlTvDocument> ReadDocumentAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return await _parser.ReadDocumentAsync(cancellationToken).ConfigureAwait(false);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(XmlTvReader));
        }
    }

    private static XmlTvReaderOptions ValidateOptions(XmlTvReaderOptions? options)
    {
        var readerOptions = options ?? new XmlTvReaderOptions();

        if (!Enum.IsDefined(typeof(XmlTvCompatibilityProfile), readerOptions.CompatibilityProfile))
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                readerOptions.CompatibilityProfile,
                "Unsupported XMLTV compatibility profile.");
        }

        if (!Enum.IsDefined(typeof(XmlTvUnknownContentHandling), readerOptions.UnknownElementHandling))
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                readerOptions.UnknownElementHandling,
                "Unsupported unknown element handling behavior.");
        }

        if (!Enum.IsDefined(typeof(XmlTvUnknownContentHandling), readerOptions.UnknownAttributeHandling))
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                readerOptions.UnknownAttributeHandling,
                "Unsupported unknown attribute handling behavior.");
        }

        if (!Enum.IsDefined(typeof(XmlTvUnknownContentHandling), readerOptions.XExtensionHandling))
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                readerOptions.XExtensionHandling,
                "Unsupported x-extension handling behavior.");
        }

        return readerOptions;
    }
}
