param(
    [string]$SolutionPath = "NxTiler.sln"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$canonicalProjects = @(
    "src\NxTiler.App\NxTiler.App.csproj",
    "src\NxTiler.Application\NxTiler.Application.csproj",
    "src\NxTiler.Domain\NxTiler.Domain.csproj",
    "src\NxTiler.Infrastructure\NxTiler.Infrastructure.csproj",
    "src\WindowCaptureCL\WindowCaptureCL.csproj",
    "src\WindowManagerCL\WindowManagerCL\WindowManagerCL.csproj",
    "src\ImageSearchCL\ImageSearchCL.csproj",
    "src\ImageSearchCL\ImageSearchCL.WindowCapture\ImageSearchCL.WindowCapture.csproj",
    "tests\NxTiler.Tests\NxTiler.Tests.csproj"
)

Write-Host "Validating canonical project track..."

$slnOutput = dotnet sln $SolutionPath list
if ($LASTEXITCODE -ne 0) {
    throw "Failed to list projects from solution '$SolutionPath'."
}

$normalizedSlnLines = $slnOutput | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }

if ($normalizedSlnLines -contains "NxTiler.csproj") {
    throw "Legacy root project 'NxTiler.csproj' must not be included in '$SolutionPath'."
}

foreach ($project in $canonicalProjects) {
    if (-not ($normalizedSlnLines -contains $project)) {
        throw "Canonical project missing from solution: $project"
    }
}

$allProjectFiles = Get-ChildItem -Recurse -Filter *.csproj | Select-Object -ExpandProperty FullName
foreach ($projectFile in $allProjectFiles) {
    $content = Get-Content $projectFile -Raw
    if ($content -match "NxTiler\.csproj") {
        throw "Project '$projectFile' references legacy root project 'NxTiler.csproj'."
    }
}

if (-not (Test-Path "Directory.Build.targets")) {
    throw "Missing 'Directory.Build.targets' required for legacy freeze warning."
}

$targetsContent = Get-Content "Directory.Build.targets" -Raw
if ($targetsContent -notmatch "NXLEGACY001") {
    throw "Legacy freeze warning code 'NXLEGACY001' is missing from Directory.Build.targets."
}

Write-Host "Canonical track validation passed."
