namespace NxTiler.Domain.Settings;

public sealed record LayoutSettings(
    int Gap,
    int TopPad,
    bool SuspendOnMax,
    int DragCooldownMs
);
