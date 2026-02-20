using NxTiler.Domain.Capture;

namespace NxTiler.Application.Abstractions;

public interface ICaptureService
{
    Task<CaptureResult> CaptureAsync(CaptureRequest request, CancellationToken ct = default);
}
