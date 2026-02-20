using NxTiler.Domain.Vision;

namespace NxTiler.Application.Abstractions;

public interface IVisionWorkflowService
{
    bool IsEnabled { get; }

    Task<VisionWorkflowResult> ToggleModeAsync(nint targetWindow, CancellationToken ct = default);

    Task<VisionWorkflowResult> RunScanAsync(nint targetWindow, CancellationToken ct = default);
}
