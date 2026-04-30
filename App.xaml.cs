using System;
using System.Linq;
using System.Threading;
using System.Windows;
using DesktopTaskWidget.Services;

namespace DesktopTaskWidget
{
    public partial class App : Application
    {
        private Mutex _mutex;
        private TrayIconService _trayIconService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool createdNew;
            _mutex = new Mutex(true, "DesktopTaskWidget.SingleInstance", out createdNew);
            if (!createdNew)
            {
                Shutdown();
                return;
            }

            AppPaths.EnsureDirectories();
            SQLitePCL.Batteries_V2.Init();

            var settingsStore = new SettingsStore();
            var settings = settingsStore.Load();
            var repository = new SqliteTaskRepository(AppPaths.DatabasePath);
            repository.Initialize();

            var window = new MainWindow(repository, settingsStore, settings);
            _trayIconService = new TrayIconService(window);
            _trayIconService.Initialize();

            bool isStartupLaunch = e.Args.Any(arg =>
                string.Equals(arg, "--startup", StringComparison.OrdinalIgnoreCase));

            if (!isStartupLaunch || settings.ShowWidgetOnStartup)
            {
                window.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_trayIconService != null)
            {
                _trayIconService.Dispose();
            }

            if (_mutex != null)
            {
                _mutex.Dispose();
            }

            base.OnExit(e);
        }
    }
}
