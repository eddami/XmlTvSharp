namespace XmlTvSharp.Models;

/// <summary>
///     Specifies the content type of an XMLTV <c>review</c> element.
/// </summary>
public enum XmlTvReviewType
{
    /// <summary>The review contains text.</summary>
    Text = 0,

    /// <summary>The review contains a URL.</summary>
    Url = 1
}
