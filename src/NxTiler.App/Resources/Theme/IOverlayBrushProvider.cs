using System.Windows.Media;

namespace NxTiler.App.Resources.Theme;

public interface IOverlayBrushProvider
{
    Brush OverlayRecordingBrush { get; }

    Brush OverlayPausedBrush { get; }

    Brush OverlaySavingBrush { get; }

    Brush MaskStrokeBrush { get; }

    Brush MaskFillBrush { get; }

    Brush DimOverlayBrush { get; }
}
