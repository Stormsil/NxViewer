using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NxTiler
{
    public partial class DimOverlayWindow : Window
    {
        public DimOverlayWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows fullscreen dimming with a rectangular cutout for the recording area.
        /// All coordinates are in physical pixels; converted to WPF DIPs internally.
        /// </summary>
        public void SetCutout(int screenX, int screenY, int screenW, int screenH,
                              int cutX, int cutY, int cutW, int cutH)
        {
            // Convert physical pixels to WPF DIPs
            var dpi = VisualTreeHelper.GetDpi(this);
            double scaleX = dpi.DpiScaleX;
            double scaleY = dpi.DpiScaleY;

            double totalW = screenW / scaleX;
            double totalH = screenH / scaleY;
            double cx = (cutX - screenX) / scaleX;
            double cy = (cutY - screenY) / scaleY;
            double cw = cutW / scaleX;
            double ch = cutH / scaleY;

            Left = screenX / scaleX;
            Top = screenY / scaleY;
            Width = totalW;
            Height = totalH;

            // Create geometry with cutout
            var fullRect = new RectangleGeometry(new Rect(0, 0, totalW, totalH));
            var cutoutRect = new RectangleGeometry(new Rect(cx, cy, cw, ch));
            var combined = new CombinedGeometry(GeometryCombineMode.Exclude, fullRect, cutoutRect);

            DimCanvas.Children.Clear();
            var path = new Path
            {
                Data = combined,
                Fill = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0))
            };
            DimCanvas.Children.Add(path);
        }
    }
}
