namespace NxTiler.App.Models;

public sealed record WindowListItem(
    int Index,
    string Name,
    string Title,
    string State,
    string HandleHex,
    nint Handle,
    uint ProcessId);
