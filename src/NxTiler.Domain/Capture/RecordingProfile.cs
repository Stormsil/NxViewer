namespace NxTiler.Domain.Capture;

public sealed record RecordingProfile(
    int FramesPerSecond,
    bool IncludeCursor = false,
    bool CaptureAudio = false
);
