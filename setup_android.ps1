# Quick Android Tools Setup for Unity
Write-Host "=== Quick Android Tools Setup ===" -ForegroundColor Cyan
Write-Host ""

# Find Unity Hub
$unityHubPath = $null
$hubPaths = @(
    "${env:ProgramFiles}\Unity Hub\Unity Hub.exe",
    "${env:ProgramFiles(x86)}\Unity Hub\Unity Hub.exe",
    "${env:LOCALAPPDATA}\Programs\Unity Hub\Unity Hub.exe"
)

foreach ($path in $hubPaths) {
    if (Test-Path $path) {
        $unityHubPath = $path
        Write-Host "Found Unity Hub: $unityHubPath" -ForegroundColor Green
        break
    }
}

if (-not $unityHubPath) {
    Write-Host "Unity Hub not found!" -ForegroundColor Red
    Write-Host "Please install Unity Hub first: https://unity.com/download" -ForegroundColor Yellow
    exit 1
}

# Android SDK default location
$sdkPath = "$env:LOCALAPPDATA\Android\Sdk"
$buildToolsPath = "$sdkPath\build-tools\30.0.3"

Write-Host ""
Write-Host "=== Installation Method ===" -ForegroundColor Cyan
Write-Host "RECOMMENDED: Install via Unity Hub" -ForegroundColor Green
Write-Host ""
Write-Host "Steps:" -ForegroundColor Yellow
Write-Host "1. Open Unity Hub" -ForegroundColor White
Write-Host "2. Installs > Add modules to your Unity version" -ForegroundColor White
Write-Host "3. Check `'Android Build Support`'" -ForegroundColor White
Write-Host "4. Install" -ForegroundColor White
Write-Host ""

$response = Read-Host "Open Unity Hub now? (Y/N)"
if ($response -eq "Y" -or $response -eq "y") {
    Start-Process $unityHubPath
    Write-Host "Unity Hub opened!" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Checking Android SDK ===" -ForegroundColor Cyan

if (Test-Path $sdkPath) {
    Write-Host "Android SDK location: $sdkPath" -ForegroundColor Green
    
    if (Test-Path $buildToolsPath) {
        Write-Host "Build Tools 30.0.3: Found" -ForegroundColor Green
    } else {
        Write-Host "Build Tools 30.0.3: Not found" -ForegroundColor Yellow
        Write-Host "Install via Android Studio SDK Manager or Unity Hub" -ForegroundColor Yellow
    }
} else {
    Write-Host "Android SDK will be installed at: $sdkPath" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Configure Unity ===" -ForegroundColor Cyan
Write-Host "After installation, set paths in Unity:" -ForegroundColor Yellow
Write-Host "Edit > Preferences > External Tools" -ForegroundColor White
Write-Host "Android SDK: $sdkPath" -ForegroundColor White

Write-Host ""
Write-Host "=== Done ===" -ForegroundColor Green
