using System.Windows.Media;

namespace NxTiler.App.Services;

public interface IOverlayGeometryService
{
    Geometry BuildCutoutGeometry(
        int screenX,
        int screenY,
        int screenW,
        int screenH,
        int cutX,
        int cutY,
        int cutW,
        int cutH,
        double scaleX,
        double scaleY);
}
