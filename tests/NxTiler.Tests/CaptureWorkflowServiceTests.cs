using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Settings;
using NxTiler.Domain.Windowing;

namespace NxTiler.Tests;

public sealed class CaptureWorkflowServiceTests
{
    [Fact]
    public async Task RunInstantSnapshotAsync_ResolvesTarget_AndCallsCaptureService()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Capture = new CaptureSettings(
                    PreferWgc: true,
                    CopySnapshotToClipboardByDefault: false,
                    SnapshotFolder: "C:\\Temp"),
            });

        var captureService = new Mock<ICaptureService>(MockBehavior.Strict);
        captureService
            .Setup(x => x.CaptureAsync(
                It.Is<CaptureRequest>(r =>
                    r.Mode == CaptureMode.InstantWindowSnapshot
                    && r.TargetWindow == (nint)5
                    && r.CopyToClipboard == false),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CaptureResult(true, "C:\\Temp\\shot.png", [1, 2, 3], new WindowBounds(0, 0, 100, 100)));

        var queryService = new Mock<IWindowQueryService>(MockBehavior.Strict);
        queryService
            .Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new TargetWindowInfo((nint)4, "NoMachine - A", "WoW1", true, 1001),
                new TargetWindowInfo((nint)5, "NoMachine - B", "WoW2", false, 1002),
            ]);

        var windowControlService = new Mock<IWindowControlService>(MockBehavior.Strict);

        var service = new CaptureWorkflowService(
            captureService.Object,
            queryService.Object,
            windowControlService.Object,
            settings,
            NullLogger<CaptureWorkflowService>.Instance);

        var result = await service.RunInstantSnapshotAsync(targetWindow: nint.Zero);

        Assert.True(result.Success);
        Assert.Equal("C:\\Temp\\shot.png", result.FilePath);
        captureService.VerifyAll();
        queryService.VerifyAll();
    }

    [Fact]
    public async Task RunInstantSnapshotAsync_ReturnsFailure_WhenNoWindowsFound()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var captureService = new Mock<ICaptureService>(MockBehavior.Strict);

        var queryService = new Mock<IWindowQueryService>(MockBehavior.Strict);
        queryService
            .Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TargetWindowInfo>());

        var windowControlService = new Mock<IWindowControlService>(MockBehavior.Strict);

        var service = new CaptureWorkflowService(
            captureService.Object,
            queryService.Object,
            windowControlService.Object,
            settings,
            NullLogger<CaptureWorkflowService>.Instance);

        var result = await service.RunInstantSnapshotAsync(targetWindow: nint.Zero);

        Assert.False(result.Success);
        Assert.Equal("No target window found for snapshot.", result.ErrorMessage);
        captureService.VerifyNoOtherCalls();
        queryService.VerifyAll();
    }

    [Fact]
    public async Task RunRegionSnapshotAsync_UsesClientAreaAndMasks()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Capture = new CaptureSettings(
                    PreferWgc: true,
                    CopySnapshotToClipboardByDefault: true,
                    SnapshotFolder: "C:\\Temp"),
            });

        var masks = new[]
        {
            new CaptureMask(10, 10, 30, 40),
        };

        var captureService = new Mock<ICaptureService>(MockBehavior.Strict);
        captureService
            .Setup(x => x.CaptureAsync(
                It.Is<CaptureRequest>(r =>
                    r.Mode == CaptureMode.RegionSnapshot
                    && r.TargetWindow == (nint)7
                    && r.Region == new WindowBounds(100, 200, 500, 300)
                    && r.Masks == masks),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CaptureResult(true, "C:\\Temp\\region.png", [4, 5], new WindowBounds(100, 200, 500, 300)));

        var queryService = new Mock<IWindowQueryService>(MockBehavior.Strict);

        var windowControlService = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControlService
            .Setup(x => x.MaximizeAsync((nint)7, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        windowControlService
            .Setup(x => x.GetClientAreaScreenBoundsAsync((nint)7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 200, 500, 300));

        var service = new CaptureWorkflowService(
            captureService.Object,
            queryService.Object,
            windowControlService.Object,
            settings,
            NullLogger<CaptureWorkflowService>.Instance);

        var result = await service.RunRegionSnapshotAsync((nint)7, masks);

        Assert.True(result.Success);
        captureService.VerifyAll();
        windowControlService.VerifyAll();
        queryService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RunRegionSnapshotAsync_UsesInteractiveSelection_WhenServiceIsProvided()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var selectedMasks = new[]
        {
            new CaptureMask(120, 220, 50, 60),
        };

        var captureService = new Mock<ICaptureService>(MockBehavior.Strict);
        captureService
            .Setup(x => x.CaptureAsync(
                It.Is<CaptureRequest>(r =>
                    r.Mode == CaptureMode.RegionSnapshot
                    && r.TargetWindow == (nint)8
                    && r.Region == new WindowBounds(100, 200, 500, 300)
                    && r.Masks == selectedMasks),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CaptureResult(true, "C:\\Temp\\interactive.png", [1], new WindowBounds(100, 200, 500, 300)));

        var queryService = new Mock<IWindowQueryService>(MockBehavior.Strict);

        var windowControlService = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControlService
            .Setup(x => x.MaximizeAsync((nint)8, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        windowControlService
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var selectionService = new Mock<ISnapshotSelectionService>(MockBehavior.Strict);
        selectionService
            .Setup(x => x.SelectRegionAndMasksAsync(new WindowBounds(0, 0, 1920, 1080), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SnapshotSelectionResult(new WindowBounds(100, 200, 500, 300), selectedMasks));

        var service = new CaptureWorkflowService(
            captureService.Object,
            queryService.Object,
            windowControlService.Object,
            settings,
            NullLogger<CaptureWorkflowService>.Instance,
            selectionService.Object);

        var result = await service.RunRegionSnapshotAsync((nint)8, Array.Empty<CaptureMask>());

        Assert.True(result.Success);
        captureService.VerifyAll();
        windowControlService.VerifyAll();
        selectionService.VerifyAll();
        queryService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RunRegionSnapshotAsync_ReturnsFailure_WhenInteractiveSelectionIsCanceled()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var captureService = new Mock<ICaptureService>(MockBehavior.Strict);
        var queryService = new Mock<IWindowQueryService>(MockBehavior.Strict);

        var windowControlService = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControlService
            .Setup(x => x.MaximizeAsync((nint)9, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        windowControlService
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var selectionService = new Mock<ISnapshotSelectionService>(MockBehavior.Strict);
        selectionService
            .Setup(x => x.SelectRegionAndMasksAsync(new WindowBounds(0, 0, 1920, 1080), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SnapshotSelectionResult?)null);

        var service = new CaptureWorkflowService(
            captureService.Object,
            queryService.Object,
            windowControlService.Object,
            settings,
            NullLogger<CaptureWorkflowService>.Instance,
            selectionService.Object);

        var result = await service.RunRegionSnapshotAsync((nint)9, Array.Empty<CaptureMask>());

        Assert.False(result.Success);
        Assert.Equal("Region snapshot canceled.", result.ErrorMessage);
        captureService.VerifyNoOtherCalls();
        queryService.VerifyNoOtherCalls();
        windowControlService.VerifyAll();
        selectionService.VerifyAll();
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
