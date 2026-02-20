using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using NxTiler.App.Messages;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private bool CanExecute(RecordingWorkflowAction action, string operation)
    {
        if (_stateMachine.CanExecute(action))
        {
            return true;
        }

        LogSkippedTransition(operation, _stateMachine.GetAllowedStates(action));
        return false;
    }

    private void ApplyTransition(RecordingWorkflowAction action, string? message = null)
    {
        if (!_stateMachine.TryTransition(action, out _, out var next))
        {
            throw new InvalidOperationException($"Invalid recording transition for action {action} from state {State}.");
        }

        _messenger.Send(new RecordingStateChangedMessage(next));
        if (!string.IsNullOrWhiteSpace(message))
        {
            RaiseMessage(message);
        }
    }

    private void RaiseMessage(string message)
    {
        _logger.LogInformation("Recording workflow: {Message}", message);
        _messenger.Send(new RecordingMessageRaisedMessage(message));
    }

    private void LogSkippedTransition(string operation, IReadOnlyList<RecordingState> requiredStates)
    {
        var required = string.Join(", ", requiredStates);
        _logger.LogDebug(
            "Recording workflow skipped operation {Operation}. Required state(s): {RequiredState}, current state: {CurrentState}",
            operation,
            required,
            State);
    }
}
