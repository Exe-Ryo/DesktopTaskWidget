using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace DesktopTaskWidget.Services
{
    public static class StartupManager
    {
        private const string AppName = "DesktopTaskWidget";
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public static void SetEnabled(bool enabled)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true))
            {
                if (key == null)
                {
                    return;
                }

                if (enabled)
                {
                    var exePath = Process.GetCurrentProcess().MainModule.FileName;
                    key.SetValue(AppName, "\"" + exePath + "\" --startup");
                }
                else
                {
                    key.DeleteValue(AppName, throwOnMissingValue: false);
                }
            }
        }

        public static bool IsEnabled()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false))
            {
                return key?.GetValue(AppName) != null;
            }
        }
    }
}

