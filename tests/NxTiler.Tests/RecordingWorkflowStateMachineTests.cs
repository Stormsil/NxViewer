using NxTiler.App.Services;
using NxTiler.Domain.Enums;

namespace NxTiler.Tests;

public sealed class RecordingWorkflowStateMachineTests
{
    [Fact]
    public void TransitionTable_AllowsHappyPath()
    {
        var machine = new RecordingWorkflowStateMachine();

        Assert.Equal(RecordingState.Idle, machine.State);
        Assert.True(machine.TryTransition(RecordingWorkflowAction.StartMaskEditing, out _, out _));
        Assert.Equal(RecordingState.MaskEditing, machine.State);

        Assert.True(machine.TryTransition(RecordingWorkflowAction.StartRecording, out _, out _));
        Assert.Equal(RecordingState.Recording, machine.State);

        Assert.True(machine.TryTransition(RecordingWorkflowAction.Pause, out _, out _));
        Assert.Equal(RecordingState.Paused, machine.State);

        Assert.True(machine.TryTransition(RecordingWorkflowAction.Resume, out _, out _));
        Assert.Equal(RecordingState.Recording, machine.State);

        Assert.True(machine.TryTransition(RecordingWorkflowAction.StopSaveBegin, out _, out _));
        Assert.Equal(RecordingState.Saving, machine.State);

        Assert.True(machine.TryTransition(RecordingWorkflowAction.CompleteSaving, out _, out _));
        Assert.Equal(RecordingState.Idle, machine.State);
    }

    [Fact]
    public void TransitionTable_RejectsInvalidTransition_AndKeepsState()
    {
        var machine = new RecordingWorkflowStateMachine();

        var transitioned = machine.TryTransition(RecordingWorkflowAction.Pause, out var previous, out var next);

        Assert.False(transitioned);
        Assert.Equal(RecordingState.Idle, previous);
        Assert.Equal(RecordingState.Idle, next);
        Assert.Equal(RecordingState.Idle, machine.State);
    }

    [Fact]
    public void CanExecute_ReturnsExpectedAllowedStates()
    {
        var machine = new RecordingWorkflowStateMachine();
        var allowed = machine.GetAllowedStates(RecordingWorkflowAction.StopSaveBegin);

        Assert.Equal([RecordingState.Recording, RecordingState.Paused], allowed);
        Assert.False(machine.CanExecute(RecordingWorkflowAction.StopSaveBegin));

        Assert.True(machine.TryTransition(RecordingWorkflowAction.StartMaskEditing, out _, out _));
        Assert.True(machine.TryTransition(RecordingWorkflowAction.StartRecording, out _, out _));

        Assert.True(machine.CanExecute(RecordingWorkflowAction.StopSaveBegin));
    }
}
