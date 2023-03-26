using MusicDownloader.Shared.Options;
using MusicDownloader.Shared.Options._base;

namespace MusicDownloader.Startup;

public static class SettingsRegistration
{
    public static void AddSettings(this IServiceCollection services)
    {
        #region Read settings

        SettingsBase.RegisterConfiguration<JwtOptions>(services);
        SettingsBase.RegisterConfiguration<ApiOptions>(services);
        
        #endregion
    }
}