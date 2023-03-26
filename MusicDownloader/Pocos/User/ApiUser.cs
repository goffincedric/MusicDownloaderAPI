namespace MusicDownloader.Pocos.User;

public class ApiUser
{
    public string Name { get; init; }
    public string Uuid { get; init; }

    public ApiUser(string name, string uuid)
    {
        Name = name;
        Uuid = uuid;
    }
}