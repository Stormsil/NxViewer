namespace NxTiler.Application.Abstractions;

public interface ITrayService : IDisposable
{
    void Initialize(bool autoArrangeEnabled);

    void ShowBalloon(string title, string message);

    void SetAutoArrangeState(bool enabled);
}
