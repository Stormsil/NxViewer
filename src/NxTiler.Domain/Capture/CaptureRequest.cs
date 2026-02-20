using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.Domain.Capture;

public sealed record CaptureRequest(
    CaptureMode Mode,
    nint TargetWindow,
    WindowBounds? Region = null,
    bool SaveToDisk = true,
    bool CopyToClipboard = true,
    bool IncludeCursor = false,
    IReadOnlyList<CaptureMask>? Masks = null
);
