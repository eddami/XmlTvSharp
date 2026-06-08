namespace XmlTvSharp;

/// <summary>
///     Represents an error encountered while reading XMLTV content.
/// </summary>
/// <remarks>Read exceptions include XML line information when the underlying reader provides it.</remarks>
public sealed class XmlTvReadException : Exception
{
    /// <summary>Initializes a read exception.</summary>
    /// <param name="message">The error message.</param>
    public XmlTvReadException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a read exception with XML reader position information.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="lineNumber">The XML line number, or zero when unavailable.</param>
    /// <param name="linePosition">The XML line position, or zero when unavailable.</param>
    public XmlTvReadException(string message, int lineNumber, int linePosition)
        : base(message)
    {
        LineNumber = lineNumber;
        LinePosition = linePosition;
    }

    /// <summary>Initializes a read exception with an inner exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public XmlTvReadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Initializes a read exception with XML reader position information and an inner exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="lineNumber">The XML line number, or zero when unavailable.</param>
    /// <param name="linePosition">The XML line position, or zero when unavailable.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public XmlTvReadException(string message, int lineNumber, int linePosition, Exception innerException)
        : base(message, innerException)
    {
        LineNumber = lineNumber;
        LinePosition = linePosition;
    }

    /// <summary>Gets the XML reader line number where the error was detected, or zero when unavailable.</summary>
    public int LineNumber { get; }

    /// <summary>Gets the XML reader line position where the error was detected, or zero when unavailable.</summary>
    public int LinePosition { get; }
}
