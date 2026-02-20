# Task Completion Checklist

- Run unit tests: `dotnet test .\NxTiler.sln -c Debug`
- If recording/overlay behavior was touched: smoke-test the hotkey workflow end-to-end (start, pause, resume, stop, discard) on a multi-monitor setup.
- Check logs under `%LOCALAPPDATA%\NxTiler\Logs` for runtime errors.
