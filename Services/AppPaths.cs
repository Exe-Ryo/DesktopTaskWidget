using System;
using System.IO;

namespace DesktopTaskWidget.Services
{
    public static class AppPaths
    {
        public static string AppDataDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DesktopTaskWidget");

        public static string DatabasePath => Path.Combine(AppDataDirectory, "tasks.db");

        public static string SettingsPath => Path.Combine(AppDataDirectory, "settings.json");

        public static void EnsureDirectories()
        {
            Directory.CreateDirectory(AppDataDirectory);
        }
    }
}

