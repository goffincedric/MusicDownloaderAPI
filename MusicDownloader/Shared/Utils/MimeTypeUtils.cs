using System.Net;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;

namespace MusicDownloader.Shared.Utils;

public static class MimeTypeUtils
{
    private static readonly Dictionary<string, string> MimeTypeMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        // MP3
        { "audio/mpeg", ContainerConstants.Containers.Mp3 },
        { "audio/mpeg3", ContainerConstants.Containers.Mp3 },
        { "audio/mp3", ContainerConstants.Containers.Mp3 },
        // Webm
        { "audio/webm", ContainerConstants.Containers.Webm },
        { "audio/weba", ContainerConstants.Containers.Webm },
        // Ogg   
        { "audio/ogg", ContainerConstants.Containers.Ogg },
        // Opus
        { "audio/opus", ContainerConstants.Containers.Opus },
        // Aac
        { "audio/aac", ContainerConstants.Containers.Aac },
    };

    public static string MapMimeTypeToExtension(string mimeType)
    {
        // First try our own mapping
        var foundValue = MimeTypeMapping.TryGetValue(mimeType, out var extension);
        if (foundValue) return extension!;
        // Next try mime types mapping
        extension = MimeTypes.GetMimeTypeExtensions(mimeType).FirstOrDefault();
        if (!string.IsNullOrEmpty(extension)) return extension;
        // Else, throw error
        throw new MusicDownloaderException($"Unmapped mime type: {mimeType}", ErrorCodes.UnmappedMimeType, HttpStatusCode.InternalServerError);
    }
}