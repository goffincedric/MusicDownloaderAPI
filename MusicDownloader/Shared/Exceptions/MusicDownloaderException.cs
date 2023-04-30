using System.Net;

namespace MusicDownloader.Shared.Exceptions;

public class MusicDownloaderException : ApplicationException
{
    public string ErrorCode { get; }
    public HttpStatusCode StatusCode { get; }

    public MusicDownloaderException(
        string message, string errorCode, HttpStatusCode statusCode, Exception? innerException = null
    ) : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}