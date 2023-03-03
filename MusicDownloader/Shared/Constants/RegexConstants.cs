using System.Text.RegularExpressions;

namespace MusicDownloader.Shared.Constants;

public class RegexConstants
{
    public class Youtube
    {
        public static readonly Regex ARTIST_EXTRACTION = new ("^([^-]+)\\s-\\s.*$");
        public static readonly List<Regex> VEVO_REGEXES = new()
        {
            new("^\\s?VEVO\\s?-\\s?", RegexOptions.IgnoreCase), // Vevo at start, spaces optional
            new("\\s?-\\s?VEVO\\s?$", RegexOptions.IgnoreCase), // Vevo at end, spaces optional
            new("-?\\sVEVO\\s-?$", RegexOptions.IgnoreCase), // Vevo in the string, spaces required
        };
    }
}