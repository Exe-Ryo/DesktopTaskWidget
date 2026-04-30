using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DesktopTaskWidget.Services
{
    public class SettingsStore
    {
        public AppSettings Load()
        {
            if (!File.Exists(AppPaths.SettingsPath))
            {
                return new AppSettings();
            }

            using (var stream = File.OpenRead(AppPaths.SettingsPath))
            {
                var serializer = new DataContractJsonSerializer(typeof(AppSettings));
                return serializer.ReadObject(stream) as AppSettings ?? new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            AppPaths.EnsureDirectories();

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(AppSettings));
                serializer.WriteObject(stream, settings);
                File.WriteAllText(AppPaths.SettingsPath, Encoding.UTF8.GetString(stream.ToArray()), Encoding.UTF8);
            }
        }
    }
}

