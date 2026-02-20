using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NxTiler
{
    public partial class RecordingBarWindow : Window
    {
        private static readonly Brush PausedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f39c12"));
        private static readonly Brush RecordingBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c"));
        private static readonly Brush SavingBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8e9aa6"));

        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(500) };
        private DateTime _startTime;
        private TimeSpan _accumulatedTime = TimeSpan.Zero;
        private bool _isPaused;

        public event EventHandler? RequestPause;
        public event EventHandler? RequestResume;
        public event EventHandler? RequestStop;

        public RecordingBarWindow()
        {
            InitializeComponent();
            _timer.Tick += Timer_Tick;
        }

        public void StartTimer()
        {
            _startTime = DateTime.UtcNow;
            _accumulatedTime = TimeSpan.Zero;
            _isPaused = false;
            _timer.Start();
            UpdateDisplay();
        }

        public void SetPaused()
        {
            // Accumulate elapsed time before pausing
            _accumulatedTime += DateTime.UtcNow - _startTime;
            _isPaused = true;

            RecDot.Fill = PausedBrush;
            PauseIcon.Text = "▶";
            BtnPause.ToolTip = "Resume";
            UpdateDisplay();
        }

        public void SetResumed()
        {
            _startTime = DateTime.UtcNow;
            _isPaused = false;

            RecDot.Fill = RecordingBrush;
            PauseIcon.Text = "⏸";
            BtnPause.ToolTip = "Pause";
            UpdateDisplay();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var elapsed = _accumulatedTime;
            if (!_isPaused)
                elapsed += DateTime.UtcNow - _startTime;

            TimerText.Text = elapsed.ToString(@"hh\:mm\:ss");

            // Blink the dot when recording
            if (!_isPaused)
            {
                RecDot.Opacity = (int)elapsed.TotalSeconds % 2 == 0 ? 1.0 : 0.6;
            }
            else
            {
                RecDot.Opacity = 1.0;
            }
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPaused)
                RequestResume?.Invoke(this, EventArgs.Empty);
            else
                RequestPause?.Invoke(this, EventArgs.Empty);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            RequestStop?.Invoke(this, EventArgs.Empty);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void SetSaving()
        {
            _timer.Stop();
            BtnPause.IsEnabled = false;
            BtnStop.IsEnabled = false;
            TimerText.Text = "Saving...";
            RecDot.Fill = SavingBrush;
            RecDot.Opacity = 1.0;
        }

        public void Cleanup()
        {
            _timer.Stop();
        }
    }
}
