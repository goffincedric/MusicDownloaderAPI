using System.ComponentModel.DataAnnotations;

namespace MusicDownloader.Api.Models.Write;

public class AuthRequest
{
    [Required]
    public string ApiToken { get; set; } = null!;
}
