using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;

namespace NxTiler
{
    // Simple helper class for binding
    public class ConfigFileItem : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        private bool _isEnabled;
        public bool IsEnabled 
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class SettingsWindow : Window
    {
        private List<ConfigFileItem> _fileItems = new();
        private readonly DispatcherTimer _filterDebounce;

        public SettingsWindow()
        {
            InitializeComponent();
            _filterDebounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _filterDebounce.Tick += (_, _) =>
            {
                _filterDebounce.Stop();
                RefreshFileList();
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load from AppSettings
            TitleFilterBox.Text = AppSettings.Default.TitleFilter;
            FilterBox.Text = AppSettings.Default.NameFilter;
            FolderBox.Text = AppSettings.Default.NxsFolder;
            GapBox.Text = AppSettings.Default.Gap.ToString();
            TopPadBox.Text = AppSettings.Default.TopPad.ToString();
            SortDescCheck.IsChecked = AppSettings.Default.SortDesc;
            SuspendOnMaxCheck.IsChecked = AppSettings.Default.SuspendOnMax;

            // Recording settings
            RecFolderBox.Text = AppSettings.Default.RecordingFolder;
            RecFpsBox.Text = AppSettings.Default.RecordingFps.ToString();
            FfmpegPathBox.Text = AppSettings.Default.FfmpegPath;

            // Load file list
            RefreshFileList();
        }

        private void RefreshFileList()
        {
            string folder = FolderBox.Text;
            string filter = FilterBox.Text;

            // Find ALL matching sessions first
            var sessions = NomachineLauncher.FindSessions(folder, filter);
            var disabled = new HashSet<string>(AppSettings.Default.DisabledFiles.Cast<string>(), StringComparer.OrdinalIgnoreCase);

            // Natural Sort (numeric aware)
            sessions = sessions.OrderBy(s => ParseNumericSuffix(s.name))
                               .ThenBy(s => s.name, StringComparer.OrdinalIgnoreCase)
                               .ToList();

            _fileItems = sessions.Select(s => new ConfigFileItem 
            { 
                Name = s.name,
                IsEnabled = !disabled.Contains(s.name)
            }).ToList();

            FileListBox.ItemsSource = _fileItems;
        }

        private static int ParseNumericSuffix(string s)
        {
            var m = Regex.Match(s ?? "", @"(\d+)$");
            return m.Success ? int.Parse(m.Groups[1].Value) : int.MaxValue;
        }

        private void FolderBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _filterDebounce.Stop();
            _filterDebounce.Start();
        }

        private void FilterBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _filterDebounce.Stop();
            _filterDebounce.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = FolderBox.Text,
                Description = "Select .nxs files folder (NoMachine)",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };
            if (dlg.ShowDialog(this) == true)
                FolderBox.Text = dlg.SelectedPath;
        }

        private void ChooseRecFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = RecFolderBox.Text,
                Description = "Select recording folder",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true
            };
            if (dlg.ShowDialog(this) == true)
                RecFolderBox.Text = dlg.SelectedPath;
        }

        private void SaveSettings()
        {
            // Validate regex fields
            if (!ValidateRegex(TitleFilterBox.Text, "Window Title")) return;
            if (!ValidateRegex(FilterBox.Text, "Session Name")) return;

            AppSettings.Default.TitleFilter = TitleFilterBox.Text;
            AppSettings.Default.NameFilter = FilterBox.Text;
            AppSettings.Default.NxsFolder = FolderBox.Text;

            // Validate numeric ranges
            if (int.TryParse(GapBox.Text, out int gap) && gap >= 0 && gap <= 100)
                AppSettings.Default.Gap = gap;
            else
                GapBox.Text = AppSettings.Default.Gap.ToString();

            if (int.TryParse(TopPadBox.Text, out int pad) && pad >= 0 && pad <= 500)
                AppSettings.Default.TopPad = pad;
            else
                TopPadBox.Text = AppSettings.Default.TopPad.ToString();

            AppSettings.Default.SortDesc = SortDescCheck.IsChecked == true;
            AppSettings.Default.SuspendOnMax = SuspendOnMaxCheck.IsChecked == true;

            // Recording settings
            AppSettings.Default.RecordingFolder = RecFolderBox.Text;
            if (int.TryParse(RecFpsBox.Text, out int fps) && fps >= 1 && fps <= 120)
                AppSettings.Default.RecordingFps = fps;
            else
                RecFpsBox.Text = AppSettings.Default.RecordingFps.ToString();

            AppSettings.Default.FfmpegPath = FfmpegPathBox.Text;

            // Save Disabled Files
            var disabled = _fileItems.Where(i => !i.IsEnabled).Select(i => i.Name).ToArray();
            AppSettings.Default.DisabledFiles.Clear();
            AppSettings.Default.DisabledFiles.AddRange(disabled);

            AppSettings.Default.Save();
        }

        private static bool ValidateRegex(string pattern, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return true;
            try
            {
                _ = new Regex(pattern);
                return true;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Invalid regex in \"{fieldName}\":\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }
    }
}
