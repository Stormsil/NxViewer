using Moq;
using NxTiler.App.Services;
using NxTiler.Domain.Enums;

namespace NxTiler.Tests;

public sealed class DashboardWorkspaceCommandServiceTests
{
    [Fact]
    public async Task SelectWindowAsync_DelegatesToWorkspaceOrchestrator()
    {
        var orchestrator = new Mock<IWorkspaceOrchestrator>(MockBehavior.Strict);
        orchestrator
            .Setup(x => x.SelectWindowAsync(3, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new DashboardWorkspaceCommandService(orchestrator.Object);

        await service.SelectWindowAsync(3, CancellationToken.None);

        orchestrator.Verify(x => x.SelectWindowAsync(3, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetModeAsync_DelegatesToWorkspaceOrchestrator()
    {
        var orchestrator = new Mock<IWorkspaceOrchestrator>(MockBehavior.Strict);
        orchestrator
            .Setup(x => x.SetModeAsync(TileMode.Focus, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new DashboardWorkspaceCommandService(orchestrator.Object);

        await service.SetModeAsync(TileMode.Focus, CancellationToken.None);

        orchestrator.Verify(x => x.SetModeAsync(TileMode.Focus, It.IsAny<CancellationToken>()), Times.Once);
    }
}
