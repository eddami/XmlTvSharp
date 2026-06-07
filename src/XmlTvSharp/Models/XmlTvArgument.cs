namespace XmlTvSharp.Models;

internal static class XmlTvArgument
{
    internal static string NotNull(string? value, string parameterName)
    {
        return value ?? throw new ArgumentNullException(parameterName);
    }

    internal static string NotEmpty(string? value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentException("Value cannot be empty.", parameterName);
        }

        return value;
    }

    internal static string NotWhiteSpace(string? value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", parameterName);
        }

        return value;
    }
}
