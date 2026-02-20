using System.Windows.Media;

namespace NxTiler.App.Resources.Theme;

public sealed class OverlayBrushProvider(IResourceProvider resourceProvider) : IOverlayBrushProvider
{
    private readonly Brush _fallbackOverlayRecordingBrush = CreateFallbackBrush("#E74C3C");
    private readonly Brush _fallbackOverlayPausedBrush = CreateFallbackBrush("#F39C12");
    private readonly Brush _fallbackOverlaySavingBrush = CreateFallbackBrush("#8E9AA6");
    private readonly Brush _fallbackMaskStrokeBrush = CreateFallbackBrush("#E74C3C");
    private readonly Brush _fallbackMaskFillBrush = CreateFallbackBrush("#B423282F");
    private readonly Brush _fallbackDimOverlayBrush = CreateFallbackBrush("#A0000000");

    public Brush OverlayRecordingBrush => Resolve("OverlayRecordingBrush", _fallbackOverlayRecordingBrush);

    public Brush OverlayPausedBrush => Resolve("OverlayPausedBrush", _fallbackOverlayPausedBrush);

    public Brush OverlaySavingBrush => Resolve("OverlaySavingBrush", _fallbackOverlaySavingBrush);

    public Brush MaskStrokeBrush => Resolve("MaskOverlayMaskStrokeBrush", _fallbackMaskStrokeBrush);

    public Brush MaskFillBrush => Resolve("MaskOverlayMaskFillBrush", _fallbackMaskFillBrush);

    public Brush DimOverlayBrush => Resolve("DimOverlayFillBrush", _fallbackDimOverlayBrush);

    private Brush Resolve(string resourceKey, Brush fallback)
    {
        if (resourceProvider.TryGetResource(resourceKey, out var value) && value is Brush brush)
        {
            return brush;
        }

        return fallback;
    }

    private static Brush CreateFallbackBrush(string colorHex)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex));
        brush.Freeze();
        return brush;
    }
}
