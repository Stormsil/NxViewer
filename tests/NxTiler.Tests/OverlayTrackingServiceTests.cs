using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Overlay;
using NxTiler.Domain.Windowing;

namespace NxTiler.Tests;

public sealed class OverlayTrackingServiceTests
{
    [Fact]
    public async Task StartAsync_EmitsTrackingState_ForTargetWindow()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .Setup(x => x.GetWindowBoundsAsync((nint)10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 200, 800, 600));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        await using var service = new OverlayTrackingService(
            windowControl.Object,
            new TestCursorPositionProvider(10, 10),
            NullLogger<OverlayTrackingService>.Instance);

        var stateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.TrackingStateChanged += (_, state) =>
        {
            if (state.IsVisible)
            {
                stateTcs.TrySetResult(state);
            }
        };

        await service.StartAsync(
            targetWindow: (nint)10,
            new OverlayTrackingRequest(
                BaseWidth: 560,
                BaseHeight: 56,
                VisibilityMode: OverlayVisibilityMode.Always,
                ScaleWithWindow: false));

        var state = await stateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.True(state.IsVisible);
        Assert.Equal(108, state.Left, precision: 0);
        Assert.Equal(208, state.Top, precision: 0);
        Assert.Equal(560, state.Width, precision: 0);
        Assert.Equal(56, state.Height, precision: 0);
    }

    [Fact]
    public async Task StartAsync_WithScaleWithWindow_EmitsScaledDimensions()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .SetupSequence(x => x.GetWindowBoundsAsync((nint)11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 200, 800, 600))
            .ReturnsAsync(new WindowBounds(100, 200, 1600, 1200))
            .ReturnsAsync(new WindowBounds(100, 200, 1600, 1200));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 2560, 1440));

        await using var service = new OverlayTrackingService(
            windowControl.Object,
            new TestCursorPositionProvider(10, 10),
            NullLogger<OverlayTrackingService>.Instance);

        var scaledStateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.TrackingStateChanged += (_, state) =>
        {
            if (state.Width >= 700)
            {
                scaledStateTcs.TrySetResult(state);
            }
        };

        await service.StartAsync(
            targetWindow: (nint)11,
            new OverlayTrackingRequest(
                BaseWidth: 400,
                BaseHeight: 40,
                VisibilityMode: OverlayVisibilityMode.Always,
                ScaleWithWindow: true));

        var state = await scaledStateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(800, state.Width, precision: 0);
        Assert.Equal(80, state.Height, precision: 0);
    }

    [Fact]
    public async Task StartAsync_WithNonUniformScale_EmitsIndependentWidthAndHeightScale()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .SetupSequence(x => x.GetWindowBoundsAsync((nint)14, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(50, 100, 800, 600))
            .ReturnsAsync(new WindowBounds(50, 100, 400, 1200))
            .ReturnsAsync(new WindowBounds(50, 100, 400, 1200));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)14, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 2560, 1440));

        await using var service = new OverlayTrackingService(
            windowControl.Object,
            new TestCursorPositionProvider(10, 10),
            NullLogger<OverlayTrackingService>.Instance);

        var scaledStateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.TrackingStateChanged += (_, state) =>
        {
            if (state.Width <= 220 && state.Height >= 75)
            {
                scaledStateTcs.TrySetResult(state);
            }
        };

        await service.StartAsync(
            targetWindow: (nint)14,
            new OverlayTrackingRequest(
                BaseWidth: 400,
                BaseHeight: 40,
                VisibilityMode: OverlayVisibilityMode.Always,
                ScaleWithWindow: true));

        var state = await scaledStateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(200, state.Width, precision: 0);
        Assert.Equal(80, state.Height, precision: 0);
    }

    [Fact]
    public async Task StartAsync_WithScaleWithWindow_RespectsMinimumDimensions()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .SetupSequence(x => x.GetWindowBoundsAsync((nint)15, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 200, 1000, 1000))
            .ReturnsAsync(new WindowBounds(100, 200, 50, 50))
            .ReturnsAsync(new WindowBounds(100, 200, 50, 50));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)15, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        await using var service = new OverlayTrackingService(
            windowControl.Object,
            new TestCursorPositionProvider(10, 10),
            NullLogger<OverlayTrackingService>.Instance);

        var stateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.TrackingStateChanged += (_, state) =>
        {
            if (state.Width <= 130 && state.Height <= 40)
            {
                stateTcs.TrySetResult(state);
            }
        };

        await service.StartAsync(
            targetWindow: (nint)15,
            new OverlayTrackingRequest(
                BaseWidth: 400,
                BaseHeight: 40,
                VisibilityMode: OverlayVisibilityMode.Always,
                ScaleWithWindow: true));

        var state = await stateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(120, state.Width, precision: 0);
        Assert.Equal(32, state.Height, precision: 0);
    }

    [Fact]
    public async Task StartAsync_WithTopRightAnchor_PlacesOverlayOnWindowRightEdge()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .Setup(x => x.GetWindowBoundsAsync((nint)16, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 200, 800, 600));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)16, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        await using var service = new OverlayTrackingService(
            windowControl.Object,
            new TestCursorPositionProvider(10, 10),
            NullLogger<OverlayTrackingService>.Instance);

        var stateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.TrackingStateChanged += (_, state) =>
        {
            if (state.IsVisible)
            {
                stateTcs.TrySetResult(state);
            }
        };

        await service.StartAsync(
            targetWindow: (nint)16,
            new OverlayTrackingRequest(
                BaseWidth: 200,
                BaseHeight: 50,
                VisibilityMode: OverlayVisibilityMode.Always,
                ScaleWithWindow: false,
                Anchor: OverlayAnchor.TopRight));

        var state = await stateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(692, state.Left, precision: 0);
        Assert.Equal(208, state.Top, precision: 0);
    }

    [Fact]
    public async Task StartAsync_WithBottomRightAnchor_ClampsToMonitorBounds()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .Setup(x => x.GetWindowBoundsAsync((nint)17, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(1100, 600, 400, 300));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)17, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1280, 720));

        await using var service = new OverlayTrackingService(
            windowControl.Object,
            new TestCursorPositionProvider(10, 10),
            NullLogger<OverlayTrackingService>.Instance);

        var stateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.TrackingStateChanged += (_, state) =>
        {
            if (state.IsVisible)
            {
                stateTcs.TrySetResult(state);
            }
        };

        await service.StartAsync(
            targetWindow: (nint)17,
            new OverlayTrackingRequest(
                BaseWidth: 240,
                BaseHeight: 64,
                VisibilityMode: OverlayVisibilityMode.Always,
                ScaleWithWindow: false,
                Anchor: OverlayAnchor.BottomRight));

        var state = await stateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(1032, state.Left, precision: 0);
        Assert.Equal(648, state.Top, precision: 0);
    }

    [Fact]
    public async Task StartAsync_OnHover_TracksCursorPresence()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .Setup(x => x.GetWindowBoundsAsync((nint)12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 200, 800, 600));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var cursorProvider = new TestCursorPositionProvider(10, 10);
        await using var service = new OverlayTrackingService(
            windowControl.Object,
            cursorProvider,
            NullLogger<OverlayTrackingService>.Instance);

        var hiddenStateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        var visibleStateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);

        service.TrackingStateChanged += (_, state) =>
        {
            if (!state.IsVisible)
            {
                hiddenStateTcs.TrySetResult(state);
                return;
            }

            visibleStateTcs.TrySetResult(state);
        };

        await service.StartAsync(
            targetWindow: (nint)12,
            new OverlayTrackingRequest(
                BaseWidth: 560,
                BaseHeight: 56,
                VisibilityMode: OverlayVisibilityMode.OnHover,
                ScaleWithWindow: false));

        var hidden = await hiddenStateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.False(hidden.IsVisible);

        cursorProvider.SetPosition(300, 400);
        var visible = await visibleStateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.True(visible.IsVisible);
    }

    [Fact]
    public async Task StartAsync_HideOnHover_HidesWhenCursorInside()
    {
        var windowControl = new Mock<IWindowControlService>(MockBehavior.Strict);
        windowControl
            .Setup(x => x.GetWindowBoundsAsync((nint)13, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 200, 800, 600));
        windowControl
            .Setup(x => x.GetMonitorBoundsForWindowAsync((nint)13, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var cursorProvider = new TestCursorPositionProvider(300, 400);
        await using var service = new OverlayTrackingService(
            windowControl.Object,
            cursorProvider,
            NullLogger<OverlayTrackingService>.Instance);

        var hiddenStateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);
        var visibleStateTcs = new TaskCompletionSource<OverlayTrackingState>(TaskCreationOptions.RunContinuationsAsynchronously);

        service.TrackingStateChanged += (_, state) =>
        {
            if (!state.IsVisible)
            {
                hiddenStateTcs.TrySetResult(state);
                return;
            }

            visibleStateTcs.TrySetResult(state);
        };

        await service.StartAsync(
            targetWindow: (nint)13,
            new OverlayTrackingRequest(
                BaseWidth: 560,
                BaseHeight: 56,
                VisibilityMode: OverlayVisibilityMode.HideOnHover,
                ScaleWithWindow: false));

        var hidden = await hiddenStateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.False(hidden.IsVisible);

        cursorProvider.SetPosition(10, 10);
        var visible = await visibleStateTcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.True(visible.IsVisible);
    }

    private sealed class TestCursorPositionProvider(int x, int y) : ICursorPositionProvider
    {
        private readonly object _sync = new();
        private int _x = x;
        private int _y = y;

        public bool TryGetCursorPosition(out int x, out int y)
        {
            lock (_sync)
            {
                x = _x;
                y = _y;
                return true;
            }
        }

        public void SetPosition(int x, int y)
        {
            lock (_sync)
            {
                _x = x;
                _y = y;
            }
        }
    }
}
