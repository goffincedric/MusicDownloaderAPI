using Newtonsoft.Json;

namespace MusicDownloader.Shared.Options._base
{
    public abstract class SettingsBase
    {
        public static void RegisterConfiguration<T>(IServiceCollection services) where T : SettingsBase
        {
            services.Configure<T>(settings => settings.SetFromEnvironmentVariables());
        }

        public abstract void SetFromEnvironmentVariables();
        
        protected static T GetValue<T>(string key)
        {
            var value = GetString(key);
            if (typeof(T) == typeof(string)) return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
            return JsonConvert.DeserializeObject<T>(value);
        }

        private static string GetString(string key)
        {
            return Environment.GetEnvironmentVariable(key) ??
                   throw new Exception($"Missing environment variable {key}");
        }
        

    }
}