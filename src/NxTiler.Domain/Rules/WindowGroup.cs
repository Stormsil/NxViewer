using NxTiler.Domain.Enums;

namespace NxTiler.Domain.Rules;

public sealed record WindowGroup(
    string Id,
    string Name,
    TileMode TileMode,
    bool AutoArrange);
