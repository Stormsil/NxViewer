using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NxTiler.App.Messages;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Settings;
using NxTiler.Domain.Windowing;

namespace NxTiler.Tests;

public sealed class RecordingWorkflowServiceTests
{
    [Fact]
    public async Task Workflow_TransitionsThroughExpectedStates_OnSaveFlow()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(1)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetClientAreaScreenBoundsAsync((nint)1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(10, 20, 1280, 720));
        control.Setup(x => x.GetMonitorBoundsForWindowAsync((nint)1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        ffmpeg.Setup(x => x.ResolveAndSaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync("C:\\ffmpeg\\ffmpeg.exe");
        ffmpeg.Setup(x => x.FindFfmpeg()).Returns("C:\\ffmpeg\\ffmpeg.exe");
        ffmpeg.Setup(x => x.DownloadAsync(It.IsAny<Action<double, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var engine = new Mock<IRecordingEngine>(MockBehavior.Strict);
        engine.Setup(x => x.Start(10, 20, 1280, 720, settings.Current.Recording.Fps, settings.Current.Paths.RecordingFolder, "C:\\ffmpeg\\ffmpeg.exe"))
            .Returns(true);
        engine.Setup(x => x.StopCurrentSegmentAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        engine.Setup(x => x.StartNewSegment()).Returns(true);
        engine.Setup(x => x.FinalizeRecordingAsync(It.IsAny<IReadOnlyList<WindowBounds>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("C:\\Videos\\capture.mp4");

        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);
        overlays.Setup(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterPauseEditModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.ReEnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.GetMaskRectsPxAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        overlays.Setup(x => x.CloseAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            engine.Object,
            NullLogger<RecordingWorkflowService>.Instance);

        await service.StartMaskEditingAsync();
        Assert.Equal(RecordingState.MaskEditing, service.State);

        await service.StartRecordingAsync();
        Assert.Equal(RecordingState.Recording, service.State);

        await service.PauseAsync();
        Assert.Equal(RecordingState.Paused, service.State);

        await service.ResumeAsync();
        Assert.Equal(RecordingState.Recording, service.State);

        var result = await service.StopAsync(save: true);

        Assert.Equal(RecordingState.Idle, service.State);
        Assert.True(result.Saved);
        Assert.Equal("C:\\Videos\\capture.mp4", result.OutputPath);
        engine.Verify(x => x.Start(10, 20, 1280, 720, settings.Current.Recording.Fps, settings.Current.Paths.RecordingFolder, "C:\\ffmpeg\\ffmpeg.exe"), Times.Once);
        engine.Verify(x => x.StopCurrentSegmentAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        engine.Verify(x => x.StartNewSegment(), Times.Once);
        engine.Verify(x => x.FinalizeRecordingAsync(It.IsAny<IReadOnlyList<WindowBounds>?>(), It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.EnterPauseEditModeAsync(It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.ReEnterRecordingModeAsync(It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.GetMaskRectsPxAsync(It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Workflow_StopDiscard_AbortsAndReturnsIdle()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(2)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetClientAreaScreenBoundsAsync((nint)2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 800, 600));
        control.Setup(x => x.GetMonitorBoundsForWindowAsync((nint)2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        ffmpeg.Setup(x => x.ResolveAndSaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ffmpeg");
        ffmpeg.Setup(x => x.FindFfmpeg()).Returns("ffmpeg");
        ffmpeg.Setup(x => x.DownloadAsync(It.IsAny<Action<double, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var engine = new Mock<IRecordingEngine>(MockBehavior.Strict);
        engine.Setup(x => x.Start(0, 0, 800, 600, settings.Current.Recording.Fps, settings.Current.Paths.RecordingFolder, "ffmpeg"))
            .Returns(true);
        engine.Setup(x => x.Abort());

        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);
        overlays.Setup(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterPauseEditModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.ReEnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.CloseAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            engine.Object,
            NullLogger<RecordingWorkflowService>.Instance);

        await service.StartMaskEditingAsync();
        await service.StartRecordingAsync();
        var result = await service.StopAsync(save: false);

        Assert.Equal(RecordingState.Idle, service.State);
        Assert.False(result.Saved);
        Assert.Null(result.OutputPath);
        engine.Verify(x => x.Abort(), Times.Once);
        overlays.Verify(x => x.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Workflow_StartMaskEditing_PublishesStateAndMessage()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(3)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetClientAreaScreenBoundsAsync((nint)3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 100, 1024, 640));
        control.Setup(x => x.GetMonitorBoundsForWindowAsync((nint)3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        ffmpeg.Setup(x => x.ResolveAndSaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ffmpeg");
        ffmpeg.Setup(x => x.FindFfmpeg()).Returns("ffmpeg");
        ffmpeg.Setup(x => x.DownloadAsync(It.IsAny<Action<double, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var engine = new Mock<IRecordingEngine>(MockBehavior.Strict);

        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);
        overlays.Setup(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterPauseEditModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.ReEnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.CloseAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var messenger = new WeakReferenceMessenger();
        var sink = new RecordingMessagesSink();
        messenger.Register<RecordingMessagesSink, RecordingStateChangedMessage>(sink, static (recipient, _) =>
        {
            recipient.StateCount++;
        });
        messenger.Register<RecordingMessagesSink, RecordingMessageRaisedMessage>(sink, static (recipient, _) =>
        {
            recipient.MessageCount++;
        });

        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            engine.Object,
            NullLogger<RecordingWorkflowService>.Instance,
            messenger);

        await service.StartMaskEditingAsync();

        Assert.True(sink.StateCount > 0);
        Assert.True(sink.MessageCount > 0);
    }

    [Fact]
    public async Task OverlayMessages_TriggerStartAndCancelFlow()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(4)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetClientAreaScreenBoundsAsync((nint)4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(50, 80, 900, 500));
        control.Setup(x => x.GetMonitorBoundsForWindowAsync((nint)4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        ffmpeg.Setup(x => x.ResolveAndSaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ffmpeg");
        ffmpeg.Setup(x => x.FindFfmpeg()).Returns("ffmpeg");
        ffmpeg.Setup(x => x.DownloadAsync(It.IsAny<Action<double, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var engine = new Mock<IRecordingEngine>(MockBehavior.Strict);
        engine.Setup(x => x.Start(50, 80, 900, 500, settings.Current.Recording.Fps, settings.Current.Paths.RecordingFolder, "ffmpeg"))
            .Returns(true);
        engine.Setup(x => x.Abort());

        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);
        overlays.Setup(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.CloseAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var messenger = new WeakReferenceMessenger();
        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            engine.Object,
            NullLogger<RecordingWorkflowService>.Instance,
            messenger);

        await service.StartMaskEditingAsync();
        Assert.Equal(RecordingState.MaskEditing, service.State);

        messenger.Send(new RecordingOverlayConfirmRequestedMessage());
        await WaitForStateAsync(service, RecordingState.Recording);

        messenger.Send(new RecordingOverlayCancelRequestedMessage());
        await WaitForStateAsync(service, RecordingState.Idle);

        engine.Verify(x => x.Start(50, 80, 900, 500, settings.Current.Recording.Fps, settings.Current.Paths.RecordingFolder, "ffmpeg"), Times.Once);
        engine.Verify(x => x.Abort(), Times.Once);
        overlays.Verify(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()), Times.Once);
        overlays.Verify(x => x.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidTransitions_DoNotChangeState_AndStopReturnsNotActive()
    {
        var settings = new FakeSettingsService(AppSettingsSnapshot.CreateDefault());

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        var engine = new Mock<IRecordingEngine>(MockBehavior.Strict);
        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);

        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            engine.Object,
            NullLogger<RecordingWorkflowService>.Instance);

        await service.PauseAsync();
        await service.ResumeAsync();
        await service.StartRecordingAsync();

        var stopResult = await service.StopAsync(save: true);

        Assert.Equal(RecordingState.Idle, service.State);
        Assert.False(stopResult.Saved);
        Assert.Equal("Recording is not active.", stopResult.Message);

        query.VerifyNoOtherCalls();
        control.VerifyNoOtherCalls();
        ffmpeg.VerifyNoOtherCalls();
        engine.VerifyNoOtherCalls();
        overlays.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Workflow_UsesVideoEngine_WhenFeatureFlagEnabled()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                FeatureFlags = AppSettingsSnapshot.CreateDefault().FeatureFlags with { UseWgcRecordingEngine = true },
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(9)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetClientAreaScreenBoundsAsync((nint)9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(40, 50, 900, 600));
        control.Setup(x => x.GetMonitorBoundsForWindowAsync((nint)9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        ffmpeg.Setup(x => x.ResolveAndSaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ffmpeg");
        ffmpeg.Setup(x => x.FindFfmpeg()).Returns("ffmpeg");
        ffmpeg.Setup(x => x.DownloadAsync(It.IsAny<Action<double, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var legacyEngine = new Mock<IRecordingEngine>(MockBehavior.Strict);

        var videoEngine = new Mock<IVideoRecordingEngine>(MockBehavior.Strict);
        videoEngine
            .Setup(x => x.StartAsync((nint)9, new WindowBounds(40, 50, 900, 600), It.IsAny<RecordingProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        videoEngine.Setup(x => x.PauseAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        videoEngine.Setup(x => x.ResumeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        videoEngine
            .Setup(x => x.StopAsync(
                It.Is<IReadOnlyList<CaptureMask>>(m =>
                    m.Count == 1
                    && m[0].X == 10
                    && m[0].Y == 10
                    && m[0].Width == 20
                    && m[0].Height == 20),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("C:\\Videos\\wgc.mp4");

        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);
        overlays.Setup(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterPauseEditModeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        overlays.Setup(x => x.ReEnterRecordingModeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        overlays.Setup(x => x.GetMaskRectsPxAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new WindowBounds(10, 10, 20, 20)]);
        overlays.Setup(x => x.CloseAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            legacyEngine.Object,
            NullLogger<RecordingWorkflowService>.Instance,
            null,
            videoEngine.Object);

        await service.StartMaskEditingAsync();
        await service.StartRecordingAsync();
        await service.PauseAsync();
        await service.ResumeAsync();
        var result = await service.StopAsync(save: true);

        Assert.True(result.Saved);
        Assert.Equal("C:\\Videos\\wgc.mp4", result.OutputPath);
        videoEngine.Verify(x => x.StartAsync((nint)9, new WindowBounds(40, 50, 900, 600), It.IsAny<RecordingProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        videoEngine.Verify(x => x.PauseAsync(It.IsAny<CancellationToken>()), Times.Once);
        videoEngine.Verify(x => x.ResumeAsync(It.IsAny<CancellationToken>()), Times.Once);
        videoEngine.Verify(x => x.StopAsync(It.IsAny<IReadOnlyList<CaptureMask>>(), It.IsAny<CancellationToken>()), Times.Once);
        legacyEngine.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Workflow_Discard_UsesVideoEngineAbort_WhenFeatureFlagEnabled()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                FeatureFlags = AppSettingsSnapshot.CreateDefault().FeatureFlags with { UseWgcRecordingEngine = true },
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(10)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetClientAreaScreenBoundsAsync((nint)10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(100, 120, 800, 600));
        control.Setup(x => x.GetMonitorBoundsForWindowAsync((nint)10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        ffmpeg.Setup(x => x.ResolveAndSaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ffmpeg");
        ffmpeg.Setup(x => x.FindFfmpeg()).Returns("ffmpeg");
        ffmpeg.Setup(x => x.DownloadAsync(It.IsAny<Action<double, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var legacyEngine = new Mock<IRecordingEngine>(MockBehavior.Strict);

        var videoEngine = new Mock<IVideoRecordingEngine>(MockBehavior.Strict);
        videoEngine
            .Setup(x => x.StartAsync((nint)10, new WindowBounds(100, 120, 800, 600), It.IsAny<RecordingProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        videoEngine.Setup(x => x.AbortAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);
        overlays.Setup(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.CloseAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            legacyEngine.Object,
            NullLogger<RecordingWorkflowService>.Instance,
            null,
            videoEngine.Object);

        await service.StartMaskEditingAsync();
        await service.StartRecordingAsync();
        var result = await service.StopAsync(save: false);

        Assert.False(result.Saved);
        Assert.Equal(RecordingState.Idle, service.State);
        videoEngine.Verify(x => x.StartAsync((nint)10, new WindowBounds(100, 120, 800, 600), It.IsAny<RecordingProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        videoEngine.Verify(x => x.AbortAsync(It.IsAny<CancellationToken>()), Times.Once);
        legacyEngine.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Workflow_FallsBackToLegacy_WhenVideoEngineStartFails()
    {
        var settings = new FakeSettingsService(
            AppSettingsSnapshot.CreateDefault() with
            {
                FeatureFlags = AppSettingsSnapshot.CreateDefault().FeatureFlags with { UseWgcRecordingEngine = true },
            });

        var query = new Mock<IWindowQueryService>(MockBehavior.Strict);
        query.Setup(x => x.QueryAsync(It.IsAny<WindowQueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateWindow(11)]);

        var control = new Mock<IWindowControlService>(MockBehavior.Strict);
        control.Setup(x => x.GetClientAreaScreenBoundsAsync((nint)11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(20, 30, 640, 480));
        control.Setup(x => x.GetMonitorBoundsForWindowAsync((nint)11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WindowBounds(0, 0, 1920, 1080));

        var ffmpeg = new Mock<IFfmpegSetupService>(MockBehavior.Strict);
        ffmpeg.Setup(x => x.ResolveAndSaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ffmpeg");
        ffmpeg.Setup(x => x.FindFfmpeg()).Returns("ffmpeg");
        ffmpeg.Setup(x => x.DownloadAsync(It.IsAny<Action<double, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var legacyEngine = new Mock<IRecordingEngine>(MockBehavior.Strict);
        legacyEngine
            .Setup(x => x.Start(20, 30, 640, 480, settings.Current.Recording.Fps, settings.Current.Paths.RecordingFolder, "ffmpeg"))
            .Returns(true);
        legacyEngine.Setup(x => x.Abort());

        var videoEngine = new Mock<IVideoRecordingEngine>(MockBehavior.Strict);
        videoEngine
            .Setup(x => x.StartAsync((nint)11, new WindowBounds(20, 30, 640, 480), It.IsAny<RecordingProfile>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("WGC unavailable"));

        var overlays = new Mock<IRecordingOverlayService>(MockBehavior.Strict);
        overlays.Setup(x => x.ShowMaskEditingAsync(It.IsAny<WindowBounds>(), It.IsAny<WindowBounds>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.EnterRecordingModeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        overlays.Setup(x => x.CloseAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RecordingWorkflowService(
            query.Object,
            control.Object,
            overlays.Object,
            settings,
            ffmpeg.Object,
            legacyEngine.Object,
            NullLogger<RecordingWorkflowService>.Instance,
            null,
            videoEngine.Object);

        await service.StartMaskEditingAsync();
        await service.StartRecordingAsync();
        Assert.Equal(RecordingState.Recording, service.State);

        var result = await service.StopAsync(save: false);

        Assert.False(result.Saved);
        Assert.Equal(RecordingState.Idle, service.State);
        videoEngine.Verify(x => x.StartAsync((nint)11, new WindowBounds(20, 30, 640, 480), It.IsAny<RecordingProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        legacyEngine.Verify(x => x.Start(20, 30, 640, 480, settings.Current.Recording.Fps, settings.Current.Paths.RecordingFolder, "ffmpeg"), Times.Once);
        legacyEngine.Verify(x => x.Abort(), Times.Once);
        videoEngine.VerifyNoOtherCalls();
    }

    private static TargetWindowInfo CreateWindow(int id)
    {
        return new TargetWindowInfo(
            Handle: (nint)id,
            Title: $"NoMachine - Session {id}",
            SourceName: $"WoW{id}",
            IsMaximized: false,
            ProcessId: 2000u + (uint)id);
    }

    private sealed class RecordingMessagesSink
    {
        public int StateCount { get; set; }

        public int MessageCount { get; set; }
    }

    private static async Task WaitForStateAsync(RecordingWorkflowService service, RecordingState expected)
    {
        var timeout = TimeSpan.FromSeconds(2);
        var start = DateTime.UtcNow;

        while (service.State != expected)
        {
            if (DateTime.UtcNow - start > timeout)
            {
                throw new TimeoutException($"Recording state did not reach {expected} within {timeout}.");
            }

            await Task.Delay(20);
        }
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
