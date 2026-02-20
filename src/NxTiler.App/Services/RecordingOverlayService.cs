using CommunityToolkit.Mvvm.Messaging;

namespace NxTiler.App.Services;

public sealed partial class RecordingOverlayService(
    IUiDispatcher uiDispatcher,
    IMessenger? messenger = null) : IRecordingOverlayService
{
    private readonly IMessenger _messenger = messenger ?? WeakReferenceMessenger.Default;
    private readonly IUiDispatcher _uiDispatcher = uiDispatcher;
}
