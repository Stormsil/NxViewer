using System.Diagnostics;
using NxTiler.Infrastructure.Recording;

namespace NxTiler.Tests;

public sealed class FfmpegProcessSupportTests
{
    [Fact]
    public void ProcessStderrTail_Append_TrimsToConfiguredSize()
    {
        var tail = new ProcessStderrTail(maxLines: 2);

        tail.Append("line-1");
        tail.Append("line-2");
        tail.Append("line-3");

        var snapshot = tail.Snapshot();
        Assert.DoesNotContain("line-1", snapshot);
        Assert.Contains("line-2", snapshot);
        Assert.Contains("line-3", snapshot);
    }

    [Fact]
    public async Task ProcessExitAwaiter_ReturnsFalse_OnTimeout()
    {
        using var process = StartProcess("-NoProfile -Command \"Start-Sleep -Seconds 2\"");

        var completed = await ProcessExitAwaiter.WaitForExitAsync(
            process,
            timeoutMs: 50,
            CancellationToken.None);

        Assert.False(completed);

        if (!process.HasExited)
        {
            process.Kill();
            process.WaitForExit();
        }
    }

    [Fact]
    public async Task ProcessExitAwaiter_ReturnsTrue_WhenProcessExits()
    {
        using var process = StartProcess("-NoProfile -Command \"exit 0\"");

        var completed = await ProcessExitAwaiter.WaitForExitAsync(
            process,
            timeoutMs: 3000,
            CancellationToken.None);

        Assert.True(completed);
    }

    [Fact]
    public async Task ProcessStderrTail_Attach_CapturesProcessErrorStream()
    {
        var tail = new ProcessStderrTail(maxLines: 10);
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-NoProfile -Command \"[Console]::Error.WriteLine('err-alpha'); [Console]::Error.WriteLine('err-beta')\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            },
        };

        tail.Attach(process);
        process.Start();
        process.BeginErrorReadLine();
        await ProcessExitAwaiter.WaitForExitAsync(process, 3000, CancellationToken.None);
        process.WaitForExit();

        var snapshot = tail.Snapshot();
        Assert.Contains("err-alpha", snapshot);
        Assert.Contains("err-beta", snapshot);
    }

    private static Process StartProcess(string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        return process;
    }
}
