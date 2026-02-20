namespace NxTiler.Domain.Recording;

public sealed record RecordingResult(bool Saved, string? OutputPath, string Message);
