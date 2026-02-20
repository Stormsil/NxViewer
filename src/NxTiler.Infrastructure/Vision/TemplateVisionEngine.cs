using ImageSearchCL.API;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Vision;

namespace NxTiler.Infrastructure.Vision;

public sealed partial class TemplateVisionEngine(
    ISettingsService settingsService,
    IWindowControlService windowControlService,
    ILogger<TemplateVisionEngine> logger) : IVisionEngine
{
    private const string TemplateDirectoryVariable = "NXTILER_VISION_TEMPLATES";
    private const int MaxTemplateFiles = 256;

    public string Name => "template";
}
