using NxTiler.Domain.Vision;

namespace NxTiler.Application.Abstractions;

public interface IVisionEngine
{
    string Name { get; }

    Task<IReadOnlyList<VisionDetection>> DetectAsync(VisionRequest request, CancellationToken ct = default);
}
