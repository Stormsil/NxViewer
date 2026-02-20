using System;
using System.Collections.Generic;
using System.Windows.Threading;
using static NxTiler.Win32;

namespace NxTiler
{
    internal sealed class WindowEventMonitor : IDisposable
    {
        public event EventHandler? ArrangeNeeded;
        public event EventHandler<IntPtr>? ForegroundChanged;

        private readonly HashSet<IntPtr> _trackedHwnds = new();
        private readonly int _selfPid;

        // Store delegate as field to prevent GC collection
        private readonly WinEventDelegate _winEventProc;
        private readonly List<IntPtr> _hooks = new();

        // Debounce for LOCATIONCHANGE
        private readonly DispatcherTimer _locationDebounce;
        private bool _locationPending;

        public WindowEventMonitor(int selfPid)
        {
            _selfPid = selfPid;
            _winEventProc = OnWinEvent;

            _locationDebounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            _locationDebounce.Tick += (_, _) =>
            {
                _locationDebounce.Stop();
                if (_locationPending)
                {
                    _locationPending = false;
                    ArrangeNeeded?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public void Start()
        {
            uint flags = WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS;

            _hooks.Add(SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventProc, 0, 0, flags));
            _hooks.Add(SetWinEventHook(EVENT_OBJECT_SHOW, EVENT_OBJECT_SHOW, IntPtr.Zero, _winEventProc, 0, 0, flags));
            _hooks.Add(SetWinEventHook(EVENT_OBJECT_HIDE, EVENT_OBJECT_HIDE, IntPtr.Zero, _winEventProc, 0, 0, flags));
            _hooks.Add(SetWinEventHook(EVENT_OBJECT_DESTROY, EVENT_OBJECT_DESTROY, IntPtr.Zero, _winEventProc, 0, 0, flags));
            _hooks.Add(SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE, IntPtr.Zero, _winEventProc, 0, 0, flags));
            _hooks.Add(SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, _winEventProc, 0, 0, flags));
        }

        public void Stop()
        {
            _locationDebounce.Stop();
            foreach (var h in _hooks)
            {
                if (h != IntPtr.Zero) UnhookWinEvent(h);
            }
            _hooks.Clear();
        }

        public void UpdateTrackedWindows(IEnumerable<IntPtr> hwnds)
        {
            _trackedHwnds.Clear();
            foreach (var h in hwnds) _trackedHwnds.Add(h);
        }

        public bool IsTracked(IntPtr hwnd) => _trackedHwnds.Contains(hwnd);

        private void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // Only care about top-level window events (idObject == 0)
            if (idObject != 0) return;

            // Skip our own process windows
            GetWindowThreadProcessId(hwnd, out uint pid);
            if (pid == (uint)_selfPid) return;

            switch (eventType)
            {
                case EVENT_SYSTEM_FOREGROUND:
                    ForegroundChanged?.Invoke(this, hwnd);
                    // If user switched to a tracked window, arrange
                    if (_trackedHwnds.Contains(hwnd))
                        ArrangeNeeded?.Invoke(this, EventArgs.Empty);
                    break;

                case EVENT_OBJECT_LOCATIONCHANGE:
                    // Only debounce for tracked windows
                    if (_trackedHwnds.Contains(hwnd))
                    {
                        _locationPending = true;
                        _locationDebounce.Stop();
                        _locationDebounce.Start();
                    }
                    break;

                case EVENT_OBJECT_SHOW:
                case EVENT_OBJECT_NAMECHANGE:
                    // A new window appeared or title changed — might be a new NoMachine session
                    var title = Win32.GetWindowText(hwnd);
                    if (title.Contains("NoMachine", StringComparison.OrdinalIgnoreCase))
                        ArrangeNeeded?.Invoke(this, EventArgs.Empty);
                    break;

                case EVENT_OBJECT_HIDE:
                case EVENT_OBJECT_DESTROY:
                    if (_trackedHwnds.Contains(hwnd))
                        ArrangeNeeded?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
