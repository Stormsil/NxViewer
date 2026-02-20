using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;

namespace NxTiler.Infrastructure.Vision;

public sealed partial class YoloVisionEngine : IVisionEngine, IDisposable
{
    private const string YoloModelPathVariable = "NXTILER_YOLO_MODEL";
    private const int DefaultInputSize = 640;
    private const float NmsIouThreshold = 0.45f;

    private readonly ISettingsService _settingsService;
    private readonly IWindowControlService _windowControlService;
    private readonly ILogger<YoloVisionEngine> _logger;
    private readonly IYoloSessionProvider _sessionProvider;
    private readonly IYoloPreprocessor _preprocessor;
    private readonly IYoloOutputParser _outputParser;

    public YoloVisionEngine(
        ISettingsService settingsService,
        IWindowControlService windowControlService,
        ILogger<YoloVisionEngine> logger)
        : this(
            settingsService,
            windowControlService,
            logger,
            new YoloSessionProvider(),
            new YoloPreprocessor(),
            new YoloOutputParser())
    {
    }

    internal YoloVisionEngine(
        ISettingsService settingsService,
        IWindowControlService windowControlService,
        ILogger<YoloVisionEngine> logger,
        IYoloSessionProvider sessionProvider,
        IYoloPreprocessor preprocessor,
        IYoloOutputParser outputParser)
    {
        _settingsService = settingsService;
        _windowControlService = windowControlService;
        _logger = logger;
        _sessionProvider = sessionProvider;
        _preprocessor = preprocessor;
        _outputParser = outputParser;
    }

    public string Name => "yolo";
}
