namespace XmlTvSharp.Models;

/// <summary>
///     Specifies the subject or intended use of an XMLTV <c>image</c> element.
/// </summary>
public enum XmlTvImageType
{
    /// <summary>A poster image.</summary>
    Poster = 0,

    /// <summary>A backdrop image.</summary>
    Backdrop = 1,

    /// <summary>A still image from the programme.</summary>
    Still = 2,

    /// <summary>An image of a person.</summary>
    Person = 3,

    /// <summary>An image of a character.</summary>
    Character = 4
}
