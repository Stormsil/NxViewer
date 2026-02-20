namespace NxTiler.Domain.Settings;

public sealed record FiltersSettings(
    string TitleFilter,
    string NameFilter,
    bool SortDescending
);
