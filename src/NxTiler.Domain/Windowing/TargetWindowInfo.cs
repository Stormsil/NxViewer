using NxTiler.Domain.Tracking;

namespace NxTiler.Domain.Windowing;

public sealed record TargetWindowInfo(
    nint Handle,
    string Title,
    string SourceName,
    bool IsMaximized,
    uint ProcessId
)
{
    public string HandleHex => $"0x{Handle.ToInt64():X}";

    public WindowIdentity? Identity { get; init; }
}
