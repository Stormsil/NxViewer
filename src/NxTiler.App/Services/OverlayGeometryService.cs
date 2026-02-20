using System.Windows;
using System.Windows.Media;

namespace NxTiler.App.Services;

public sealed class OverlayGeometryService : IOverlayGeometryService
{
    public Geometry BuildCutoutGeometry(
        int screenX,
        int screenY,
        int screenW,
        int screenH,
        int cutX,
        int cutY,
        int cutW,
        int cutH,
        double scaleX,
        double scaleY)
    {
        var totalW = screenW / scaleX;
        var totalH = screenH / scaleY;
        var cx = (cutX - screenX) / scaleX;
        var cy = (cutY - screenY) / scaleY;
        var cw = cutW / scaleX;
        var ch = cutH / scaleY;

        var fullRect = new RectangleGeometry(new Rect(0, 0, totalW, totalH));
        var cutoutRect = new RectangleGeometry(new Rect(cx, cy, cw, ch));
        return new CombinedGeometry(GeometryCombineMode.Exclude, fullRect, cutoutRect);
    }
}
