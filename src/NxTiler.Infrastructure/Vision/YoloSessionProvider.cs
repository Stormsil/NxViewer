using Microsoft.ML.OnnxRuntime;

namespace NxTiler.Infrastructure.Vision;

internal sealed class YoloSessionProvider : IYoloSessionProvider, IDisposable
{
    private readonly object _sessionGate = new();
    private InferenceSession? _session;
    private string? _loadedModelPath;
    private IReadOnlyList<string> _labels = Array.Empty<string>();

    public YoloSessionContext GetOrCreate(string modelPath)
    {
        lock (_sessionGate)
        {
            if (_session is null || !string.Equals(_loadedModelPath, modelPath, StringComparison.OrdinalIgnoreCase))
            {
                _session?.Dispose();
                var sessionOptions = new SessionOptions
                {
                    GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_EXTENDED,
                    ExecutionMode = ExecutionMode.ORT_SEQUENTIAL,
                };
                _session = new InferenceSession(modelPath, sessionOptions);
                _loadedModelPath = modelPath;
                _labels = LoadLabels(modelPath);
            }

            return new YoloSessionContext(_session, _labels);
        }
    }

    public void Dispose()
    {
        lock (_sessionGate)
        {
            _session?.Dispose();
            _session = null;
            _loadedModelPath = null;
            _labels = Array.Empty<string>();
        }
    }

    private static IReadOnlyList<string> LoadLabels(string modelPath)
    {
        var directory = Path.GetDirectoryName(modelPath) ?? string.Empty;
        var stem = Path.GetFileNameWithoutExtension(modelPath);
        var candidates = new[]
        {
            Path.Combine(directory, $"{stem}.labels.txt"),
            Path.Combine(directory, $"{stem}.names"),
            Path.Combine(directory, "classes.txt"),
        };

        foreach (var path in candidates)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            var lines = File.ReadAllLines(path)
                .Where(static x => !string.IsNullOrWhiteSpace(x))
                .Select(static x => x.Trim())
                .ToArray();
            if (lines.Length > 0)
            {
                return lines;
            }
        }

        return Array.Empty<string>();
    }
}
