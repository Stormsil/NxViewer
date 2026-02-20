using NxTiler.Domain.Enums;
using NxTiler.Domain.Settings;

namespace NxTiler.Application.Abstractions;

public interface IHotkeyService : IDisposable
{
    Task RegisterAllAsync(HotkeysSettings settings, CancellationToken ct = default);

    Task UnregisterAllAsync(CancellationToken ct = default);
}
