using System.Windows;
using System.Windows.Input;
using WpfKey = System.Windows.Input.Key;

namespace NxTiler.App.Controls;

public partial class HotkeyBox
{
    private void InputBox_GotFocus(object sender, RoutedEventArgs e)
    {
        InputBox.Text = "Press keys...";
    }

    private void InputBox_LostFocus(object sender, RoutedEventArgs e)
    {
        UpdateText();
    }

    private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;

        var key = e.Key == WpfKey.System ? e.SystemKey : e.Key;

        // Ignore standalone modifier presses
        if (key is WpfKey.LeftCtrl or WpfKey.RightCtrl or
            WpfKey.LeftAlt or WpfKey.RightAlt or
            WpfKey.LeftShift or WpfKey.RightShift or
            WpfKey.LWin or WpfKey.RWin)
        {
            return;
        }

        // Calculate modifiers
        uint modifiers = 0;
        if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) modifiers |= 1;
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) modifiers |= 2;
        if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) modifiers |= 4;
        if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) modifiers |= 8;

        if (key == WpfKey.Escape)
        {
            Key = 0;
            Modifiers = 0;
            Keyboard.ClearFocus();
            return;
        }

        if (key == WpfKey.Back)
        {
            Key = 0;
            Modifiers = 0;
            return;
        }

        Key = KeyInterop.VirtualKeyFromKey(key);
        Modifiers = modifiers;
        Keyboard.ClearFocus();
    }
}
