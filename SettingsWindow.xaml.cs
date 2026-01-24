using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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

        public SettingsWindow()
        {
            InitializeComponent();
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

        private void FolderBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => RefreshFileList();
        private void FilterBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => RefreshFileList();

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
                Description = "Выберите папку с файлами .nxs (NoMachine)",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };
            if (dlg.ShowDialog(this) == true)
                FolderBox.Text = dlg.SelectedPath;
        }

        private void SaveSettings()
        {
            AppSettings.Default.TitleFilter = TitleFilterBox.Text;
            AppSettings.Default.NameFilter = FilterBox.Text;
            AppSettings.Default.NxsFolder = FolderBox.Text;
            
            if (int.TryParse(GapBox.Text, out int gap)) AppSettings.Default.Gap = gap;
            if (int.TryParse(TopPadBox.Text, out int pad)) AppSettings.Default.TopPad = pad;
            
            AppSettings.Default.SortDesc = SortDescCheck.IsChecked == true;
            AppSettings.Default.SuspendOnMax = SuspendOnMaxCheck.IsChecked == true;

            // Save Disabled Files
            var disabled = _fileItems.Where(i => !i.IsEnabled).Select(i => i.Name).ToArray();
            AppSettings.Default.DisabledFiles.Clear();
            AppSettings.Default.DisabledFiles.AddRange(disabled);

            AppSettings.Default.Save();
        }
    }
}
