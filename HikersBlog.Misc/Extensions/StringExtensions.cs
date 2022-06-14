namespace HikersBlog.Misc.Extensions;

public static class StringExtensions
{
    public static string[] SplitOnLastOccurence(this string s, char c)
    {
        var lastIndex = s.LastIndexOf(c);

        if (lastIndex != -1)
        {
            var s1 = s.Substring(0, lastIndex);
            var s2 = s.Substring(lastIndex + 1);

            return new[] { s1, s2 };
        }

        return new[] { s };
    }
}
