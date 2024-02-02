using MusicDownloader.Pocos.User;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Options._base;

namespace MusicDownloader.Shared.Options;

public class ApiOptions : SettingsBase
{
    public List<ApiUser> Users { get; set; }

    public override void SetFromEnvironmentVariables()
    {
        // Api settings
        Users = GetValue<string[]>(ApplicationConstants.EnvKeys.ApiUsers)
            .Select(apiUser =>
            {
                var userDetails = apiUser.Split(":");
                return new ApiUser(userDetails[0], userDetails[1]);
            })
            .ToList();
    }
}
