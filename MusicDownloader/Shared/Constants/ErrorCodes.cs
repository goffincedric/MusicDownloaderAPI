using System.Reflection;

namespace MusicDownloader.Shared.Constants;

public static class ErrorCodes
{
    public static class Youtube
    {
        // Classname to use in rest of error codes
        private static readonly string ClassName = (MethodBase.GetCurrentMethod()?.DeclaringType?.Name ?? nameof(Youtube)).ToUpperInvariant();

        // Error codes        
        public static readonly string LivestreamDownloadNotAllowed = $"{ClassName}_LIVESTREAM_DOWNLOAD_NOT_ALLOWED";
        public static readonly string LongVideoDownloadNotAllowed = $"{ClassName}_LONG_VIDEO_DOWNLOAD_NOT_ALLOWED";
    }

}