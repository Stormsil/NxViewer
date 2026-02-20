namespace NxTiler.Domain.Windowing;

public sealed record WindowQueryOptions(
    int SelfProcessId,
    string? TitleFilter,
    string? SessionNameFilter,
    bool SortDescending,
    IReadOnlyCollection<string>? PreferNames = null
);
