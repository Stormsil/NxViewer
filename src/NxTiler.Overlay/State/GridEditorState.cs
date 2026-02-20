namespace NxTiler.Overlay.State;

public sealed record GridEditorState(
    int Cols,
    int Rows,
    int? DragStartCol,
    int? DragStartRow,
    int? DragCurrentCol,
    int? DragCurrentRow,
    bool IsSaving)
{
    public static readonly GridEditorState Default = new(6, 4, null, null, null, null, false);
}
