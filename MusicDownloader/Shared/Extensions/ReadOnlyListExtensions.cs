namespace MusicDownloader.Shared.Extensions;

public static class ReadOnlyListExtensions
{
    public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind)
    {
        var i = 0;
        foreach (var element in self)
        {
            if (Equals(element, elementToFind))
                return i;
            i++;
        }

        return -1;
    }

    public static int FindIndex<T>(this IReadOnlyList<T> self, Func<T, bool> predicate)
    {
        for (var i = 0; i < self.Count; i++)
        {
            if (!predicate(self[i])) continue;
            return i;
        }

        return -1;
    }
}