using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static NxTiler.Win32;

namespace NxTiler
{
    public partial class HotkeysWindow : Window
    {
        // Temp storage
        private int _kOverlay, _kMain, _kFocus, _kMin, _kPrev, _kNext, _kRecord, _kRecPause, _kRecStop;
        private uint _mOverlay, _mMain, _mFocus, _mMin, _mPrev, _mNext, _mRecord, _mRecPause, _mRecStop;

        private static readonly Brush NormalBackground = new SolidColorBrush(Color.FromRgb(0x2a, 0x2f, 0x35));
        private static readonly Brush ConflictBackground = new SolidColorBrush(Color.FromRgb(0x5a, 0x2d, 0x2d));

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
            _kPrev = AppSettings.Default.HkPrevKey;       _mPrev = AppSettings.Default.HkPrevMod;
            _kNext = AppSettings.Default.HkNextKey;       _mNext = AppSettings.Default.HkNextMod;
            _kRecord = AppSettings.Default.HkRecordKey;     _mRecord = AppSettings.Default.HkRecordMod;
            _kRecPause = AppSettings.Default.HkRecPauseKey;  _mRecPause = AppSettings.Default.HkRecPauseMod;
            _kRecStop = AppSettings.Default.HkRecStopKey;    _mRecStop = AppSettings.Default.HkRecStopMod;

            UpdateUI();
            CheckConflicts();
        }

        private void UpdateUI()
        {
            BoxOverlay.Text = FormatHotkey(_kOverlay, _mOverlay);
            BoxMain.Text = FormatHotkey(_kMain, _mMain);
            BoxFocus.Text = FormatHotkey(_kFocus, _mFocus);
            BoxMinimize.Text = FormatHotkey(_kMin, _mMin);
            BoxPrev.Text = FormatHotkey(_kPrev, _mPrev);
            BoxNext.Text = FormatHotkey(_kNext, _mNext);
            BoxRecord.Text = FormatHotkey(_kRecord, _mRecord);
            BoxRecPause.Text = FormatHotkey(_kRecPause, _mRecPause);
            BoxRecStop.Text = FormatHotkey(_kRecStop, _mRecStop);
        }

        private string FormatHotkey(int vKey, uint mod)
        {
            if (vKey == 0) return "(None)";
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
                case "Prev": _kPrev = vKey; _mPrev = mod; break;
                case "Next": _kNext = vKey; _mNext = mod; break;
                case "Record": _kRecord = vKey; _mRecord = mod; break;
                case "RecPause": _kRecPause = vKey; _mRecPause = mod; break;
                case "RecStop": _kRecStop = vKey; _mRecStop = mod; break;
            }
            UpdateUI();
            CheckConflicts();
        }

        private void BtnClearHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                switch (tag)
                {
                    case "Overlay": _kOverlay = 0; _mOverlay = 0; break;
                    case "Main": _kMain = 0; _mMain = 0; break;
                    case "Focus": _kFocus = 0; _mFocus = 0; break;
                    case "Minimize": _kMin = 0; _mMin = 0; break;
                    case "Prev": _kPrev = 0; _mPrev = 0; break;
                    case "Next": _kNext = 0; _mNext = 0; break;
                    case "Record": _kRecord = 0; _mRecord = 0; break;
                    case "RecPause": _kRecPause = 0; _mRecPause = 0; break;
                    case "RecStop": _kRecStop = 0; _mRecStop = 0; break;
                }
                UpdateUI();
                CheckConflicts();
            }
        }

        private bool CheckConflicts()
        {
            // Collect all hotkey pairs (tag, key, mod, textbox)
            var entries = new List<(string tag, int key, uint mod, TextBox box)>
            {
                ("Overlay", _kOverlay, _mOverlay, BoxOverlay),
                ("Main", _kMain, _mMain, BoxMain),
                ("Focus", _kFocus, _mFocus, BoxFocus),
                ("Minimize", _kMin, _mMin, BoxMinimize),
                ("Prev", _kPrev, _mPrev, BoxPrev),
                ("Next", _kNext, _mNext, BoxNext),
                ("Record", _kRecord, _mRecord, BoxRecord),
                ("RecPause", _kRecPause, _mRecPause, BoxRecPause),
                ("RecStop", _kRecStop, _mRecStop, BoxRecStop),
            };

            // Reset all backgrounds
            foreach (var e in entries)
                e.box.Background = NormalBackground;

            // Find duplicates (skip entries with key == 0, i.e. cleared)
            var active = entries.Where(e => e.key != 0).ToList();
            var conflicting = new HashSet<string>();

            for (int i = 0; i < active.Count; i++)
            {
                for (int j = i + 1; j < active.Count; j++)
                {
                    if (active[i].key == active[j].key && active[i].mod == active[j].mod)
                    {
                        conflicting.Add(active[i].tag);
                        conflicting.Add(active[j].tag);
                    }
                }
            }

            // Highlight conflicting boxes
            foreach (var e in entries)
            {
                if (conflicting.Contains(e.tag))
                    e.box.Background = ConflictBackground;
            }

            return conflicting.Count > 0;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            // Defaults
            _kOverlay = 112; _mOverlay = 0; // F1
            _kMain = 112;    _mMain = 2;    // Ctrl+F1
            _kFocus = 112;   _mFocus = 4;   // Shift+F1
            _kMin = 192;     _mMin = 0;     // ~
            _kPrev = 37;     _mPrev = 0;    // Left Arrow
            _kNext = 39;     _mNext = 0;    // Right Arrow
            _kRecord = 113;  _mRecord = 0;  // F2
            _kRecPause = 114; _mRecPause = 0; // F3
            _kRecStop = 115; _mRecStop = 0; // F4
            UpdateUI();
            CheckConflicts();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Block save if there are conflicts
            if (CheckConflicts())
            {
                MessageBox.Show("Hotkey conflicts detected. Fix duplicates before saving.",
                    "Conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppSettings.Default.HkOverlayKey = _kOverlay; AppSettings.Default.HkOverlayMod = _mOverlay;
            AppSettings.Default.HkMainKey = _kMain;       AppSettings.Default.HkMainMod = _mMain;
            AppSettings.Default.HkFocusKey = _kFocus;     AppSettings.Default.HkFocusMod = _mFocus;
            AppSettings.Default.HkMinimizeKey = _kMin;    AppSettings.Default.HkMinimizeMod = _mMin;
            AppSettings.Default.HkPrevKey = _kPrev;       AppSettings.Default.HkPrevMod = _mPrev;
            AppSettings.Default.HkNextKey = _kNext;       AppSettings.Default.HkNextMod = _mNext;
            AppSettings.Default.HkRecordKey = _kRecord;     AppSettings.Default.HkRecordMod = _mRecord;
            AppSettings.Default.HkRecPauseKey = _kRecPause;  AppSettings.Default.HkRecPauseMod = _mRecPause;
            AppSettings.Default.HkRecStopKey = _kRecStop;    AppSettings.Default.HkRecStopMod = _mRecStop;

            AppSettings.Default.Save();
            DialogResult = true;
            Close();
        }
    }
}
