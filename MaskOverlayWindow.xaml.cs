using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NxTiler
{
    public partial class MaskOverlayWindow : Window
    {
        private static readonly Brush MaskStrokeBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c"));
        private static readonly Brush MaskFillBrush = new SolidColorBrush(Color.FromArgb(180, 0x23, 0x28, 0x2f));

        private readonly List<Rectangle> _masks = new();
        private Point _dragStart;
        private Rectangle? _currentRect;
        private bool _isDragging;
        private bool _isRecordingMode;

        public event EventHandler? ConfirmMasks;
        public event EventHandler? CancelEditing;

        public MaskOverlayWindow()
        {
            InitializeComponent();
        }

        public List<Rect> GetMaskRects()
        {
            var rects = new List<Rect>();
            foreach (var mask in _masks)
            {
                double x = Canvas.GetLeft(mask);
                double y = Canvas.GetTop(mask);
                rects.Add(new Rect(x, y, mask.Width, mask.Height));
            }
            return rects;
        }

        public void EnterRecordingMode()
        {
            _isRecordingMode = true;
            EditBorder.BorderThickness = new Thickness(0);
            StatusBar.Visibility = Visibility.Collapsed;

            // Keep semi-transparent fill, just remove editing stroke
            foreach (var mask in _masks)
            {
                mask.Stroke = null;
                mask.StrokeThickness = 0;
            }

            MakeClickThrough();
        }

        public void EnterPauseEditMode()
        {
            _isRecordingMode = false;
            MakeInteractive();

            EditBorder.BorderThickness = new Thickness(3);
            StatusBar.Visibility = Visibility.Visible;
            StatusText.Text = "LMB — draw mask. RMB — remove. F3 — resume recording.";

            foreach (var mask in _masks)
            {
                mask.Stroke = MaskStrokeBrush;
                mask.StrokeThickness = 2;
            }

            Focus();
        }

        public void ReEnterRecordingMode()
        {
            _isRecordingMode = true;
            EditBorder.BorderThickness = new Thickness(0);
            StatusBar.Visibility = Visibility.Collapsed;

            foreach (var mask in _masks)
            {
                mask.Stroke = null;
                mask.StrokeThickness = 0;
            }

            MakeClickThrough();
        }

        private void MakeClickThrough()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            var style = Win32.GetWindowLong(hwnd, Win32.GWL_EXSTYLE);
            Win32.SetWindowLong(hwnd, Win32.GWL_EXSTYLE,
                style | Win32.WS_EX_TRANSPARENT | Win32.WS_EX_LAYERED);
        }

        private void MakeInteractive()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            var style = Win32.GetWindowLong(hwnd, Win32.GWL_EXSTYLE);
            Win32.SetWindowLong(hwnd, Win32.GWL_EXSTYLE,
                style & ~Win32.WS_EX_TRANSPARENT);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isRecordingMode) return;

            _dragStart = e.GetPosition(MaskCanvas);
            _currentRect = new Rectangle
            {
                Stroke = MaskStrokeBrush,
                StrokeThickness = 2,
                Fill = MaskFillBrush,
                Width = 0,
                Height = 0
            };
            Canvas.SetLeft(_currentRect, _dragStart.X);
            Canvas.SetTop(_currentRect, _dragStart.Y);
            MaskCanvas.Children.Add(_currentRect);
            _isDragging = true;
            e.Handled = true;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _currentRect == null) return;

            var pos = e.GetPosition(MaskCanvas);
            double x = Math.Min(pos.X, _dragStart.X);
            double y = Math.Min(pos.Y, _dragStart.Y);
            double w = Math.Abs(pos.X - _dragStart.X);
            double h = Math.Abs(pos.Y - _dragStart.Y);

            Canvas.SetLeft(_currentRect, x);
            Canvas.SetTop(_currentRect, y);
            _currentRect.Width = w;
            _currentRect.Height = h;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging || _currentRect == null) return;
            _isDragging = false;

            if (_currentRect.Width < 5 || _currentRect.Height < 5)
            {
                MaskCanvas.Children.Remove(_currentRect);
            }
            else
            {
                _masks.Add(_currentRect);
            }
            _currentRect = null;
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isRecordingMode) return;

            var pos = e.GetPosition(MaskCanvas);
            for (int i = _masks.Count - 1; i >= 0; i--)
            {
                var mask = _masks[i];
                double mx = Canvas.GetLeft(mask);
                double my = Canvas.GetTop(mask);
                if (pos.X >= mx && pos.X <= mx + mask.Width &&
                    pos.Y >= my && pos.Y <= my + mask.Height)
                {
                    MaskCanvas.Children.Remove(mask);
                    _masks.RemoveAt(i);
                    break;
                }
            }
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CancelEditing?.Invoke(this, EventArgs.Empty);
            }
            else if (e.Key == Key.Enter && !_isRecordingMode)
            {
                e.Handled = true;
                ConfirmMasks?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
