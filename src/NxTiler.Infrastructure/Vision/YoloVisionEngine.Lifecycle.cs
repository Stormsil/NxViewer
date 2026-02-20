namespace NxTiler.Infrastructure.Vision;

public sealed partial class YoloVisionEngine
{
    public void Dispose()
    {
        if (_sessionProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
