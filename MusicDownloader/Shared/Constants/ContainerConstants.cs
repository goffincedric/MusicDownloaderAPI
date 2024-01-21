namespace MusicDownloader.Shared.Constants;

public abstract class ContainerConstants
{
    internal abstract class Containers
    {
        // Just returns the youtube supplied audio container. No metadata will be added
        public const string Default = "*";
        
        // Transcode to a specific container
        public const string Ogg = "ogg";
        public const string Mp3 = "mp3";
        public const string Opus = "opus";
        public const string Aac = "aac";
    }
}