namespace XmlTvSharp;

/// <summary>
///     Represents an error encountered while writing XMLTV content.
/// </summary>
/// <remarks>
///     Writer validation errors identify the XMLTV element being written when possible. Underlying XML writer or I/O
///     failures are preserved as <see cref="Exception.InnerException" />.
/// </remarks>
public sealed class XmlTvWriteException : Exception
{
    /// <summary>Initializes a write exception.</summary>
    /// <param name="message">The error message.</param>
    public XmlTvWriteException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a write exception with an inner exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public XmlTvWriteException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
