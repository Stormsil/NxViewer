param(
    [string]$SolutionPath = "NxTiler.sln",
    [string]$TestProjectPath = "tests/NxTiler.Tests/NxTiler.Tests.csproj"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-PerfRun {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Configuration
    )

    $scriptPath = ".\scripts\perf\perf-smoke.ps1"
    & powershell -ExecutionPolicy Bypass -File $scriptPath `
        -Configuration $Configuration `
        -SolutionPath $SolutionPath `
        -TestProjectPath $TestProjectPath `
        -SummaryFileName "perf-smoke-$($Configuration.ToLowerInvariant())-latest.json"
}

Write-Host "Running NxTiler regression performance matrix (Debug + Release)..."
Invoke-PerfRun -Configuration "Debug"
Invoke-PerfRun -Configuration "Release"
Write-Host "Regression performance matrix completed."
