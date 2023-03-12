namespace MusicDownloader.Controllers.Models;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; }
    public string?[] ExtraInfo { get; set; }
}