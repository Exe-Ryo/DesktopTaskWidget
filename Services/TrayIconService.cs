using System;
using System.Windows;
using Forms = System.Windows.Forms;

namespace DesktopTaskWidget.Services
{
    public class TrayIconService : IDisposable
    {
        private readonly MainWindow _window;
        private Forms.NotifyIcon _notifyIcon;

        public TrayIconService(MainWindow window)
        {
            _window = window;
        }

        public void Initialize()
        {
            var menu = new Forms.ContextMenuStrip();
            menu.Items.Add("表示", null, (sender, args) => _window.ShowFromTray());
            menu.Items.Add("隠す", null, (sender, args) => _window.Hide());
            menu.Items.Add("終了", null, (sender, args) => Application.Current.Shutdown());

            _notifyIcon = new Forms.NotifyIcon
            {
                Text = "Desktop Task Widget",
                Icon = System.Drawing.SystemIcons.Application,
                Visible = true,
                ContextMenuStrip = menu
            };

            _notifyIcon.DoubleClick += (sender, args) => _window.ShowFromTray();
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
        }
    }
}

