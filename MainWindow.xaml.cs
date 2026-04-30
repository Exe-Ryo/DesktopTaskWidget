using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DesktopTaskWidget.Models;
using DesktopTaskWidget.Services;

namespace DesktopTaskWidget
{
    public partial class MainWindow : Window
    {
        private readonly ITaskRepository _repository;
        private readonly SettingsStore _settingsStore;
        private readonly AppSettings _settings;
        private bool _isLoadingSettings;

        public ObservableCollection<TaskItemViewModel> Tasks { get; } = new ObservableCollection<TaskItemViewModel>();

        public MainWindow(ITaskRepository repository, SettingsStore settingsStore, AppSettings settings)
        {
            InitializeComponent();
            _repository = repository;
            _settingsStore = settingsStore;
            _settings = settings;
            DataContext = this;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        public void ShowFromTray()
        {
            Show();
            Activate();
            PlaceAtBottomRight();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoadingSettings = true;
            StartupCheckBox.IsChecked = StartupManager.IsEnabled();
            ShowCompletedCheckBox.IsChecked = _settings.ShowCompletedTasks;
            _isLoadingSettings = false;

            LoadTasks();
            PlaceAtBottomRight();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _settings.Width = Width;
            _settings.Height = Height;
            _settingsStore.Save(_settings);
        }

        private void LoadTasks()
        {
            Tasks.Clear();
            foreach (var task in _repository.GetTasks(_settings.ShowCompletedTasks))
            {
                Tasks.Add(new TaskItemViewModel(task));
            }

            var activeCount = Tasks.Count(t => !t.IsCompleted);
            SummaryText.Text = activeCount + "件の未完了";
        }

        private void PlaceAtBottomRight()
        {
            Width = _settings.Width > 0 ? _settings.Width : Width;
            Height = _settings.Height > 0 ? _settings.Height : Height;

            var workArea = SystemParameters.WorkArea;
            Left = Math.Max(workArea.Left, workArea.Right - Width - 16);
            Top = Math.Max(workArea.Top, workArea.Bottom - Height - 16);
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            AddTask();
        }

        private void TitleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddTask();
            }
        }

        private void AddTask()
        {
            var title = TitleTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            _repository.AddTask(title, DueDatePicker.SelectedDate);
            TitleTextBox.Clear();
            DueDatePicker.SelectedDate = null;
            LoadTasks();
        }

        private void TaskCompleted_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is TaskItemViewModel task)
            {
                _repository.SetCompleted(task.TaskId, task.IsCompleted);
                LoadTasks();
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is TaskItemViewModel task)
            {
                _repository.DeleteTask(task.TaskId);
                LoadTasks();
            }
        }

        private void StartupCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoadingSettings)
            {
                return;
            }

            StartupManager.SetEnabled(StartupCheckBox.IsChecked == true);
        }

        private void ShowCompletedCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoadingSettings)
            {
                return;
            }

            _settings.ShowCompletedTasks = ShowCompletedCheckBox.IsChecked == true;
            _settingsStore.Save(_settings);
            LoadTasks();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class TaskItemViewModel : INotifyPropertyChanged
    {
        private readonly TaskItem _task;

        public TaskItemViewModel(TaskItem task)
        {
            _task = task;
        }

        public long TaskId => _task.TaskId;
        public string Title => _task.Title;

        public bool IsCompleted
        {
            get => _task.IsCompleted;
            set
            {
                _task.IsCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            }
        }

        public string DueDateText
        {
            get
            {
                if (!_task.DueDate.HasValue)
                {
                    return string.Empty;
                }

                var today = DateTime.Today;
                var date = _task.DueDate.Value.Date;
                if (date < today)
                {
                    return "期限切れ: " + date.ToString("yyyy/MM/dd");
                }

                if (date == today)
                {
                    return "今日まで";
                }

                return "期限: " + date.ToString("yyyy/MM/dd");
            }
        }

        public Brush DueDateBrush
        {
            get
            {
                if (!_task.DueDate.HasValue)
                {
                    return Brushes.Transparent;
                }

                var date = _task.DueDate.Value.Date;
                if (date < DateTime.Today)
                {
                    return new SolidColorBrush(Color.FromRgb(179, 58, 58));
                }

                if (date == DateTime.Today)
                {
                    return new SolidColorBrush(Color.FromRgb(46, 125, 111));
                }

                return new SolidColorBrush(Color.FromRgb(104, 115, 130));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

