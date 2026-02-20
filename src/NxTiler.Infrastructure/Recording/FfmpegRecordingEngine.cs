using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine(ILogger<FfmpegRecordingEngine> logger) : IRecordingEngine
{
    private Process? _ffmpeg;
    private readonly List<string> _segments = [];
    private string _outputFolder = string.Empty;
    private string _ffmpegPath = "ffmpeg";
    private string _sessionPrefix = string.Empty;
    private int _sourceX;
    private int _sourceY;
    private int _actualLeft;
    private int _actualTop;
    private int _x;
    private int _y;
    private int _width;
    private int _height;
    private int _fps;

    private readonly ProcessStderrTail _stderrTail = new();

    public bool IsRunning => _ffmpeg is not null && !_ffmpeg.HasExited;

    public string? LastError { get; private set; }
}
