using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;             // ContextMenu, MenuItem
using System.Windows.Interop;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;   // NuGet: Hardcodet.NotifyIcon.Wpf
using Ookii.Dialogs.Wpf;
using static NxTiler.Win32;
using System.Windows.Media;

namespace NxTiler
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(800) };
        private readonly List<TargetWindow> _targets = new();
        private OverlayWindow? _overlay;

        // tray
        private TaskbarIcon? _tray;
        private MenuItem? _trayAutoItem;

        private HwndSource? _src;
        private const int HOTKEY_OVERLAY_ID = 100; // F1
        private const uint VK_F1 = 0x70;
        
        private const int HOTKEY_MAIN_ID = 101; // Ctrl + F1

        private readonly int _selfPid = System.Diagnostics.Process.GetCurrentProcess().Id;
        private bool _isRealExit = false; // Flag to allow real exit

        public MainWindow()
        {
            InitializeComponent();

            _timer.Tick += (_, __) => AutoTick();

            StateChanged += MainWindow_StateChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _src = (HwndSource)PresentationSource.FromVisual(this)!;
            _src.AddHook(WndProc);
            
            // F1 -> Overlay
            RegisterHotKey(_src.Handle, HOTKEY_OVERLAY_ID, 0, VK_F1);
            // Ctrl + F1 -> Toggle Main Window
            RegisterHotKey(_src.Handle, HOTKEY_MAIN_ID, MOD_CONTROL, VK_F1);

            CreateTray(); 
            RefreshWindows();
        }

        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            if (!_isRealExit)
            {
                e.Cancel = true;
                Hide();
                ShowInTaskbar = false;
                Status("Свернуто в трей. Используйте меню трея для выхода.");
            }
            else
            {
                // Real exit cleanup
                try 
                { 
                    if (_src != null) 
                    {
                        UnregisterHotKey(_src.Handle, HOTKEY_OVERLAY_ID);
                        UnregisterHotKey(_src.Handle, HOTKEY_MAIN_ID);
                    }
                } 
                catch { }
                _overlay?.Close();
                _tray?.Dispose();
            }
        }

        // ===== Tray =====
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

            // контекстное меню
            var menu = new ContextMenu();
            var mShow = new MenuItem { Header = "Показать" }; mShow.Click += (_, __) => RestoreFromTray();
            var mArrange = new MenuItem { Header = "Разложить сейчас" }; mArrange.Click += (_, __) => ArrangeNow();
            _trayAutoItem = new MenuItem { Header = "Автораскладка", IsCheckable = true, IsChecked = AutoArrangeCheck.IsChecked == true };
            _trayAutoItem.Click += (_, __) => AutoArrangeCheck.IsChecked = _trayAutoItem.IsChecked;
            var mExit = new MenuItem { Header = "Выход" }; 
            mExit.Click += (_, __) => 
            {
                _isRealExit = true;
                Close();
            };

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
            if (Visibility == Visibility.Visible)
            {
                Hide();
                ShowInTaskbar = false;
            }
            else
            {
                RestoreFromTray();
            }
        }

        // ===== Hotkey =====
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == HOTKEY_OVERLAY_ID)
                {
                    ToggleOverlay();
                    handled = true;
                }
                else if (id == HOTKEY_MAIN_ID)
                {
                    ToggleMainWindow();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void ToggleOverlay()
        {
            if (_overlay == null)
            {
                _overlay = new OverlayWindow { Owner = this };
                _overlay.RequestArrange += (_, __) => ArrangeNow();
                _overlay.RequestToggleAuto += (_, on) => AutoArrangeCheck.IsChecked = on;
                _overlay.RequestClose += (_, __) => _overlay?.Hide();

                _overlay.Left = SystemParameters.WorkArea.Left + 20;
                _overlay.Top = SystemParameters.WorkArea.Top + 20;
                _overlay.Show();
                _overlay.SetAuto(AutoArrangeCheck.IsChecked == true);
            }
            else
            {
                if (_overlay.IsVisible) _overlay.Hide(); else _overlay.Show();
                _overlay.SetAuto(AutoArrangeCheck.IsChecked == true);
            }
        }

        // ===== UI =====
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var win = new SettingsWindow { Owner = this };
            win.ShowDialog();
            ArrangeNow();
        }

        private async void OpenSelected_Click(object sender, RoutedEventArgs e)
        {
            var sessions = NomachineLauncher.FindSessions(AppSettings.Default.NxsFolder, AppSettings.Default.NameFilter);
            
            var disabled = new HashSet<string>(AppSettings.Default.DisabledFiles.Cast<string>(), StringComparer.OrdinalIgnoreCase);
            sessions = sessions.Where(s => !disabled.Contains(s.name)).ToList();

            if (sessions.Count == 0) { Status("Подходящих (и включенных) .nxs файлов не найдено."); return; }

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
            Status(AutoArrangeCheck.IsChecked == true ? "Автораскладка: ВКЛ" : "Автораскладка: ВЫКЛ");
        }

        private void ArrangeNow_Click(object sender, RoutedEventArgs e) => ArrangeNow();

        // ===== Logic =====
        private void AutoTick()
        {
            if (AppSettings.Default.SuspendOnMax && _targets.Any(t => t.IsMaximized))
            {
                Status("Пауза: окно развернуто на весь экран.");
                return;
            }
            ArrangeNow();
        }

        private void RefreshWindows(List<string>? preferNames = null)
        {
            _targets.Clear();

            Regex? reTitle = string.IsNullOrWhiteSpace(AppSettings.Default.TitleFilter) ? null
                             : new Regex(AppSettings.Default.TitleFilter, RegexOptions.IgnoreCase);
            Regex? reName = string.IsNullOrWhiteSpace(AppSettings.Default.NameFilter) ? null
                             : new Regex(AppSettings.Default.NameFilter, RegexOptions.IgnoreCase);

            var temp = new List<TargetWindow>();
            EnumWindows((h, l) =>
            {
                if (!IsWindowVisible(h)) return true;

                var title = Win32.GetWindowText(h);
                if (string.IsNullOrWhiteSpace(title)) return true;

                GetWindowThreadProcessId(h, out uint pid);
                if (pid == (uint)_selfPid) return true;

                if (!title.Contains("NoMachine", StringComparison.OrdinalIgnoreCase))
                    return true;

                var sessionName = ExtractSessionNameFromTitle(title);
                if (string.IsNullOrWhiteSpace(sessionName)) return true;

                if (sessionName.Equals("NoMachine", StringComparison.OrdinalIgnoreCase)) return true;

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
                ? chosen.OrderByDescending(t => ParseNumericSuffix(t.SourceName))
                        .ThenByDescending(t => t.SourceName, StringComparer.OrdinalIgnoreCase)
                : chosen.OrderBy(t => ParseNumericSuffix(t.SourceName))
                        .ThenBy(t => t.SourceName, StringComparer.OrdinalIgnoreCase);

            _targets.AddRange(ordered);

            WindowsList.ItemsSource = _targets.Select(t => new
            {
                t.HwndHex,
                Title = t.Title,
                t.State,
                Source = t.SourceName
            }).ToList();

            Status($"Найдено окон: {_targets.Count}");
        }

        private static string ExtractSessionNameFromTitle(string title)
        {
            var idx1 = title.IndexOf(" - NoMachine", StringComparison.OrdinalIgnoreCase);
            if (idx1 > 0) return title[..idx1].Trim();

            const string token = "NoMachine - ";
            var idx2 = title.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (idx2 >= 0 && idx2 + token.Length < title.Length)
                return title[(idx2 + token.Length)..].Trim();

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
            RefreshWindows();

            var candidates = _targets.Where(t => !t.IsMaximized).ToList();
            if (candidates.Count == 0)
            {
                Status("Нечего раскладывать.");
                return;
            }

            var waPx = Win32.GetWorkAreaPxForWindow(candidates[0].Hwnd);
            int pxLeft = waPx.x;
            int pxTop = waPx.y;
            int pxWidth = waPx.w;
            int pxHeight = waPx.h;

            int topPad = AppSettings.Default.TopPad;
            int effTop = pxTop + topPad;
            int effHeight = Math.Max(1, pxHeight - topPad);

            int rows, cols;
            if (candidates.Count is >= 6 and <= 10)
            {
                rows = 2;
                cols = (int)Math.Ceiling(candidates.Count / 2.0);
            }
            else
            {
                (rows, cols) = Tiler.ComputeGrid(candidates.Count, pxWidth, effHeight);
            }

            int gap = AppSettings.Default.Gap;

            int totalGapW = (cols - 1) * gap;
            int totalGapH = (rows - 1) * gap;
            int baseW = (pxWidth - totalGapW) / cols;
            int baseH = (effHeight - totalGapH) / rows;
            int remW = (pxWidth - totalGapW) % cols;
            int remH = (effHeight - totalGapH) % rows;

            var cells = new List<(int x, int y, int w, int h)>(candidates.Count);
            int y = effTop;
            for (int r = 0; r < rows; r++)
            {
                int h = baseH + (r < remH ? 1 : 0);
                int x = pxLeft;
                for (int c = 0; c < cols; c++)
                {
                    int w = baseW + (c < remW ? 1 : 0);
                    if (cells.Count < candidates.Count)
                        cells.Add((x, y, w, h));
                    x += w + gap;
                }
                y += h + gap;
            }

            for (int i = 0; i < candidates.Count; i++)
            {
                var wnd = candidates[i];
                var cell = cells[i];

                // FIX: Strictly map window frames to cells to prevent overlap.
                // We use cell dimensions directly, ignoring client/non-client adjustments.
                // This aligns the window's OUTER frame to the cell grid.
                // The visual content will be slightly smaller than the cell due to invisible borders.
                
                int x0 = cell.x;
                int y0 = cell.y;
                int w0 = cell.w;
                int h0 = cell.h;

                // Adjust Y to prevent going above allowed top padding (though cell.y handles this already)
                if (y0 < effTop) y0 = effTop;

                ShowWindow(wnd.Hwnd, SW_SHOWNORMAL);
                SetWindowPos(wnd.Hwnd, HWND_TOP, x0, y0, w0, h0, SWP_NOZORDER | SWP_NOACTIVATE);
            }

            Status($"Разложено: {candidates.Count} окон ({rows}x{cols}), gap={gap}px.");
        }

        private void Status(string s) => StatusText.Text = s;
    }
}
