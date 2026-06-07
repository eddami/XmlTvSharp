using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

internal static class XmlTvCollections
{
    internal static void AddRequiredItems<T>(Collection<T> target, IEnumerable<T> source, string parameterName)
        where T : class
    {
        if (source is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        foreach (var item in source)
        {
            target.Add(item ?? throw new ArgumentException("Collection cannot contain null items.", parameterName));
        }

        if (target.Count == 0)
        {
            throw new ArgumentException("Collection must contain at least one item.", parameterName);
        }
    }
}
