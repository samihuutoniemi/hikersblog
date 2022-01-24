namespace HikersBlog.Misc.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
    {
        return collection == null ? Enumerable.Empty<T>() : collection;
    }
}
