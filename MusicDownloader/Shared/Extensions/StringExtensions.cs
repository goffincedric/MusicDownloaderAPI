using System.Globalization;

namespace MusicDownloader.Shared.Extensions;

public static class StringExtensions
{
    public static string ReplaceIgnoreCase(this string s, string oldValue, string newValue)
    {
        return s.Replace(oldValue, newValue, true, CultureInfo.InvariantCulture);
    }
}