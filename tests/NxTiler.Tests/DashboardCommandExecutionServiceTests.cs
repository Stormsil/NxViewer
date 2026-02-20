using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NxTiler.App.Services;

namespace NxTiler.Tests;

public sealed class DashboardCommandExecutionServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCanStartFalse_DoesNotExecuteAction()
    {
        var feedback = new Mock<IUserFeedbackService>(MockBehavior.Strict);
        var service = new DashboardCommandExecutionService(
            feedback.Object,
            NullLogger<DashboardCommandExecutionService>.Instance);

        var wasExecuted = false;
        var busyTransitions = new List<bool>();
        var statusMessages = new List<string>();

        await service.ExecuteAsync(
            _ =>
            {
                wasExecuted = true;
                return Task.CompletedTask;
            },
            canStart: () => false,
            setBusy: busyTransitions.Add,
            setStatus: statusMessages.Add);

        Assert.False(wasExecuted);
        Assert.Empty(busyTransitions);
        Assert.Empty(statusMessages);
        feedback.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenActionSucceeds_TogglesBusy()
    {
        var feedback = new Mock<IUserFeedbackService>(MockBehavior.Strict);
        var service = new DashboardCommandExecutionService(
            feedback.Object,
            NullLogger<DashboardCommandExecutionService>.Instance);

        var busyTransitions = new List<bool>();
        var statusMessages = new List<string>();

        await service.ExecuteAsync(
            _ => Task.CompletedTask,
            canStart: () => true,
            setBusy: busyTransitions.Add,
            setStatus: statusMessages.Add);

        Assert.Equal(new[] { true, false }, busyTransitions);
        Assert.Empty(statusMessages);
        feedback.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenActionFails_ReportsErrorAndStatus()
    {
        var feedback = new Mock<IUserFeedbackService>(MockBehavior.Strict);
        feedback
            .Setup(x => x.Error("Command failed", "boom"));

        var service = new DashboardCommandExecutionService(
            feedback.Object,
            NullLogger<DashboardCommandExecutionService>.Instance);

        var busyTransitions = new List<bool>();
        string? status = null;

        await service.ExecuteAsync(
            _ => throw new InvalidOperationException("boom"),
            canStart: () => true,
            setBusy: busyTransitions.Add,
            setStatus: message => status = message);

        Assert.Equal("boom", status);
        Assert.Equal(new[] { true, false }, busyTransitions);
        feedback.Verify(x => x.Error("Command failed", "boom"), Times.Once);
        feedback.VerifyNoOtherCalls();
    }
}
