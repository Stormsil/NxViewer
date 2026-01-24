using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        public event EventHandler? RequestArrange;
        public event EventHandler<bool>? RequestToggleAuto;
        public event EventHandler<bool>? RequestToggleMinimize;
        public event EventHandler<bool>? RequestToggleMain; // New event
        public event EventHandler? RequestClose;
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
        public void SetModeText(string text) => ModeText.Text = text;

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
    }
}
