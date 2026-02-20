using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NxTiler.App.Controls;

public partial class HotkeyBox
{
    private static void OnKeyOrModifiersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HotkeyBox box)
        {
            box.UpdateText();
        }
    }

    private void UpdateText()
    {
        if (Key == 0)
        {
            DisplayText = "None";
            return;
        }

        var sb = new StringBuilder();
        if ((Modifiers & 1) != 0) sb.Append("Alt + "); // MOD_ALT = 1
        if ((Modifiers & 2) != 0) sb.Append("Ctrl + "); // MOD_CONTROL = 2
        if ((Modifiers & 4) != 0) sb.Append("Shift + "); // MOD_SHIFT = 4
        if ((Modifiers & 8) != 0) sb.Append("Win + "); // MOD_WIN = 8

        try
        {
            var key = KeyInterop.KeyFromVirtualKey(Key);
            sb.Append(key.ToString());
        }
        catch
        {
            sb.Append($"VK_{Key}");
        }

        DisplayText = sb.ToString();
    }
}
