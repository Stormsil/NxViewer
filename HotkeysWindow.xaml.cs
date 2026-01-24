using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static NxTiler.Win32;

namespace NxTiler
{
    public partial class HotkeysWindow : Window
    {
        // Temp storage
        private int _kOverlay, _kMain, _kFocus, _kMin;
        private uint _mOverlay, _mMain, _mFocus, _mMin;

        public HotkeysWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load current
            _kOverlay = AppSettings.Default.HkOverlayKey; _mOverlay = AppSettings.Default.HkOverlayMod;
            _kMain = AppSettings.Default.HkMainKey;       _mMain = AppSettings.Default.HkMainMod;
            _kFocus = AppSettings.Default.HkFocusKey;     _mFocus = AppSettings.Default.HkFocusMod;
            _kMin = AppSettings.Default.HkMinimizeKey;    _mMin = AppSettings.Default.HkMinimizeMod;

            UpdateUI();
        }

        private void UpdateUI()
        {
            BoxOverlay.Text = FormatHotkey(_kOverlay, _mOverlay);
            BoxMain.Text = FormatHotkey(_kMain, _mMain);
            BoxFocus.Text = FormatHotkey(_kFocus, _mFocus);
            BoxMinimize.Text = FormatHotkey(_kMin, _mMin);
        }

        private string FormatHotkey(int vKey, uint mod)
        {
            if (vKey == 0) return "(Нет)";
            var key = KeyInterop.KeyFromVirtualKey(vKey);
            string s = key.ToString();
            
            // Special handling for ~
            if (vKey == 0xC0) s = "~"; 
            else if (key >= Key.F1 && key <= Key.F12) s = key.ToString();
            else if (key >= Key.D0 && key <= Key.D9) s = key.ToString().TrimStart('D');

            string m = "";
            if ((mod & MOD_CONTROL) != 0) m += "Ctrl + ";
            if ((mod & MOD_SHIFT) != 0) m += "Shift + ";
            if ((mod & MOD_ALT) != 0) m += "Alt + ";
            if ((mod & MOD_WIN) != 0) m += "Win + ";

            return m + s;
        }

        private void Box_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var box = (TextBox)sender;
            string tag = (string)box.Tag;

            // Handle modifiers
            uint mod = 0;
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) mod |= MOD_CONTROL;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) mod |= MOD_SHIFT;
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) mod |= MOD_ALT;
            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) mod |= MOD_WIN;

            // If only modifier is pressed, ignore
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LWin || e.Key == Key.RWin || e.Key == Key.System)
                return;

            int vKey = KeyInterop.VirtualKeyFromKey(e.Key);

            // Assign
            switch (tag)
            {
                case "Overlay": _kOverlay = vKey; _mOverlay = mod; break;
                case "Main": _kMain = vKey; _mMain = mod; break;
                case "Focus": _kFocus = vKey; _mFocus = mod; break;
                case "Minimize": _kMin = vKey; _mMin = mod; break;
            }
            UpdateUI();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            // Defaults
            _kOverlay = 112; _mOverlay = 0; // F1
            _kMain = 112;    _mMain = 2;    // Ctrl+F1
            _kFocus = 112;   _mFocus = 4;   // Shift+F1
            _kMin = 192;     _mMin = 0;     // ~
            UpdateUI();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.HkOverlayKey = _kOverlay; AppSettings.Default.HkOverlayMod = _mOverlay;
            AppSettings.Default.HkMainKey = _kMain;       AppSettings.Default.HkMainMod = _mMain;
            AppSettings.Default.HkFocusKey = _kFocus;     AppSettings.Default.HkFocusMod = _mFocus;
            AppSettings.Default.HkMinimizeKey = _kMin;    AppSettings.Default.HkMinimizeMod = _mMin;
            
            AppSettings.Default.Save();
            DialogResult = true;
            Close();
        }
    }
}
