using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NxTiler
{
    public class WindowSelectorItem
    {
        public int Index { get; set; } // Index in the _targets list
        public string Label { get; set; } = ""; // Actual session name
        public bool IsActive { get; set; }
    }

    public partial class OverlayWindow : Window
    {
        private static readonly Brush PausedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f39c12"));
        private static readonly Brush RecordingBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c"));
        public event EventHandler? RequestArrange;
        public event EventHandler<bool>? RequestToggleAuto;
        public event EventHandler<bool>? RequestToggleMinimize;
        public event EventHandler<bool>? RequestToggleMain; // New event
        public event EventHandler? RequestClose;
        public event EventHandler? RequestCycleMode;
        public event EventHandler<int>? WindowSelected;
        public event EventHandler<int>? WindowReconnect; // New event for Right Click

        public OverlayWindow()
        {
            InitializeComponent();
        }

        private void BtnArrange_Click(object sender, RoutedEventArgs e) => RequestArrange?.Invoke(this, EventArgs.Empty);
        private void BtnAuto_Checked(object sender, RoutedEventArgs e) => RequestToggleAuto?.Invoke(this, true);
        private void BtnAuto_Unchecked(object sender, RoutedEventArgs e) => RequestToggleAuto?.Invoke(this, false);
        
        private void BtnMinimize_Checked(object sender, RoutedEventArgs e) => RequestToggleMinimize?.Invoke(this, true);
        private void BtnMinimize_Unchecked(object sender, RoutedEventArgs e) => RequestToggleMinimize?.Invoke(this, false);

        private void BtnMainWin_Checked(object sender, RoutedEventArgs e) => RequestToggleMain?.Invoke(this, true);
        private void BtnMainWin_Unchecked(object sender, RoutedEventArgs e) => RequestToggleMain?.Invoke(this, false);

        private void BtnClose_Click(object sender, RoutedEventArgs e) => RequestClose?.Invoke(this, EventArgs.Empty);
        private void BtnCycleMode_Click(object sender, RoutedEventArgs e) => RequestCycleMode?.Invoke(this, EventArgs.Empty);
        
        private void Item_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                WindowSelected?.Invoke(this, index);
            }
        }

        private void Item_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                e.Handled = true;
                WindowReconnect?.Invoke(this, index);
            }
        }

        public void SetAuto(bool isAuto) => BtnAuto.IsChecked = isAuto;
        public void SetMainWin(bool isVisible) => BtnMainWin.IsChecked = isVisible;
        public void SetModeText(string text)
        {
            // ModeText is inside BtnCycleMode's ControlTemplate
            if (BtnCycleMode.Template.FindName("ModeText", BtnCycleMode) is System.Windows.Controls.TextBlock tb)
                tb.Text = text;
        }

        public void SetRecording(string? state)
        {
            if (state == null)
            {
                RecPanel.Visibility = Visibility.Collapsed;
                return;
            }
            RecPanel.Visibility = Visibility.Visible;
            RecText.Text = state;
            if (state == "PAUSED")
            {
                RecDot.Fill = PausedBrush;
                RecText.Foreground = PausedBrush;
            }
            else
            {
                RecDot.Fill = RecordingBrush;
                RecText.Foreground = RecordingBrush;
            }
        }

        public void UpdateWindowList(List<string> sessionNames, int activeIndex)
        {
            // Always visible now
            WindowSelector.Visibility = Visibility.Visible;
            var list = new List<WindowSelectorItem>();
            for (int i = 0; i < sessionNames.Count; i++)
            {
                list.Add(new WindowSelectorItem
                {
                    Index = i,
                    Label = sessionNames[i], // Use actual name
                    IsActive = (i == activeIndex)
                });
            }
            WindowSelector.ItemsSource = list;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        public void SavePosition()
        {
            if (IsVisible && WindowState == WindowState.Normal)
            {
                AppSettings.Default.OverlayLeft = Left;
                AppSettings.Default.OverlayTop = Top;
                AppSettings.Default.Save();
            }
        }
    }
}
