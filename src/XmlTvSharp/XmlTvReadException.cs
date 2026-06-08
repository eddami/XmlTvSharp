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

    /// <summary>Initializes a read exception with an inner exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public XmlTvReadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
