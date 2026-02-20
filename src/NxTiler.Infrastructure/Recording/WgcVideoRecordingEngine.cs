using System.Diagnostics;
using System.Drawing;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine(
    ISettingsService settingsService,
    ILogger<WgcVideoRecordingEngine> logger) : IVideoRecordingEngine
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly ManualResetEventSlim _pauseGate = new(initialState: true);
    private readonly ProcessStderrTail _stderrTail = new();

    private Process? _ffmpeg;
    private Task? _framePumpTask;
    private CancellationTokenSource? _framePumpCts;

    private nint _targetWindow;
    private Rectangle _captureRect = Rectangle.Empty;
    private int _fps;
    private bool _includeCursor;

    private string _ffmpegPath = "ffmpeg";
    private string _outputFolder = string.Empty;
    private string _sessionPrefix = string.Empty;
    private string? _rawOutputPath;

    public bool IsRunning { get; private set; }
}
