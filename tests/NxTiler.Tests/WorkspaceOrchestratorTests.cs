using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NxTiler.App.Messages;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;
using NxTiler.Application.Messaging;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Settings;
using NxTiler.Domain.Windowing;

namespace NxTiler.Tests;

public sealed class WorkspaceOrchestratorTests
{
    [Fact]
    public async Task ArrangeEvent_DoesNotArrange_WhenAutoArrangeDisabled()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: false),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);
        var messenger = new WeakReferenceMessenger();

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new WindowArrangeNeededMessage());

        await Task.Delay(50);

        arrangement.Verify(
            x => x.BuildPlacements(
                It.IsAny<IReadOnlyList<TargetWindowInfo>>(),
                It.IsAny<TileMode>(),
                It.IsAny<ArrangementContext>()),
            Times.Never);
    }

    [Fact]
    public async Task ArrangeEvent_Arranges_WhenAutoArrangeEnabled()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arranged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);
        arrangement.Setup(x => x.BuildPlacements(It.IsAny<IReadOnlyList<TargetWindowInfo>>(), It.IsAny<TileMode>(), It.IsAny<ArrangementContext>()))
            .Callback(() => arranged.TrySetResult())
            .Returns([new WindowPlacement((nint)1, 0, 0, 1920, 1080)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);
        var messenger = new WeakReferenceMessenger();

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new WindowArrangeNeededMessage());

        await arranged.Task.WaitAsync(TimeSpan.FromSeconds(2));
        arrangement.Verify(
            x => x.BuildPlacements(
                It.IsAny<IReadOnlyList<TargetWindowInfo>>(),
                It.IsAny<TileMode>(),
                It.IsAny<ArrangementContext>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task SetAutoArrangeAsync_DoesNotPersist_WhenStateUnchanged()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance);

        await orchestrator.StartAsync();
        await orchestrator.SetAutoArrangeAsync(enabled: true);

        Assert.Equal(0, settingsService.SaveCalls);
        tray.Verify(x => x.SetAutoArrangeState(true), Times.Once);
    }

    [Fact]
    public async Task ToggleOverlayHotkey_PublishesOverlayMessageOnly()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();
        var sink = new WorkspaceMessagesSink();
        messenger.Register<WorkspaceMessagesSink, OverlayToggleRequestedMessage>(sink, static (recipient, _) =>
        {
            recipient.OverlayToggleCount++;
        });
        messenger.Register<WorkspaceMessagesSink, MainWindowToggleRequestedMessage>(sink, static (recipient, _) =>
        {
            recipient.MainToggleCount++;
        });

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new HotkeyActionPressedMessage(HotkeyAction.ToggleOverlay));

        Assert.Equal(1, sink.OverlayToggleCount);
        Assert.Equal(0, sink.MainToggleCount);
    }

    [Fact]
    public async Task ToggleMainWindowHotkey_PublishesMainWindowMessage()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();
        var sink = new WorkspaceMessagesSink();
        messenger.Register<WorkspaceMessagesSink, MainWindowToggleRequestedMessage>(sink, static (recipient, _) =>
        {
            recipient.MainToggleCount++;
        });

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new HotkeyActionPressedMessage(HotkeyAction.ToggleMainWindow));

        Assert.Equal(1, sink.MainToggleCount);

        orchestrator.RequestMainWindowToggle();
        Assert.Equal(2, sink.MainToggleCount);
    }

    [Fact]
    public async Task ArrangeEvent_QueriesWindowsOnlyOncePerCycle()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arranged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);
        arrangement.Setup(x => x.BuildPlacements(It.IsAny<IReadOnlyList<TargetWindowInfo>>(), It.IsAny<TileMode>(), It.IsAny<ArrangementContext>()))
            .Callback(() => arranged.TrySetResult())
            .Returns([new WindowPlacement((nint)1, 0, 0, 1920, 1080)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);
        var messenger = new WeakReferenceMessenger();

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        query.Invocations.Clear();

        messenger.Send(new WindowArrangeNeededMessage());

        await arranged.Task.WaitAsync(TimeSpan.FromSeconds(2));
        query.Verify(
            x => x.QueryAsync(
                It.IsAny<WindowQueryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Navigate_QueriesWindowsOnlyOnce()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1), CreateWindow(2)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);
        arrangement.Setup(x => x.BuildPlacements(It.IsAny<IReadOnlyList<TargetWindowInfo>>(), It.IsAny<TileMode>(), It.IsAny<ArrangementContext>()))
            .Returns([new WindowPlacement((nint)1, 0, 0, 960, 1080), new WindowPlacement((nint)2, 960, 0, 960, 1080)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance);

        await orchestrator.StartAsync();
        query.Invocations.Clear();

        await orchestrator.NavigateAsync(1);

        query.Verify(
            x => x.QueryAsync(
                It.IsAny<WindowQueryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StartAsync_PublishesSnapshotAndStatusMessages()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();
        var sink = new WorkspaceMessagesSink();
        messenger.Register<WorkspaceMessagesSink, WorkspaceSnapshotChangedMessage>(sink, static (recipient, _) =>
        {
            recipient.SnapshotCount++;
        });
        messenger.Register<WorkspaceMessagesSink, WorkspaceStatusChangedMessage>(sink, static (recipient, _) =>
        {
            recipient.StatusCount++;
        });

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();

        Assert.True(sink.SnapshotCount > 0);
        Assert.True(sink.StatusCount > 0);
    }

    [Fact]
    public async Task TrayShowMessage_PublishesMainWindowToggleMessage()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();
        var sink = new WorkspaceMessagesSink();
        messenger.Register<WorkspaceMessagesSink, MainWindowToggleRequestedMessage>(sink, static (recipient, _) =>
        {
            recipient.MainToggleCount++;
        });

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new TrayShowRequestedMessage());

        Assert.Equal(1, sink.MainToggleCount);
    }

    [Fact]
    public async Task TrayArrangeMessage_TriggersArrange()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arranged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);
        arrangement.Setup(x => x.BuildPlacements(It.IsAny<IReadOnlyList<TargetWindowInfo>>(), It.IsAny<TileMode>(), It.IsAny<ArrangementContext>()))
            .Callback(() => arranged.TrySetResult())
            .Returns([new WindowPlacement((nint)1, 0, 0, 1920, 1080)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new TrayArrangeRequestedMessage());

        await arranged.Task.WaitAsync(TimeSpan.FromSeconds(2));
        arrangement.Verify(
            x => x.BuildPlacements(
                It.IsAny<IReadOnlyList<TargetWindowInfo>>(),
                It.IsAny<TileMode>(),
                It.IsAny<ArrangementContext>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task TrayAutoArrangeToggleMessage_PersistsUpdatedFlag()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: false),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new TrayAutoArrangeToggledMessage(true));

        await WaitForAsync(() => settingsService.Current.Ui.AutoArrangeEnabled);

        Assert.True(settingsService.Current.Ui.AutoArrangeEnabled);
        Assert.Equal(1, settingsService.SaveCalls);
        tray.Verify(x => x.SetAutoArrangeState(true), Times.Once);
    }

    [Fact]
    public async Task ForegroundChangedMessage_UpdatesForeignAppFlag()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new WindowForegroundChangedMessage((nint)999));
        await orchestrator.RefreshAsync();
        Assert.True(orchestrator.Snapshot.IsForeignAppActive);

        messenger.Send(new WindowForegroundChangedMessage((nint)1));
        await orchestrator.RefreshAsync();
        Assert.False(orchestrator.Snapshot.IsForeignAppActive);
    }

    [Fact]
    public async Task TrayExitMessage_PublishesApplicationExitRequestedMessage()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();
        var sink = new WorkspaceMessagesSink();
        messenger.Register<WorkspaceMessagesSink, ApplicationExitRequestedMessage>(sink, static (recipient, _) =>
        {
            recipient.ApplicationExitCount++;
        });

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        messenger.Send(new TrayExitRequestedMessage());

        Assert.Equal(1, sink.ApplicationExitCount);
    }

    [Fact]
    public async Task DisposeAsync_IsIdempotent_AndStopsServicesOnlyOnce()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance);

        await orchestrator.StartAsync();
        await orchestrator.DisposeAsync();
        await orchestrator.DisposeAsync();

        hotkeys.Verify(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        hotkeys.Verify(x => x.Dispose(), Times.Once);
        monitor.Verify(x => x.StopAsync(It.IsAny<CancellationToken>()), Times.Once);
        monitor.Verify(x => x.Dispose(), Times.Once);
        tray.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public async Task StartAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);
        var control = new Mock<IWindowControlService>(MockBehavior.Strict);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);

        var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance);

        await orchestrator.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => orchestrator.StartAsync());
    }

    [Fact]
    public async Task Messages_AfterDispose_AreIgnored()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();

        var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await orchestrator.StartAsync();
        await orchestrator.DisposeAsync();
        messenger.Send(new TrayArrangeRequestedMessage());

        await Task.Delay(50);
        arrangement.Verify(
            x => x.BuildPlacements(
                It.IsAny<IReadOnlyList<TargetWindowInfo>>(),
                It.IsAny<TileMode>(),
                It.IsAny<ArrangementContext>()),
            Times.Never);
    }

    [Fact]
    public async Task StartAsync_CalledTwice_InitializesDependenciesOnlyOnce()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: true),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance);

        await orchestrator.StartAsync();
        await orchestrator.StartAsync();

        tray.Verify(x => x.Initialize(It.IsAny<bool>()), Times.Once);
        hotkeys.Verify(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()), Times.Once);
        monitor.Verify(x => x.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        query.Verify(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetAutoArrangeAsync_WhenEnabledStartsMonitor_AndWhenDisabledStopsMonitor()
    {
        var settingsService = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                Ui = new UiSettings(TrayHintShown: false, AutoArrangeEnabled: false),
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);
        arrangement.Setup(x => x.BuildPlacements(It.IsAny<IReadOnlyList<TargetWindowInfo>>(), It.IsAny<TileMode>(), It.IsAny<ArrangementContext>()))
            .Returns([new WindowPlacement((nint)1, 0, 0, 1920, 1080)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.Setup(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance);

        await orchestrator.StartAsync();
        monitor.Verify(x => x.StartAsync(It.IsAny<CancellationToken>()), Times.Never);
        await orchestrator.SetAutoArrangeAsync(true);
        await orchestrator.SetAutoArrangeAsync(false);

        monitor.Verify(x => x.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        monitor.Verify(x => x.StopAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(2, settingsService.SaveCalls);
    }

    [Fact]
    public async Task StartAsync_RetryAfterFailure_DoesNotDuplicateMessageHandlers()
    {
        var settingsService = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var arrangement = new Mock<IArrangementService>(MockBehavior.Strict);
        arrangement.Setup(x => x.BuildPlacements(It.IsAny<IReadOnlyList<TargetWindowInfo>>(), It.IsAny<TileMode>(), It.IsAny<ArrangementContext>()))
            .Returns([new WindowPlacement((nint)1, 0, 0, 1920, 1080)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetWorkAreaForWindowAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));
        control.Setup(x => x.ApplyPlacementsAsync(It.IsAny<IReadOnlyList<WindowPlacement>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.MinimizeAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.RestoreAllAsync(It.IsAny<IReadOnlyList<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        control.Setup(x => x.BringToForegroundAsync(It.IsAny<nint>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var monitor = new Mock<IWindowEventMonitorService>(MockBehavior.Strict);
        monitor.Setup(x => x.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        monitor.Setup(x => x.UpdateTrackedWindowsAsync(It.IsAny<IReadOnlyCollection<nint>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        monitor.Setup(x => x.Dispose());

        var sessions = new Mock<INomachineSessionService>(MockBehavior.Strict);
        sessions.Setup(x => x.FindSessionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        sessions.Setup(x => x.OpenIfNeededAsync(It.IsAny<IEnumerable<SessionFileInfo>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sessions.Setup(x => x.LaunchSessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var hotkeys = new Mock<IHotkeyService>(MockBehavior.Strict);
        hotkeys.SetupSequence(x => x.RegisterAllAsync(It.IsAny<HotkeysSettings>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Register failed"))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.UnregisterAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        hotkeys.Setup(x => x.Dispose());

        var tray = new Mock<ITrayService>(MockBehavior.Strict);
        tray.Setup(x => x.Initialize(It.IsAny<bool>()));
        tray.Setup(x => x.SetAutoArrangeState(It.IsAny<bool>()));
        tray.Setup(x => x.Dispose());

        var recording = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        recording.SetupGet(x => x.State).Returns(RecordingState.Idle);

        var messenger = new WeakReferenceMessenger();

        await using var orchestrator = new WorkspaceOrchestrator(
            query.Object,
            arrangement.Object,
            control.Object,
            monitor.Object,
            sessions.Object,
            settingsService,
            hotkeys.Object,
            tray.Object,
            recording.Object,
            NullLogger<WorkspaceOrchestrator>.Instance,
            messenger);

        await Assert.ThrowsAsync<InvalidOperationException>(() => orchestrator.StartAsync());
        await orchestrator.StartAsync();

        messenger.Send(new TrayArrangeRequestedMessage());
        await Task.Delay(50);

        arrangement.Verify(
            x => x.BuildPlacements(
                It.IsAny<IReadOnlyList<TargetWindowInfo>>(),
                It.IsAny<TileMode>(),
                It.IsAny<ArrangementContext>()),
            Times.Once);
        query.Verify(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    private static TargetWindowInfo CreateWindow(int id)
    {
        return new TargetWindowInfo(
            Handle: (nint)id,
            Title: $"NoMachine - {id}",
            SourceName: $"WoW{id}",
            IsMaximized: false,
            ProcessId: 1000u + (uint)id);
    }

    private sealed class WorkspaceMessagesSink
    {
        public int SnapshotCount { get; set; }

        public int StatusCount { get; set; }

        public int MainToggleCount { get; set; }

        public int OverlayToggleCount { get; set; }

        public int ApplicationExitCount { get; set; }
    }

    private sealed class FakeSettingsService(AppSettingsSnapshot snapshot) : ISettingsService
    {
        public AppSettingsSnapshot Current { get; private set; } = snapshot;

        public int SaveCalls { get; private set; }

        public void Update(AppSettingsSnapshot updated)
        {
            Current = updated;
        }

        public Task SaveAsync(CancellationToken ct = default)
        {
            SaveCalls++;
            return Task.CompletedTask;
        }

        public Task ReloadAsync(CancellationToken ct = default) => Task.CompletedTask;
    }

    private static async Task WaitForAsync(Func<bool> condition)
    {
        var timeout = TimeSpan.FromSeconds(2);
        var start = DateTime.UtcNow;

        while (!condition())
        {
            if (DateTime.UtcNow - start > timeout)
            {
                throw new TimeoutException($"Condition was not met within {timeout}.");
            }

            await Task.Delay(20);
        }
    }
}
