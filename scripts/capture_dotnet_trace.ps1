param(
    [string]$ProcessName = "TerrariaServer",
    [int]$DurationSeconds = 120,
    [string]$OutputDirectory = "profiles"
)

$traceTool = Get-Command dotnet-trace -ErrorAction SilentlyContinue
if ($null -eq $traceTool)
{
    Write-Error "dotnet-trace is not installed. Install with: dotnet tool install -g dotnet-trace"
    exit 1
}

if ($DurationSeconds -le 0)
{
    Write-Error "DurationSeconds must be greater than zero."
    exit 1
}

$targetProcess = Get-Process -Name $ProcessName -ErrorAction SilentlyContinue |
    Sort-Object CPU -Descending |
    Select-Object -First 1

if ($null -eq $targetProcess)
{
    Write-Error "No running process found with name '$ProcessName'."
    exit 1
}

$duration = [TimeSpan]::FromSeconds($DurationSeconds)
$durationArg = "{0:00}:{1:00}:{2:00}:{3:00}" -f $duration.Days, $duration.Hours, $duration.Minutes, $duration.Seconds

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$baseName = "{0}_{1}" -f $ProcessName, $stamp
$outputPath = Join-Path $OutputDirectory ("{0}.nettrace" -f $baseName)
$speedscopePath = Join-Path $OutputDirectory ("{0}.speedscope.json" -f $baseName)

Write-Host ("Collecting CPU trace for PID {0} ({1}) for {2} seconds..." -f $targetProcess.Id, $targetProcess.ProcessName, $DurationSeconds)

dotnet-trace collect --process-id $targetProcess.Id --duration $durationArg --profile cpu-sampling --output $outputPath --format Speedscope
if ($LASTEXITCODE -ne 0)
{
    exit $LASTEXITCODE
}

Write-Host ("NetTrace: {0}" -f $outputPath)
if (Test-Path $speedscopePath)
{
    Write-Host ("Speedscope: {0}" -f $speedscopePath)
}
