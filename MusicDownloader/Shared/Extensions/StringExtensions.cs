using System.Globalization;
using System.Text;

namespace MusicDownloader.Shared.Extensions;

public static class StringExtensions
{
    private static readonly HashSet<char> UnsafeFilenameChars = [..Path.GetInvalidFileNameChars()];

    public static string ReplaceIgnoreCase(this string s, string oldValue, string newValue) =>
        s.Replace(oldValue, newValue, true, CultureInfo.InvariantCulture);

    public static string ToSafeFilename(this string s)
    {
        var sanitizedFileName = s.ToCharArray();
        for (var i = 0; i < sanitizedFileName.Length; i++)
        {
            if (UnsafeFilenameChars.Contains(sanitizedFileName[i]))
                sanitizedFileName[i] = '_';
        }

        return new string(sanitizedFileName);
    }

    public static string ToPrintableASCIIOnly(this string s)
    {
        var asciiString = s.Where(ch => ch >= 32 && ch < 127);
        var sb = new StringBuilder();
        foreach (var ch in asciiString)
            sb.Append(ch);
        return sb.ToString();
    }

    public static string EncodeBase64(this string s) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(s));

    public static string DecodeBase64(this string s) =>
        Encoding.UTF8.GetString(Convert.FromBase64String(s));
}
