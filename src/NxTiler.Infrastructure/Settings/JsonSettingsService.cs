using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService : ISettingsService
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly ILogger<JsonSettingsService> _logger;
    private readonly ISettingsMigrationService _migrationService;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonSettingsService(ILogger<JsonSettingsService> logger, ISettingsMigrationService migrationService)
    {
        _logger = logger;
        _migrationService = migrationService;

        Current = LoadInitialSnapshot();
    }

    public AppSettingsSnapshot Current { get; private set; }

    public void Update(AppSettingsSnapshot snapshot)
    {
        Current = snapshot;
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            await PersistSnapshotAsync(Current, ct);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ReloadAsync(CancellationToken ct = default)
    {
        var loadedSnapshot = await LoadAsync(ct);

        await _gate.WaitAsync(ct);
        try
        {
            Current = loadedSnapshot;
        }
        finally
        {
            _gate.Release();
        }
    }
}
