using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Vision;

namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService(
    ISettingsService settingsService,
    IWindowQueryService windowQueryService,
    IEnumerable<IVisionEngine> engines,
    ILogger<VisionWorkflowService> logger) : IVisionWorkflowService
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly IReadOnlyDictionary<string, IVisionEngine> _engines = engines.ToDictionary(
        static x => x.Name,
        StringComparer.OrdinalIgnoreCase);

    public bool IsEnabled { get; private set; }
}
