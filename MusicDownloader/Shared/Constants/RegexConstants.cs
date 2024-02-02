using System.Text.RegularExpressions;

namespace MusicDownloader.Shared.Constants;

public static class RegexConstants
{
    public static class Youtube
    {
        public static readonly Regex ARTIST_EXTRACTION = new("^([^-]+)\\s-\\s.*$");
        public static readonly List<Regex> VevoRegexes =
        [
            new Regex(@"^\s?VEVO\s?-\s?", RegexOptions.IgnoreCase), // Vevo at start, spaces optional
            new Regex(@"\s?-\s?VEVO\s?$", RegexOptions.IgnoreCase), // Vevo at end, spaces optional
            new Regex(@"-?\sVEVO\s-?$", RegexOptions.IgnoreCase) // Vevo in the string, spaces required
        ];
    }
}
