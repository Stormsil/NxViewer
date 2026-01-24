using System.Windows;

namespace NxTiler
{
    public partial class OverlayWindow : Window
    {
        public event EventHandler? RequestArrange;
        public event EventHandler<bool>? RequestToggleAuto;
        public event EventHandler? RequestClose;

        public OverlayWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = SystemParameters.WorkArea.Left + 20;
            Top = SystemParameters.WorkArea.Top + 20;

            // Возможность перетаскивать панель за любое место
            MouseLeftButtonDown += (_, __) => DragMove();
        }

        public void SetAuto(bool on)
        {
            BtnAuto.IsChecked = on;
            BtnAuto.Content = on ? "Автораскладка ON" : "Автораскладка OFF";
        }

        private void BtnArrange_Click(object sender, RoutedEventArgs e) => RequestArrange?.Invoke(this, EventArgs.Empty);
        private void BtnAuto_Checked(object sender, RoutedEventArgs e) => RequestToggleAuto?.Invoke(this, true);
        private void BtnAuto_Unchecked(object sender, RoutedEventArgs e) => RequestToggleAuto?.Invoke(this, false);
        private void BtnClose_Click(object sender, RoutedEventArgs e) => RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
