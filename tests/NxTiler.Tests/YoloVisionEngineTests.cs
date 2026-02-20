using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Settings;
using NxTiler.Domain.Vision;
using NxTiler.Infrastructure.Vision;

namespace NxTiler.Tests;

public sealed class YoloVisionEngineTests
{
    [Fact]
    public async Task DetectAsync_Throws_WhenModelPathMissing()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Vision = AppSettingsSnapshot.CreateDefault().Vision with
                {
                    YoloModelPath = string.Empty,
                },
            });

        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        var engine = new YoloVisionEngine(
            settings,
            windowControl.Object,
            NullLogger<YoloVisionEngine>.Instance);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            engine.DetectAsync(new VisionRequest(TargetWindow: (nint)100)));

        Assert.Contains("YOLO model path is not configured", error.Message, StringComparison.OrdinalIgnoreCase);
        windowControl.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DetectAsync_Throws_WhenModelFileMissing()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Vision = AppSettingsSnapshot.CreateDefault().Vision with
                {
                    YoloModelPath = @"C:\does-not-exist\nxtiler-yolo.onnx",
                },
            });

        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        var engine = new YoloVisionEngine(
            settings,
            windowControl.Object,
            NullLogger<YoloVisionEngine>.Instance);

        var error = await Assert.ThrowsAsync<FileNotFoundException>(() =>
            engine.DetectAsync(new VisionRequest(TargetWindow: (nint)100)));

        Assert.Contains("yolo.onnx", error.FileName ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        windowControl.VerifyNoOtherCalls();
    }

    private sealed class FakeSettingsService(AppSettingsSnapshot snapshot) : ISettingsService
    {
        public AppSettingsSnapshot Current { get; private set; } = snapshot;

        public void Update(AppSettingsSnapshot updated)
        {
            Current = updated;
        }

        public Task SaveAsync(CancellationToken ct = default) => Task.CompletedTask;

        public Task ReloadAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
