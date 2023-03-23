using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MusicDownloader.Shared.Extensions;

public static class StringExtensions
{
    private static readonly HashSet<char> UNSAFE_FILENAME_CHARS = new(Path.GetInvalidFileNameChars());

    public static string ReplaceIgnoreCase(this string s, string oldValue, string newValue)
    {
        return s.Replace(oldValue, newValue, true, CultureInfo.InvariantCulture);
    }

    public static string ToSafeFilename(this string s)
    {
        var sanitizedFileName = s.ToCharArray();
        for (var i = 0; i < sanitizedFileName.Length; i++)
        {
            if (UNSAFE_FILENAME_CHARS.Contains(sanitizedFileName[1])) sanitizedFileName[i] = '_';
        }

        return new string(sanitizedFileName);
    }

    public static string ToPrintableASCIIOnly(this string s)
    {
        var sb = new StringBuilder();
        foreach (var ch in s)
        {
            // Printable ASCII range
            if (ch >= 32 && ch < 127) sb.Append(ch);
        }

        return sb.ToString();
    }

    public static string EncodeBase64(this string s)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
    }

    public static string DecodeBase64(this string s)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(s));
    }
}