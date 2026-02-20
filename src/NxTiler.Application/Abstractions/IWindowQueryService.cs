using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IWindowQueryService
{
    Task<IReadOnlyList<TargetWindowInfo>> QueryAsync(WindowQueryOptions options, CancellationToken ct = default);
}
