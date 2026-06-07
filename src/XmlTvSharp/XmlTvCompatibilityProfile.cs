namespace XmlTvSharp;

/// <summary>
///     Specifies the XMLTV compatibility profile used while reading XMLTV content.
/// </summary>
public enum XmlTvCompatibilityProfile
{
    /// <summary>Read standard XMLTV content only.</summary>
    Standard = 0,

    /// <summary>Enable supported Jellyfin XMLTV dialect fields, currently the programme <c>live</c> marker.</summary>
    Jellyfin = 1
}
