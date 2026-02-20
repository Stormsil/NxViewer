using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Settings;
using NxTiler.Domain.Vision;
using NxTiler.Domain.Windowing;

namespace NxTiler.Tests;

public sealed class VisionWorkflowServiceTests
{
    [Fact]
    public async Task ToggleModeAsync_EnablesMode_AndRunsScanWithPreferredEngine()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Vision = new VisionSettings(
                    Enabled: true,
                    ConfidenceThreshold: 0.65f,
                    PreferredEngine: "template"),
                FeatureFlags = new FeatureFlagsSettings(
                    UseWgcRecordingEngine: false,
                    EnableTemplateMatchingFallback: true,
                    EnableYoloEngine: false),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query
            .Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new TargetWindowInfo((nint)5, "NoMachine - A", "WoW1", false, 1001),
            ]);

        var engine = new Mock<IVisionEngine>(MockBehavior.Strict);
        engine.SetupGet(x => x.Name).Returns("template");
        engine
            .Setup(x => x.DetectAsync(
                It.Is<VisionRequest>(r => r.TargetWindow == (nint)5 && r.MinConfidence == 0.65f),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new VisionDetection(
                    Label: "hpbar",
                    Confidence: 0.91f,
                    Bounds: new WindowBounds(100, 100, 20, 12),
                    TimestampUtc: DateTime.UtcNow),
            ]);

        var service = new VisionWorkflowService(
            settings,
            query.Object,
            [engine.Object],
            NullLogger<VisionWorkflowService>.Instance);

        var result = await service.ToggleModeAsync(nint.Zero);

        Assert.True(result.Success);
        Assert.True(result.ModeEnabled);
        Assert.Equal("template", result.EngineName);
        Assert.Single(result.Detections);
        query.VerifyAll();
        engine.VerifyAll();
    }

    [Fact]
    public async Task ToggleModeAsync_DisablesMode_OnSecondCall()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query
            .Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new TargetWindowInfo((nint)8, "NoMachine - B", "WoW2", false, 1002),
            ]);

        var engine = new Mock<IVisionEngine>(MockBehavior.Strict);
        engine.SetupGet(x => x.Name).Returns("template");
        engine
            .Setup(x => x.DetectAsync(It.IsAny<VisionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<VisionDetection>());

        var service = new VisionWorkflowService(
            settings,
            query.Object,
            [engine.Object],
            NullLogger<VisionWorkflowService>.Instance);

        _ = await service.ToggleModeAsync(nint.Zero);
        var result = await service.ToggleModeAsync(nint.Zero);

        Assert.True(result.Success);
        Assert.False(result.ModeEnabled);
        Assert.Equal("Vision mode OFF.", result.Message);
    }

    [Fact]
    public async Task RunScanAsync_ReturnsFailure_WhenNoEngineEnabledByFlags()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                FeatureFlags = new FeatureFlagsSettings(
                    UseWgcRecordingEngine: false,
                    EnableTemplateMatchingFallback: false,
                    EnableYoloEngine: false),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        var engine = new Mock<IVisionEngine>(MockBehavior.Strict);
        engine.SetupGet(x => x.Name).Returns("template");

        var service = new VisionWorkflowService(
            settings,
            query.Object,
            [engine.Object],
            NullLogger<VisionWorkflowService>.Instance);

        var result = await service.RunScanAsync((nint)9);

        Assert.False(result.Success);
        Assert.Equal("No vision engine is enabled by feature flags.", result.Message);
    }

    [Fact]
    public async Task RunScanAsync_ReturnsFailure_WhenNoTargetWindow()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query
            .Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TargetWindowInfo>());

        var engine = new Mock<IVisionEngine>(MockBehavior.Strict);
        engine.SetupGet(x => x.Name).Returns("template");

        var service = new VisionWorkflowService(
            settings,
            query.Object,
            [engine.Object],
            NullLogger<VisionWorkflowService>.Instance);

        var result = await service.RunScanAsync(nint.Zero);

        Assert.False(result.Success);
        Assert.Equal("No target window found for vision.", result.Message);
        query.VerifyAll();
        engine.VerifyGet(x => x.Name, Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunScanAsync_FallsBackToTemplate_WhenYoloFails()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Vision = AppSettingsSnapshot.CreateDefault().Vision with { PreferredEngine = "yolo" },
                FeatureFlags = new FeatureFlagsSettings(
                    UseWgcRecordingEngine: false,
                    EnableTemplateMatchingFallback: true,
                    EnableYoloEngine: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query
            .Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new TargetWindowInfo((nint)21, "NoMachine - Yolo", "WoW3", false, 1003),
            ]);

        var yolo = new Mock<IVisionEngine>(MockBehavior.Strict);
        yolo.SetupGet(x => x.Name).Returns("yolo");
        yolo
            .Setup(x => x.DetectAsync(It.IsAny<VisionRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("YOLO model path is not configured."));

        var template = new Mock<IVisionEngine>(MockBehavior.Strict);
        template.SetupGet(x => x.Name).Returns("template");
        template
            .Setup(x => x.DetectAsync(
                It.Is<VisionRequest>(r => r.TargetWindow == (nint)21),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new VisionDetection(
                    Label: "target",
                    Confidence: 0.88f,
                    Bounds: new WindowBounds(10, 20, 30, 40),
                    TimestampUtc: DateTime.UtcNow),
            ]);

        var service = new VisionWorkflowService(
            settings,
            query.Object,
            [yolo.Object, template.Object],
            NullLogger<VisionWorkflowService>.Instance);

        var result = await service.RunScanAsync(nint.Zero);

        Assert.True(result.Success);
        Assert.Equal("template", result.EngineName);
        Assert.Single(result.Detections);
        Assert.Contains("fallback", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RunScanAsync_ReturnsFailure_WhenYoloFails_AndTemplateFallbackDisabled()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Vision = AppSettingsSnapshot.CreateDefault().Vision with { PreferredEngine = "yolo" },
                FeatureFlags = new FeatureFlagsSettings(
                    UseWgcRecordingEngine: false,
                    EnableTemplateMatchingFallback: false,
                    EnableYoloEngine: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query
            .Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new TargetWindowInfo((nint)22, "NoMachine - Yolo", "WoW4", false, 1004),
            ]);

        var yolo = new Mock<IVisionEngine>(MockBehavior.Strict);
        yolo.SetupGet(x => x.Name).Returns("yolo");
        yolo
            .Setup(x => x.DetectAsync(It.IsAny<VisionRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("YOLO model path is not configured."));

        var service = new VisionWorkflowService(
            settings,
            query.Object,
            [yolo.Object],
            NullLogger<VisionWorkflowService>.Instance);

        var result = await service.RunScanAsync(nint.Zero);

        Assert.False(result.Success);
        Assert.Equal("yolo", result.EngineName);
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
