using System.Net;

namespace MusicDownloader.Shared.Exceptions;

public class MusicDownloaderException(
    string message,
    string errorCode,
    HttpStatusCode statusCode,
    Exception? innerException = null
) : ApplicationException(message, innerException)
{
    public string ErrorCode { get; } = errorCode;
    public HttpStatusCode StatusCode { get; } = statusCode;
}
