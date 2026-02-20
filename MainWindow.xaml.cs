using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using Ookii.Dialogs.Wpf;
using static NxTiler.Win32;
using System.Windows.Media;

namespace NxTiler
{
    public enum TileMode { Grid, Focus, MaxSize }

    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(5000) };
        private readonly List<TargetWindow> _targets = new();
        private OverlayWindow? _overlay;

        private TaskbarIcon? _tray;
        private MenuItem? _trayAutoItem;

        private HwndSource? _src;
        private const int HOTKEY_OVERLAY_ID = 100; // F1
        private const uint VK_F1 = 0x70;
        
        private const int HOTKEY_MAIN_ID = 101; // Ctrl + F1
        private const int HOTKEY_FOCUS_MODE_ID = 102; // Shift + F1
        private const int HOTKEY_MINIMIZE_ID = 103; // ` (Backtick)
        private const int HOTKEY_PREV_ID = 104; // Left Arrow
        private const int HOTKEY_NEXT_ID = 105; // Right Arrow
        private const int HOTKEY_RECORD_ID = 106; // F2
        private const int HOTKEY_REC_PAUSE_ID = 107; // F3
        private const int HOTKEY_REC_STOP_ID = 108; // F4
        private const uint VK_OEM_3 = 0xC0; // Backtick key

        private readonly int _selfPid = System.Diagnostics.Process.GetCurrentProcess().Id;
        private bool _isRealExit = false;

        // Mode State
        private TileMode _currentMode = TileMode.Grid;
        private IntPtr _focusedHwnd = IntPtr.Zero;
        private bool _allMinimized = false;
        private DateTime _lastModeSwitch = DateTime.MinValue;

        // Smart Auto
        private WindowEventMonitor? _eventMonitor;
        private bool _isForeignAppActive = false;

        // Drag cooldown state machine
        private bool _wasDragging = false;
        private DateTime _dragReleaseTime = DateTime.MinValue;
        private const int DRAG_COOLDOWN_MS = 1500;

        // Recording State
        private enum RecordingPhase { Idle, MaskEditing, Recording, Paused }
        private RecordingPhase _recordingPhase = RecordingPhase.Idle;
        private ScreenRecorder? _recorder;
        private MaskOverlayWindow? _maskOverlay;
        private DimOverlayWindow? _dimOverlay;
        private RecordingBarWindow? _recordingBar;
        private IntPtr _recordingTargetHwnd = IntPtr.Zero;
        private bool _isStopDialogShowing = false;
        private int _recX, _recY, _recW, _recH;

        public MainWindow()
        {
            InitializeComponent();
            _timer.Tick += (_, __) => AutoTick();
            StateChanged += MainWindow_StateChanged;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _src = (HwndSource)PresentationSource.FromVisual(this)!;
            _src.AddHook(WndProc);

            RegisterHotKeys();

            // Initialize event-driven window monitoring
            _eventMonitor = new WindowEventMonitor(_selfPid);
            _eventMonitor.ArrangeNeeded += (_, _) => SmartArrange();
            _eventMonitor.ForegroundChanged += OnForegroundChanged;
            _eventMonitor.Start();

            CreateTray();
            RefreshWindows();

            // Auto-install ffmpeg if not found
            if (FfmpegSetup.FindFfmpeg() == null)
            {
                Status("Installing ffmpeg...");
                var path = await FfmpegSetup.DownloadAsync((_, text) =>
                    Dispatcher.Invoke(() => Status(text)));
                Status(path != null ? $"ffmpeg installed: {path}" : "ffmpeg not installed — recording unavailable.");
            }
        }

        private void RegisterHotKeys()
        {
            if (_src == null) return;
            try
            {
                UnregisterHotKey(_src.Handle, HOTKEY_OVERLAY_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_MAIN_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_FOCUS_MODE_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_MINIMIZE_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_PREV_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_NEXT_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_RECORD_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_REC_PAUSE_ID);
                UnregisterHotKey(_src.Handle, HOTKEY_REC_STOP_ID);
            } catch { }

            RegisterHotKey(_src.Handle, HOTKEY_OVERLAY_ID, AppSettings.Default.HkOverlayMod, (uint)AppSettings.Default.HkOverlayKey);
            RegisterHotKey(_src.Handle, HOTKEY_MAIN_ID, AppSettings.Default.HkMainMod, (uint)AppSettings.Default.HkMainKey);
            RegisterHotKey(_src.Handle, HOTKEY_FOCUS_MODE_ID, AppSettings.Default.HkFocusMod, (uint)AppSettings.Default.HkFocusKey);
            RegisterHotKey(_src.Handle, HOTKEY_MINIMIZE_ID, AppSettings.Default.HkMinimizeMod, (uint)AppSettings.Default.HkMinimizeKey);
            RegisterHotKey(_src.Handle, HOTKEY_PREV_ID, AppSettings.Default.HkPrevMod, (uint)AppSettings.Default.HkPrevKey);
            RegisterHotKey(_src.Handle, HOTKEY_NEXT_ID, AppSettings.Default.HkNextMod, (uint)AppSettings.Default.HkNextKey);
            RegisterHotKey(_src.Handle, HOTKEY_RECORD_ID, AppSettings.Default.HkRecordMod, (uint)AppSettings.Default.HkRecordKey);
            RegisterHotKey(_src.Handle, HOTKEY_REC_PAUSE_ID, AppSettings.Default.HkRecPauseMod, (uint)AppSettings.Default.HkRecPauseKey);
            RegisterHotKey(_src.Handle, HOTKEY_REC_STOP_ID, AppSettings.Default.HkRecStopMod, (uint)AppSettings.Default.HkRecStopKey);

            UpdateHotkeyHelp();
        }

        private void UpdateHotkeyHelp()
        {
            // Simple helper to format for display
            string Fmt(int k, uint m) 
            {
                var key = System.Windows.Input.KeyInterop.KeyFromVirtualKey(k);
                string s = key.ToString();
                if (k == 0xC0) s = "~";
                else if (key >= System.Windows.Input.Key.D0 && key <= System.Windows.Input.Key.D9) s = s.TrimStart('D');
                
                string mod = "";
                if ((m & MOD_CONTROL) != 0) mod += "Ctrl+";
                if ((m & MOD_SHIFT) != 0) mod += "Shift+";
                if ((m & MOD_ALT) != 0) mod += "Alt+";
                if ((m & MOD_WIN) != 0) mod += "Win+";
                return mod + s;
            }

            if (HelpTextOverlay != null) HelpTextOverlay.Text = $"{Fmt(AppSettings.Default.HkOverlayKey, AppSettings.Default.HkOverlayMod)} — Overlay";
            if (HelpTextMain != null) HelpTextMain.Text = $"{Fmt(AppSettings.Default.HkMainKey, AppSettings.Default.HkMainMod)} — Main";
            if (HelpTextFocus != null) HelpTextFocus.Text = $"{Fmt(AppSettings.Default.HkFocusKey, AppSettings.Default.HkFocusMod)} — Focus";
            if (HelpTextMin != null) HelpTextMin.Text = $"{Fmt(AppSettings.Default.HkMinimizeKey, AppSettings.Default.HkMinimizeMod)} — Minimize";
            if (HelpTextNav != null) HelpTextNav.Text = $"{Fmt(AppSettings.Default.HkPrevKey, AppSettings.Default.HkPrevMod)}/{Fmt(AppSettings.Default.HkNextKey, AppSettings.Default.HkNextMod)} — Navigate";
            if (HelpTextRec != null) HelpTextRec.Text = $"{Fmt(AppSettings.Default.HkRecordKey, AppSettings.Default.HkRecordMod)} Rec | {Fmt(AppSettings.Default.HkRecPauseKey, AppSettings.Default.HkRecPauseMod)} Pause | {Fmt(AppSettings.Default.HkRecStopKey, AppSettings.Default.HkRecStopMod)} Stop";
        }

        private void BtnHotkeys_Click(object sender, RoutedEventArgs e)
        {
            var win = new HotkeysWindow { Owner = this };
            if (win.ShowDialog() == true)
            {
                RegisterHotKeys();
            }
        }

        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            if (!_isRealExit)
            {
                e.Cancel = true;
                Hide();
                ShowInTaskbar = false;

                // Show balloon tip on first minimize to tray
                if (!AppSettings.Default.TrayHintShown && _tray != null)
                {
                    _tray.ShowBalloonTip(
                        "NxTiler",
                        "Minimized to tray. Double-click to open, right-click for menu.",
                        BalloonIcon.Info);
                    AppSettings.Default.TrayHintShown = true;
                    AppSettings.Default.Save();
                }
                Status("Minimized to tray.");
            }
            else
            {
                try 
                { 
                    if (_src != null) 
                    {
                        UnregisterHotKey(_src.Handle, HOTKEY_OVERLAY_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_MAIN_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_FOCUS_MODE_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_MINIMIZE_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_PREV_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_NEXT_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_RECORD_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_REC_PAUSE_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_REC_STOP_ID);
                    }
                }
                catch { }
                _eventMonitor?.Dispose();
                _recorder?.Abort();
                _recordingBar?.Cleanup();
                _recordingBar?.Close();
                _maskOverlay?.Close();
                _dimOverlay?.Close();
                _overlay?.Close();
                _tray?.Dispose();
            }
        }

        private void CreateTray()
        {
            var iconUri = new Uri("pack://application:,,,/app.ico");
            var stream = Application.GetResourceStream(iconUri)?.Stream;

            _tray = new TaskbarIcon
            {
                ToolTipText = "NoMachine Tiler",
                Icon = stream != null ? new System.Drawing.Icon(stream) : System.Drawing.SystemIcons.Application,
                Visibility = Visibility.Visible
            };
            _tray.TrayMouseDoubleClick += (_, __) => RestoreFromTray();

            var menu = new ContextMenu();
            var mShow = new MenuItem { Header = "Show" }; mShow.Click += (_, __) => RestoreFromTray();
            var mArrange = new MenuItem { Header = "Arrange Now" }; mArrange.Click += (_, __) => ArrangeNow();
            _trayAutoItem = new MenuItem { Header = "Auto-Arrange", IsCheckable = true, IsChecked = AutoArrangeCheck.IsChecked == true };
            _trayAutoItem.Click += (_, __) => AutoArrangeCheck.IsChecked = _trayAutoItem.IsChecked;
            var mExit = new MenuItem { Header = "Exit" }; 
            mExit.Click += (_, __) => { _isRealExit = true; Close(); };

            menu.Items.Add(mShow);
            menu.Items.Add(mArrange);
            menu.Items.Add(new Separator());
            menu.Items.Add(_trayAutoItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(mExit);

            _tray.ContextMenu = menu;

            AutoArrangeCheck.Checked += (_, __) => { if (_trayAutoItem != null) _trayAutoItem.IsChecked = true; };
            AutoArrangeCheck.Unchecked += (_, __) => { if (_trayAutoItem != null) _trayAutoItem.IsChecked = false; };
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
            }
        }

        private void RestoreFromTray()
        {
            ShowInTaskbar = true;
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void ToggleMainWindow()
        {
            if (Visibility == Visibility.Visible) { Hide(); ShowInTaskbar = false; }
            else RestoreFromTray();
            // Sync overlay button state
            _overlay?.SetMainWin(Visibility == Visibility.Visible);
        }

        private string GetModeText() => _currentMode switch
        {
            TileMode.Grid => "GRID",
            TileMode.Focus => "FOCUS",
            TileMode.MaxSize => "MAXSIZE",
            _ => "GRID"
        };

        private void CycleMode()
        {
            // Carousel: Grid → Focus → MaxSize → Grid
            _currentMode = (TileMode)(((int)_currentMode + 1) % 3);
            _lastModeSwitch = DateTime.UtcNow;
            _overlay?.SetModeText(GetModeText());
            Status($"Mode: {GetModeText()}");
            
            // Focus and MaxSize modes enforce auto-arrange
            if (_currentMode != TileMode.Grid && AutoArrangeCheck.IsChecked == false)
            {
                AutoArrangeCheck.IsChecked = true;
            }
            ArrangeNow();
        }

        private void NavigateWindow(int delta)
        {
            RefreshWindows();
            var candidates = _targets.Where(t => !t.IsMaximized).ToList();
            if (candidates.Count == 0) return;

            // Find current index
            int currentIndex = 0;
            if (_focusedHwnd != IntPtr.Zero)
            {
                var current = candidates.FirstOrDefault(c => c.Hwnd == _focusedHwnd);
                if (current != null) currentIndex = candidates.IndexOf(current);
            }

            // Calculate new index with wrap-around
            int newIndex = (currentIndex + delta + candidates.Count) % candidates.Count;
            _focusedHwnd = candidates[newIndex].Hwnd;
            
            // Activate selected window to make it foreground and prevent AutoTick reset
            _lastModeSwitch = DateTime.UtcNow;
            if (_currentMode != TileMode.Grid)
            {
                SetForegroundWindow(_focusedHwnd);
            }

            ArrangeNow();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == HOTKEY_OVERLAY_ID) { ToggleOverlay(); handled = true; }
                else if (id == HOTKEY_MAIN_ID) { ToggleMainWindow(); handled = true; }
                else if (id == HOTKEY_FOCUS_MODE_ID) { CycleMode(); handled = true; }
                else if (id == HOTKEY_MINIMIZE_ID) { ToggleMinimizeAll(); handled = true; }
                else if (id == HOTKEY_PREV_ID) { NavigateWindow(-1); handled = true; }
                else if (id == HOTKEY_NEXT_ID) { NavigateWindow(1); handled = true; }
                else if (id == HOTKEY_RECORD_ID) { HandleRecordHotkey(); handled = true; }
                else if (id == HOTKEY_REC_PAUSE_ID) { HandlePauseHotkey(); handled = true; }
                else if (id == HOTKEY_REC_STOP_ID) { HandleStopHotkey(); handled = true; }
            }
            return IntPtr.Zero;
        }

        private void ToggleMinimizeAll()
        {
            _allMinimized = !_allMinimized;
            Overlay_RequestToggleMinimize(this, _allMinimized);
        }

        private void Overlay_RequestToggleMinimize(object? sender, bool minimize)
        {
            _allMinimized = minimize;
            RefreshWindows(); 
            foreach (var t in _targets)
            {
                ShowWindow(t.Hwnd, minimize ? SW_SHOWMINIMIZED : SW_SHOWNORMAL);
            }
            if (!minimize) ArrangeNow();
        }

        private void ToggleOverlay()
        {
            if (_overlay == null)
            {
                _overlay = new OverlayWindow { Owner = this };
                _overlay.RequestArrange += (_, __) => ArrangeNow();
                _overlay.RequestToggleAuto += (_, on) => AutoArrangeCheck.IsChecked = on;
                _overlay.RequestToggleMinimize += Overlay_RequestToggleMinimize;
                _overlay.RequestToggleMain += (_, show) => ToggleMainWindow(show);
                _overlay.RequestClose += (_, __) => _overlay?.Hide();
                _overlay.RequestCycleMode += (_, __) => CycleMode();
                _overlay.WindowSelected += Overlay_WindowSelected;
                _overlay.WindowReconnect += Overlay_WindowReconnect;

                // Restore saved position or use default
                if (AppSettings.Default.OverlayLeft >= 0 && AppSettings.Default.OverlayTop >= 0)
                {
                    _overlay.Left = AppSettings.Default.OverlayLeft;
                    _overlay.Top = AppSettings.Default.OverlayTop;
                }
                else
                {
                    _overlay.Left = SystemParameters.WorkArea.Left + 20;
                    _overlay.Top = SystemParameters.WorkArea.Top + 20;
                }
                _overlay.LocationChanged += (_, _) => _overlay?.SavePosition();
                _overlay.Show();
                _overlay.SetAuto(AutoArrangeCheck.IsChecked == true);
                _overlay.SetModeText(GetModeText());
                _overlay.SetMainWin(Visibility == Visibility.Visible);
                
                // Force update list immediately
                ArrangeNow(); 
            }
            else
            {
                if (_overlay.IsVisible) _overlay.Hide(); 
                else 
                {
                    _overlay.Show();
                    // Force update list on show
                    ArrangeNow();
                }
                _overlay.SetAuto(AutoArrangeCheck.IsChecked == true);
                _overlay.SetModeText(GetModeText());
                _overlay.SetMainWin(Visibility == Visibility.Visible);
            }
        }

        private void ToggleMainWindow(bool? forceState = null)
        {
            if (forceState.HasValue)
            {
                if (forceState.Value) RestoreFromTray();
                else { Hide(); ShowInTaskbar = false; }
            }
            else
            {
                if (Visibility == Visibility.Visible) { Hide(); ShowInTaskbar = false; }
                else RestoreFromTray();
            }
            _overlay?.SetMainWin(Visibility == Visibility.Visible);
        }

        private void Overlay_WindowSelected(object? sender, int index)
        {
            if (index >= 0 && index < _targets.Count)
            {
                var target = _targets[index];
                _focusedHwnd = target.Hwnd;
                
                // Activate selected window to prevent AutoTick from overwriting selection
                if (_currentMode != TileMode.Grid)
                {
                    SetForegroundWindow(target.Hwnd);
                }
                
                // Always arrange - this updates selection in all modes
                ArrangeNow();
            }
        }

        

                private async void Overlay_WindowReconnect(object? sender, int index)

                {

                    if (index >= 0 && index < _targets.Count)

                    {

                        var target = _targets[index];

                        var name = target.SourceName;

                        

                        // 1. Close Process

                        GetWindowThreadProcessId(target.Hwnd, out uint pid);

                        try { System.Diagnostics.Process.GetProcessById((int)pid).Kill(); } catch { }

        

                        // 2. Wait

                        Status($"Reconnecting {name}...");

                        await Task.Delay(1500);

        

                        // 3. Relaunch

                        NomachineLauncher.LaunchSession(name, AppSettings.Default.NxsFolder);

                        

                        // 4. Wait & Refresh

                        await Task.Delay(2500);

                        RefreshWindows();

                        ArrangeNow();

                    }

                }

        

                [System.Runtime.InteropServices.DllImport("user32.dll")]

                private static extern bool IsIconic(IntPtr hWnd);

                [System.Runtime.InteropServices.DllImport("user32.dll")]

                private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var win = new SettingsWindow { Owner = this };
            win.ShowDialog();
            ArrangeNow();
        }

        private async void OpenSelected_Click(object sender, RoutedEventArgs e)
        {
            RefreshWindows(); // Update _targets to know what's running
            var sessions = NomachineLauncher.FindSessions(AppSettings.Default.NxsFolder, AppSettings.Default.NameFilter);
            var disabled = new HashSet<string>(AppSettings.Default.DisabledFiles.Cast<string>(), StringComparer.OrdinalIgnoreCase);
            
            // Filter disabled AND already running
            var runningNames = new HashSet<string>(_targets.Select(t => t.SourceName), StringComparer.OrdinalIgnoreCase);
            sessions = sessions.Where(s => !disabled.Contains(s.name) && !runningNames.Contains(s.name)).ToList();

            if (sessions.Count == 0) { Status("No matching (enabled) .nxs files found or already open."); return; }

            NomachineLauncher.OpenIfNeeded(sessions);
            await Task.Delay(1200);
            RefreshWindows(sessions.Select(s => s.name).ToList());
            ArrangeNow();
        }

        private void AutoArrangeCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (AutoArrangeCheck.IsChecked == true) _timer.Start(); else _timer.Stop();
            _overlay?.SetAuto(AutoArrangeCheck.IsChecked == true);
            if (_trayAutoItem != null) _trayAutoItem.IsChecked = AutoArrangeCheck.IsChecked == true;
            Status(AutoArrangeCheck.IsChecked == true ? "Auto-Arrange: ON" : "Auto-Arrange: OFF");
        }

        private void ArrangeNow_Click(object sender, RoutedEventArgs e) => ArrangeNow();

        private void AutoTick()
        {
            // Heartbeat fallback (5s) — catches edge cases that events miss
            SmartArrange();
        }

        private void SmartArrange()
        {
            // Gate 1: Block during recording
            if (_recordingPhase == RecordingPhase.Recording || _recordingPhase == RecordingPhase.Paused)
                return;

            // Gate 2: Drag detection + cooldown
            bool isMouseDown = (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
            if (isMouseDown)
            {
                _wasDragging = true;
                return;
            }
            if (_wasDragging)
            {
                _wasDragging = false;
                _dragReleaseTime = DateTime.UtcNow;
                return;
            }
            if ((DateTime.UtcNow - _dragReleaseTime).TotalMilliseconds < AppSettings.Default.DragCooldownMs)
                return;

            // Gate 3: Foreign app active — only refresh tracked HWNDs, skip arrange
            if (_isForeignAppActive && AutoArrangeCheck.IsChecked == true)
            {
                RefreshWindows();
                _eventMonitor?.UpdateTrackedWindows(_targets.Select(t => t.Hwnd));
                return;
            }

            // Gate 4: SuspendOnMax
            if (AppSettings.Default.SuspendOnMax && _targets.Any(t => t.IsMaximized))
            {
                Status("Paused: window maximized.");
                return;
            }

            ArrangeNow();
        }

        private void OnForegroundChanged(object? sender, IntPtr foregroundHwnd)
        {
            if (AutoArrangeCheck.IsChecked != true) return;

            GetWindowThreadProcessId(foregroundHwnd, out uint pid);

            // Is it our own NxTiler process?
            if (pid == (uint)_selfPid)
            {
                _isForeignAppActive = false;
                return;
            }

            // Is it one of our tracked NoMachine windows?
            var target = _targets.FirstOrDefault(t => t.Hwnd == foregroundHwnd);
            if (target != null)
            {
                _isForeignAppActive = false;

                // Update focused window if in Focus/MaxSize and past cooldown
                if (_currentMode != TileMode.Grid &&
                    (DateTime.UtcNow - _lastModeSwitch).TotalMilliseconds > 1200 &&
                    target.Hwnd != _focusedHwnd)
                {
                    _focusedHwnd = target.Hwnd;
                }
                return;
            }

            // Foreign app — suppress auto-arrange
            _isForeignAppActive = true;
        }

        private void RefreshWindows(List<string>? preferNames = null)
        {
            _targets.Clear();
            Regex? reTitle = string.IsNullOrWhiteSpace(AppSettings.Default.TitleFilter) ? null : new Regex(AppSettings.Default.TitleFilter, RegexOptions.IgnoreCase);
            Regex? reName = string.IsNullOrWhiteSpace(AppSettings.Default.NameFilter) ? null : new Regex(AppSettings.Default.NameFilter, RegexOptions.IgnoreCase);

            var temp = new List<TargetWindow>();
            EnumWindows((h, l) =>
            {
                if (!IsWindowVisible(h)) return true;
                var title = Win32.GetWindowText(h);
                if (string.IsNullOrWhiteSpace(title)) return true;

                GetWindowThreadProcessId(h, out uint pid);
                if (pid == (uint)_selfPid) return true;

                if (!title.Contains("NoMachine", StringComparison.OrdinalIgnoreCase)) return true;
                var sessionName = ExtractSessionNameFromTitle(title);
                if (string.IsNullOrWhiteSpace(sessionName) || sessionName.Equals("NoMachine", StringComparison.OrdinalIgnoreCase)) return true;

                if (reName != null && !reName.IsMatch(sessionName)) return true;
                if (reTitle != null && !reTitle.IsMatch(title)) return true;

                var wp = new WINDOWPLACEMENT { length = System.Runtime.InteropServices.Marshal.SizeOf<WINDOWPLACEMENT>() };
                GetWindowPlacement(h, ref wp);

                temp.Add(new TargetWindow
                {
                    Hwnd = h,
                    Title = title,
                    IsMaximized = (wp.showCmd == SW_SHOWMAXIMIZED),
                    SourceName = sessionName
                });
                return true;
            }, IntPtr.Zero);

            IEnumerable<TargetWindow> chosen = temp;
            if (preferNames != null && preferNames.Count > 0)
            {
                var set = new HashSet<string>(preferNames, StringComparer.OrdinalIgnoreCase);
                chosen = temp.Where(t => set.Contains(t.SourceName));
            }

            bool desc = AppSettings.Default.SortDesc;
            IEnumerable<TargetWindow> ordered = desc
                ? chosen.OrderByDescending(t => ParseNumericSuffix(t.SourceName)).ThenByDescending(t => t.SourceName, StringComparer.OrdinalIgnoreCase)
                : chosen.OrderBy(t => ParseNumericSuffix(t.SourceName)).ThenBy(t => t.SourceName, StringComparer.OrdinalIgnoreCase);

            _targets.AddRange(ordered);

            WindowsList.ItemsSource = _targets.Select(t => new
            {
                t.HwndHex,
                Title = t.Title,
                t.State,
                Source = t.SourceName
            }).ToList();

            Status($"Windows: {_targets.Count}");
        }

        private static string ExtractSessionNameFromTitle(string title)
        {
            var idx1 = title.IndexOf(" - NoMachine", StringComparison.OrdinalIgnoreCase);
            if (idx1 > 0) return title[..idx1].Trim();
            const string token = "NoMachine - ";
            var idx2 = title.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (idx2 >= 0 && idx2 + token.Length < title.Length) return title[(idx2 + token.Length)..].Trim();
            var m = Regex.Match(title, @"\b(?:WoW|Poe)\d+\b", RegexOptions.IgnoreCase);
            if (m.Success) return m.Value;
            return "";
        }

        private static int ParseNumericSuffix(string s)
        {
            var m = Regex.Match(s ?? "", @"(\d+)$");
            return m.Success ? int.Parse(m.Groups[1].Value) : int.MaxValue;
        }

        private void ArrangeNow()
        {
            // Cancel mask editing — windows are about to move, mask position becomes invalid
            if (_recordingPhase == RecordingPhase.MaskEditing)
                CancelMaskEditing();

            // Block arrange during active recording (capture coordinates are fixed)
            if (_recordingPhase == RecordingPhase.Recording || _recordingPhase == RecordingPhase.Paused)
                return;

            RefreshWindows();

            // Keep event monitor in sync with current tracked windows
            _eventMonitor?.UpdateTrackedWindows(_targets.Select(t => t.Hwnd));

            var candidates = _targets.Where(t => !t.IsMaximized).ToList();
            if (candidates.Count == 0)
            {
                // Clear overlay if no windows? Or keep last known?
                _overlay?.UpdateWindowList(new List<string>(), -1);
                Status("Nothing to arrange.");
                return;
            }

            // Update Overlay UI (Common for both modes)
            // Determine active index based on _focusedHwnd
            int activeIndex = -1;
            if (_focusedHwnd != IntPtr.Zero)
            {
                var focusWindow = candidates.FirstOrDefault(c => c.Hwnd == _focusedHwnd);
                if (focusWindow != null) activeIndex = candidates.IndexOf(focusWindow);
            }
            
            _overlay?.UpdateWindowList(candidates.Select(c => c.SourceName).ToList(), activeIndex);

            switch (_currentMode)
            {
                case TileMode.Grid:
                    ArrangeGridMode(candidates);
                    break;
                case TileMode.Focus:
                    ArrangeFocusMode(candidates);
                    break;
                case TileMode.MaxSize:
                    ArrangeMaxSizeMode(candidates);
                    break;
            }

            // Ensure Overlay is always on top of the arranged windows
            if (_overlay != null && _overlay.IsVisible)
            {
                 var interop = new WindowInteropHelper(_overlay);
                 SetWindowPos(interop.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
        }

        private void ArrangeGridMode(List<TargetWindow> candidates)
        {
            var waPx = Win32.GetWorkAreaPxForWindow(candidates[0].Hwnd);
            int topPad = AppSettings.Default.TopPad;
            int effTop = waPx.y + topPad;
            int effHeight = Math.Max(1, waPx.h - topPad);

            int rows, cols;
            if (candidates.Count is >= 6 and <= 10)
            {
                rows = 2;
                cols = (int)Math.Ceiling(candidates.Count / 2.0);
            }
            else
            {
                (rows, cols) = Tiler.ComputeGrid(candidates.Count, waPx.w, effHeight);
            }

            ApplyLayout(candidates, waPx.x, effTop, waPx.w, effHeight, rows, cols);
        }

        private void ArrangeFocusMode(List<TargetWindow> candidates)
        {
            // Ensure we have a valid focus target
            if (_focusedHwnd == IntPtr.Zero || !candidates.Any(c => c.Hwnd == _focusedHwnd))
            {
                // Default to the first one if current focus is invalid
                _focusedHwnd = candidates.First().Hwnd;
            }

            var focusWindow = candidates.First(c => c.Hwnd == _focusedHwnd);
            var others = candidates.Where(c => c.Hwnd != _focusedHwnd).ToList();

            var waPx = Win32.GetWorkAreaPxForWindow(focusWindow.Hwnd);
            int topPad = AppSettings.Default.TopPad;
            int effTop = waPx.y + topPad;
            int effHeight = Math.Max(1, waPx.h - topPad);
            int gap = AppSettings.Default.Gap;

            // Calculate Grid for Others
            int bottomRows = 1;
            int bottomCols = others.Count;
            
            // If too many others, split into 2 rows
            if (others.Count > 10)
            {
                bottomRows = 2;
                bottomCols = (int)Math.Ceiling(others.Count / 2.0);
            }

            // Define minimum height for bottom rows (e.g., 150px or proportional)
            // Let's use 20% height logic or fixed minimum
            int rowHeight = (int)(effHeight * 0.15); // 15% per row
            if (rowHeight < 120) rowHeight = 120; // Absolute min limit

            int bottomAreaHeight = (bottomRows * rowHeight) + ((bottomRows - 1) * gap);
            int focusAreaHeight = effHeight - bottomAreaHeight - gap;

            // Place Focused Window
            SafeSetWindowPos(focusWindow.Hwnd, waPx.x, effTop, waPx.w, focusAreaHeight);

            // Place Others
            if (others.Count > 0)
            {
                int bottomY = effTop + focusAreaHeight + gap;
                ApplyLayout(others, waPx.x, bottomY, waPx.w, bottomAreaHeight, bottomRows, bottomCols);
            }
        }

        private void ArrangeMaxSizeMode(List<TargetWindow> candidates)
        {
            // Ensure we have a valid focus target
            if (_focusedHwnd == IntPtr.Zero || !candidates.Any(c => c.Hwnd == _focusedHwnd))
            {
                // Default to the first one if current focus is invalid
                _focusedHwnd = candidates.First().Hwnd;
            }

            var focusWindow = candidates.First(c => c.Hwnd == _focusedHwnd);
            var others = candidates.Where(c => c.Hwnd != _focusedHwnd).ToList();

            // Get work area (excludes taskbar)
            var waPx = Win32.GetWorkAreaPxForWindow(focusWindow.Hwnd);

            // Fill entire work area without covering taskbar
            SafeSetWindowPos(focusWindow.Hwnd, waPx.x, waPx.y, waPx.w, waPx.h);
        }

        private void ApplyLayout(List<TargetWindow> windows, int startX, int startY, int width, int height, int rows, int cols)
        {
            int gap = AppSettings.Default.Gap;
            int totalGapW = (cols - 1) * gap;
            int totalGapH = (rows - 1) * gap;
            int baseW = (width - totalGapW) / cols;
            int baseH = (height - totalGapH) / rows;
            int remW = (width - totalGapW) % cols;
            int remH = (height - totalGapH) % rows;

            int i = 0;
            int y = startY;

            for (int r = 0; r < rows; r++)
            {
                int h = baseH + (r < remH ? 1 : 0);
                int x = startX;
                for (int c = 0; c < cols; c++)
                {
                    if (i >= windows.Count) break;

                    int w = baseW + (c < remW ? 1 : 0);
                    SafeSetWindowPos(windows[i].Hwnd, x, y, w, h);
                    
                    x += w + gap;
                    i++;
                }
                y += h + gap;
            }
        }

        private void SafeSetWindowPos(IntPtr hwnd, int x, int y, int w, int h)
        {
            // Anti-flicker: Get current VISUAL bounds (DWM) and compare with target
            var cur = Win32.GetWindowBoundsPx(hwnd);
            
            // Tolerance epsilon (1px for better precision)
            if (Math.Abs(cur.Left - x) > 1 || 
                Math.Abs(cur.Top - y) > 1 || 
                Math.Abs(cur.Width - w) > 1 || 
                Math.Abs(cur.Height - h) > 1)
            {
                // Compensate for invisible borders (Win10/11)
                // Windows has invisible resize handles that make windows look smaller than they are.
                var borders = Win32.GetInvisibleBorder(hwnd);
                
                int targetX = x - borders.left;
                int targetY = y - borders.top;
                int targetW = w + borders.left + borders.right;
                int targetH = h + borders.top + borders.bottom;

                // Clear old topmost flag first, then set as regular top window
                SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                SetWindowPos(hwnd, HWND_TOP, targetX, targetY, targetW, targetH, SWP_NOACTIVATE);
            }
        }

        // ===== RECORDING =====

        private void HandleRecordHotkey()
        {
            switch (_recordingPhase)
            {
                case RecordingPhase.Idle:
                    StartMaskEditing();
                    break;
                case RecordingPhase.MaskEditing:
                    StartRecording();
                    break;
            }
        }

        private void HandlePauseHotkey()
        {
            switch (_recordingPhase)
            {
                case RecordingPhase.Recording:
                    PauseRecording();
                    break;
                case RecordingPhase.Paused:
                    ResumeRecording();
                    break;
            }
        }

        private void HandleStopHotkey()
        {
            switch (_recordingPhase)
            {
                case RecordingPhase.MaskEditing:
                    CancelMaskEditing();
                    break;
                case RecordingPhase.Recording:
                case RecordingPhase.Paused:
                    ShowStopDialog();
                    break;
            }
        }

        private void ShowStopDialog()
        {
            if (_isStopDialogShowing) return;
            _isStopDialogShowing = true;

            var result = MessageBox.Show(
                "Yes — save recording\nNo — discard recording\nCancel — continue",
                "Stop Recording?",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            _isStopDialogShowing = false;

            switch (result)
            {
                case MessageBoxResult.Yes:
                    _ = StopRecording();
                    break;
                case MessageBoxResult.No:
                    DiscardRecording();
                    break;
            }
        }

        private void DiscardRecording()
        {
            _recorder?.Abort();

            _recordingBar?.Cleanup();
            _recordingBar?.Close();
            _recordingBar = null;
            _maskOverlay?.Close();
            _maskOverlay = null;
            _dimOverlay?.Close();
            _dimOverlay = null;

            _recordingPhase = RecordingPhase.Idle;
            _recorder = null;
            _overlay?.SetRecording(null);
            Status("Recording discarded.");
        }

        private void StartMaskEditing()
        {
            // Determine target window
            IntPtr targetHwnd = _focusedHwnd;
            if (targetHwnd == IntPtr.Zero && _targets.Count > 0)
                targetHwnd = _targets[0].Hwnd;
            if (targetHwnd == IntPtr.Zero)
            {
                Status("No windows available for recording.");
                return;
            }

            _recordingTargetHwnd = targetHwnd;
            var clientArea = Win32.GetClientAreaScreenRect(targetHwnd);
            _recX = clientArea.x;
            _recY = clientArea.y;
            _recW = clientArea.w;
            _recH = clientArea.h;

            if (_recW <= 0 || _recH <= 0)
            {
                Status("Cannot determine client area.");
                return;
            }

            // Get monitor info for dim overlay
            var monPx = Win32.GetMonitorRectPxForWindow(targetHwnd);

            // DPI scaling for WPF positioning
            var dpiSource = PresentationSource.FromVisual(this);
            double scaleX = dpiSource?.CompositionTarget?.TransformFromDevice.M11 ?? 1.0;
            double scaleY = dpiSource?.CompositionTarget?.TransformFromDevice.M22 ?? 1.0;

            // Show dim overlay
            _dimOverlay = new DimOverlayWindow();
            _dimOverlay.Show();
            _dimOverlay.SetCutout(monPx.x, monPx.y, monPx.w, monPx.h,
                                   _recX, _recY, _recW, _recH);

            // Show mask overlay over client area
            _maskOverlay = new MaskOverlayWindow();
            _maskOverlay.Left = _recX * scaleX;
            _maskOverlay.Top = _recY * scaleY;
            _maskOverlay.Width = _recW * scaleX;
            _maskOverlay.Height = _recH * scaleY;
            _maskOverlay.ConfirmMasks += (_, _) => StartRecording();
            _maskOverlay.CancelEditing += (_, _) => CancelMaskEditing();
            _maskOverlay.Show();
            _maskOverlay.Focus();

            _recordingPhase = RecordingPhase.MaskEditing;
            _overlay?.SetRecording("MASK");
            Status("LMB — draw mask. RMB — remove. Enter/F2 — record. Esc/F4 — cancel.");
        }

        private void StartRecording()
        {
            if (_maskOverlay == null) return;

            var ffmpegPath = FfmpegSetup.ResolveAndSave();
            if (ffmpegPath == null)
            {
                MessageBox.Show(
                    "ffmpeg not installed.\nRestart the app or specify path in Settings → Screen Recording.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CancelMaskEditing();
                return;
            }

            // Hide dim overlay (it shouldn't appear in recording)
            _dimOverlay?.Hide();

            // Switch mask overlay to click-through mode (masks stay visible but don't block input)
            _maskOverlay.EnterRecordingMode();

            // Start ffmpeg
            _recorder = new ScreenRecorder();
            if (!_recorder.Start(_recX, _recY, _recW, _recH,
                           AppSettings.Default.RecordingFps,
                           AppSettings.Default.RecordingFolder,
                           ffmpegPath))
            {
                MessageBox.Show(_recorder.LastError, "Recording Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CancelMaskEditing();
                return;
            }

            // Show recording bar (Game Bar style) — OUTSIDE the capture area
            var dpiSource = PresentationSource.FromVisual(this);
            double scaleX = dpiSource?.CompositionTarget?.TransformFromDevice.M11 ?? 1.0;
            double scaleY = dpiSource?.CompositionTarget?.TransformFromDevice.M22 ?? 1.0;

            _recordingBar = new RecordingBarWindow();
            _recordingBar.RequestPause += (_, _) => PauseRecording();
            _recordingBar.RequestResume += (_, _) => ResumeRecording();
            _recordingBar.RequestStop += (_, _) => ShowStopDialog();
            _recordingBar.Show();

            double barW = _recordingBar.ActualWidth;
            double barH = _recordingBar.ActualHeight;
            double capLeft = _recX * scaleX;
            double capTop = _recY * scaleY;
            double capRight = (_recX + _recW) * scaleX;
            double capBottom = (_recY + _recH) * scaleY;

            // Try above capture area first
            double barX = capLeft + (_recW * scaleX - barW) / 2;
            double barY = capTop - barH - 6;
            if (barY < 0)
            {
                // No room above — try below
                barY = capBottom + 6;
            }
            _recordingBar.Left = Math.Max(0, barX);
            _recordingBar.Top = barY;
            _recordingBar.StartTimer();

            _recordingPhase = RecordingPhase.Recording;
            _overlay?.SetRecording("REC");
            Status("Recording...");
        }

        private async void PauseRecording()
        {
            if (_recorder == null) return;
            await _recorder.StopCurrentSegment();

            _recordingPhase = RecordingPhase.Paused;
            _recordingBar?.SetPaused();
            _overlay?.SetRecording("PAUSED");

            // Enable mask editing during pause
            _maskOverlay?.EnterPauseEditMode();
            Status("Paused. Edit masks.");
        }

        private void ResumeRecording()
        {
            if (_recorder == null) return;

            // Disable mask editing before resuming capture
            _maskOverlay?.ReEnterRecordingMode();

            if (!_recorder.StartNewSegment())
            {
                MessageBox.Show(_recorder.LastError, "Recording Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _recordingPhase = RecordingPhase.Recording;
            _recordingBar?.SetResumed();
            _overlay?.SetRecording("REC");
            Status("Recording...");
        }

        private async Task StopRecording()
        {
            if (_recorder == null) return;

            // Show saving state on recording bar
            _recordingBar?.SetSaving();
            Status("Finalizing...");
            var finalPath = await _recorder.FinalizeRecording();

            // Cleanup overlays
            _recordingBar?.Cleanup();
            _recordingBar?.Close();
            _recordingBar = null;
            _maskOverlay?.Close();
            _maskOverlay = null;
            _dimOverlay?.Close();
            _dimOverlay = null;

            _recordingPhase = RecordingPhase.Idle;
            _recorder = null;
            _overlay?.SetRecording(null);

            if (!string.IsNullOrEmpty(finalPath))
                Status($"Saved: {finalPath}");
            else
                Status("Recording finished (no file created).");
        }

        private void CancelMaskEditing()
        {
            _recordingBar?.Cleanup();
            _recordingBar?.Close();
            _recordingBar = null;
            _maskOverlay?.Close();
            _maskOverlay = null;
            _dimOverlay?.Close();
            _dimOverlay = null;

            _recordingPhase = RecordingPhase.Idle;
            _overlay?.SetRecording(null);
            Status("Recording cancelled.");
        }

        private void Status(string s) => StatusText.Text = s;
    }
}