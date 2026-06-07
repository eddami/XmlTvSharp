namespace XmlTvSharp.Models;

/// <summary>
///     Represents an XMLTV <c>image</c> element and its optional classification attributes.
/// </summary>
public sealed record XmlTvImage
{
    /// <summary>
    ///     Initializes an image.
    /// </summary>
    /// <param name="value">The image reference stored as element content.</param>
    /// <param name="type">The optional image type.</param>
    /// <param name="size">The optional relative image size.</param>
    /// <param name="orientation">The optional image orientation.</param>
    /// <param name="system">The optional provider-defined image system.</param>
    public XmlTvImage(
        string value,
        XmlTvImageType? type = null,
        XmlTvImageSize? size = null,
        XmlTvImageOrientation? orientation = null,
        string? system = null)
    {
        Value = XmlTvArgument.NotEmpty(value, nameof(value));
        Type = ValidateEnum(type, nameof(type));
        Size = ValidateEnum(size, nameof(size));
        Orientation = ValidateEnum(orientation, nameof(orientation));
        System = system;
    }

    /// <summary>Gets the image reference stored as element content.</summary>
    public string Value { get; }

    /// <summary>Gets the optional image type.</summary>
    public XmlTvImageType? Type { get; }

    /// <summary>Gets the optional relative image size.</summary>
    public XmlTvImageSize? Size { get; }

    /// <summary>Gets the optional image orientation.</summary>
    public XmlTvImageOrientation? Orientation { get; }

    /// <summary>Gets the optional provider-defined image system.</summary>
    public string? System { get; }

    private static TEnum? ValidateEnum<TEnum>(TEnum? value, string parameterName)
        where TEnum : struct
    {
        if (value is not null && !Enum.IsDefined(typeof(TEnum), value.Value))
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }

        return value;
    }
}
