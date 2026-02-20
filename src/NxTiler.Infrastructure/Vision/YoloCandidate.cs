using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Vision;

internal readonly record struct YoloCandidate(int ClassId, float Confidence, WindowBounds Bounds);
