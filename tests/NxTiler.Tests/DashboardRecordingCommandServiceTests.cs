using Moq;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Recording;

namespace NxTiler.Tests;

public sealed class DashboardRecordingCommandServiceTests
{
    [Fact]
    public async Task StartMaskEditingAsync_DelegatesToRecordingWorkflowService()
    {
        var workflow = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        workflow
            .Setup(x => x.StartMaskEditingAsync((nint)123, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new DashboardRecordingCommandService(workflow.Object);

        await service.StartMaskEditingAsync((nint)123, CancellationToken.None);

        workflow.Verify(x => x.StartMaskEditingAsync((nint)123, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StopAsync_DelegatesSaveFlagToRecordingWorkflowService()
    {
        var result = new RecordingResult(
            Saved: true,
            OutputPath: @"C:\\capture\\video.mp4",
            Message: "saved");

        var workflow = new Mock<IRecordingWorkflowService>(MockBehavior.Strict);
        workflow
            .Setup(x => x.StopAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        var service = new DashboardRecordingCommandService(workflow.Object);

        var actual = await service.StopAsync(save: true, CancellationToken.None);

        Assert.Equal(result, actual);
        workflow.Verify(x => x.StopAsync(true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
