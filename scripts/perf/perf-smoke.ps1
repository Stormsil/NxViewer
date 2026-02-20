param(
    [string]$Configuration = "Release",
    [string]$SolutionPath = "NxTiler.sln",
    [string]$TestProjectPath = "tests/NxTiler.Tests/NxTiler.Tests.csproj",
    [string]$SummaryFileName = "",
    [int]$BuildThresholdSeconds = 120,
    [int]$TestThresholdSeconds = 40,
    [int]$TotalThresholdSeconds = 160
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-TimedCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Command
    )

    $start = Get-Date
    & powershell -NoProfile -Command $Command | Out-Host
    $exitCode = $LASTEXITCODE
    $end = Get-Date
    $duration = [math]::Round(($end - $start).TotalSeconds, 2)

    return [pscustomobject]@{
        Command = $Command
        ExitCode = $exitCode
        DurationSeconds = $duration
        StartedAtUtc = $start.ToUniversalTime().ToString("o")
        FinishedAtUtc = $end.ToUniversalTime().ToString("o")
    }
}

Write-Host "Running NxTiler perf smoke checks..."
$build = Invoke-TimedCommand -Command "dotnet build `"$SolutionPath`" -c $Configuration"
if ($build.ExitCode -ne 0) {
    throw "Build failed with exit code $($build.ExitCode)."
}

$test = Invoke-TimedCommand -Command "dotnet test `"$TestProjectPath`" -c $Configuration --no-build"
if ($test.ExitCode -ne 0) {
    throw "Tests failed with exit code $($test.ExitCode)."
}

$totalSeconds = [math]::Round($build.DurationSeconds + $test.DurationSeconds, 2)
$summary = [pscustomobject]@{
    TimestampUtc = (Get-Date).ToUniversalTime().ToString("o")
    Configuration = $Configuration
    Build = $build
    Test = $test
    TotalSeconds = $totalSeconds
    Thresholds = [pscustomobject]@{
        BuildThresholdSeconds = $BuildThresholdSeconds
        TestThresholdSeconds = $TestThresholdSeconds
        TotalThresholdSeconds = $TotalThresholdSeconds
    }
}

$artifactsDir = Join-Path -Path "artifacts" -ChildPath "perf"
New-Item -ItemType Directory -Path $artifactsDir -Force | Out-Null
if ([string]::IsNullOrWhiteSpace($SummaryFileName)) {
    $SummaryFileName = "perf-smoke-$($Configuration.ToLowerInvariant())-latest.json"
}

$summaryPath = Join-Path -Path $artifactsDir -ChildPath $SummaryFileName
$summary | ConvertTo-Json -Depth 8 | Set-Content -Path $summaryPath -Encoding UTF8

Write-Host "Build duration: $($build.DurationSeconds)s (threshold $BuildThresholdSeconds s)"
Write-Host "Test duration: $($test.DurationSeconds)s (threshold $TestThresholdSeconds s)"
Write-Host "Total duration: $totalSeconds s (threshold $TotalThresholdSeconds s)"
Write-Host "Saved summary: $summaryPath"

if ($build.DurationSeconds -gt $BuildThresholdSeconds) {
    throw "Build duration exceeded threshold."
}

if ($test.DurationSeconds -gt $TestThresholdSeconds) {
    throw "Test duration exceeded threshold."
}

if ($totalSeconds -gt $TotalThresholdSeconds) {
    throw "Total duration exceeded threshold."
}

Write-Host "Perf smoke checks passed."
