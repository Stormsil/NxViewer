using CommunityToolkit.Mvvm.Messaging;
using NxTiler.Application.Messaging;

namespace NxTiler.Infrastructure.Hotkeys;

public sealed partial class GlobalHotkeyService
{
    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == WmHotkey)
        {
            var id = wParam.ToInt32();
            if (_idToAction.TryGetValue(id, out var action))
            {
                handled = true;
                _messenger.Send(new HotkeyActionPressedMessage(action));
            }
        }

        return nint.Zero;
    }
}
