using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public enum RecordingWorkflowAction
{
    StartMaskEditing = 0,
    StartRecording = 1,
    Pause = 2,
    Resume = 3,
    StopSaveBegin = 4,
    CompleteSaving = 5,
    StopDiscard = 6,
    Cancel = 7,
}

public sealed class RecordingWorkflowStateMachine
{
    private static readonly IReadOnlyDictionary<RecordingWorkflowAction, TransitionRule> Rules =
        new Dictionary<RecordingWorkflowAction, TransitionRule>
        {
            [RecordingWorkflowAction.StartMaskEditing] = new(
                Target: RecordingState.MaskEditing,
                Allowed: [RecordingState.Idle]),
            [RecordingWorkflowAction.StartRecording] = new(
                Target: RecordingState.Recording,
                Allowed: [RecordingState.MaskEditing]),
            [RecordingWorkflowAction.Pause] = new(
                Target: RecordingState.Paused,
                Allowed: [RecordingState.Recording]),
            [RecordingWorkflowAction.Resume] = new(
                Target: RecordingState.Recording,
                Allowed: [RecordingState.Paused]),
            [RecordingWorkflowAction.StopSaveBegin] = new(
                Target: RecordingState.Saving,
                Allowed: [RecordingState.Recording, RecordingState.Paused]),
            [RecordingWorkflowAction.CompleteSaving] = new(
                Target: RecordingState.Idle,
                Allowed: [RecordingState.Saving]),
            [RecordingWorkflowAction.StopDiscard] = new(
                Target: RecordingState.Idle,
                Allowed: [RecordingState.Recording, RecordingState.Paused]),
            [RecordingWorkflowAction.Cancel] = new(
                Target: RecordingState.Idle,
                Allowed: [RecordingState.MaskEditing, RecordingState.Recording, RecordingState.Paused, RecordingState.Saving]),
        };

    public RecordingState State { get; private set; } = RecordingState.Idle;

    public bool CanExecute(RecordingWorkflowAction action)
    {
        return GetRule(action).Allowed.Contains(State);
    }

    public IReadOnlyList<RecordingState> GetAllowedStates(RecordingWorkflowAction action)
    {
        return GetRule(action).Allowed;
    }

    public bool TryTransition(RecordingWorkflowAction action, out RecordingState previous, out RecordingState next)
    {
        previous = State;
        var rule = GetRule(action);
        if (!rule.Allowed.Contains(State))
        {
            next = State;
            return false;
        }

        State = rule.Target;
        next = State;
        return true;
    }

    private static TransitionRule GetRule(RecordingWorkflowAction action)
    {
        if (!Rules.TryGetValue(action, out var rule))
        {
            throw new ArgumentOutOfRangeException(nameof(action), action, "Unknown recording workflow action.");
        }

        return rule;
    }

    private sealed record TransitionRule(
        RecordingState Target,
        IReadOnlyList<RecordingState> Allowed
    );
}
