using NxTiler.Domain.Rules;

namespace NxTiler.Domain.Settings;

public sealed record WindowGroupsSettings(
    IReadOnlyList<WindowGroup> Groups);
