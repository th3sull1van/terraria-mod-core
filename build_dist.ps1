# TerrariaModCore Distribution Build Script
$ErrorActionPreference = "Stop"

$workspace = if ($PSScriptRoot) { $PSScriptRoot } else { Get-Location }
$dist = Join-Path $workspace "dist"

Write-Host "Building TerrariaModCore Solution (Release x86)..." -ForegroundColor Cyan

# Locate MSBuild
$msbuild = $null
$possiblePaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Msbuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
)
foreach ($p in $possiblePaths) {
    if (Test-Path $p) {
        $msbuild = $p
        break
    }
}

if ($msbuild) {
    & $msbuild "$workspace\TerrariaModCore.sln" /p:Configuration=Release /p:Platform="x86" /v:m
} else {
    dotnet build "$workspace\TerrariaModCore.sln" -c Release -p:Platform="x86" -v m
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE! Halting distribution packaging."
    exit $LASTEXITCODE
}

Write-Host "Assembling Distribution in $dist..." -ForegroundColor Cyan
if (Test-Path $dist) {
    Remove-Item $dist -Recurse -Force
}

# Create Folder Structure
New-Item -ItemType Directory -Path "$dist\TMC\config" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\TMC\logs" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\OreCascade" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\AutoFishing" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\FishingLinePlus" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\TurboExtractinator" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\AutoBuff" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\AutoOpen" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\AutoResearch" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\PiggyVault" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\TurboBucket" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\BossCursor" -Force | Out-Null
New-Item -ItemType Directory -Path "$dist\mods\BossCursor\UI" -Force | Out-Null

# Copy Launcher (TerrariaModded.exe)
Copy-Item "$workspace\src\TerrariaModCore.Launcher\bin\Release\TerrariaModded.exe" "$dist\" -Force
if (Test-Path "$workspace\src\TerrariaModCore.Launcher\bin\Release\TerrariaModded.pdb") {
    Copy-Item "$workspace\src\TerrariaModCore.Launcher\bin\Release\TerrariaModded.pdb" "$dist\" -Force
}
if (Test-Path "$workspace\src\TerrariaModCore.Launcher\bin\Release\TerrariaModded.exe.config") {
    Copy-Item "$workspace\src\TerrariaModCore.Launcher\bin\Release\TerrariaModded.exe.config" "$dist\" -Force
}

# Apply Large Address Aware (LAA 0x0020) flag to TerrariaModded.exe (enables 4GB memory on 64-bit OS)
function Enable-LAA ($exePath) {
    if (Test-Path $exePath) {
        $bytes = [System.IO.File]::ReadAllBytes($exePath)
        $peOffset = [System.BitConverter]::ToInt32($bytes, 0x3C)
        $charOffset = $peOffset + 4 + 18
        $characteristics = [System.BitConverter]::ToUInt16($bytes, $charOffset)
        $newChar = $characteristics -bor 0x0020
        $newBytes = [System.BitConverter]::GetBytes([uint16]$newChar)
        $bytes[$charOffset] = $newBytes[0]
        $bytes[$charOffset + 1] = $newBytes[1]
        [System.IO.File]::WriteAllBytes($exePath, $bytes)
    }
}
Enable-LAA "$dist\TerrariaModded.exe"
Enable-LAA "$workspace\src\TerrariaModCore.Launcher\bin\Release\TerrariaModded.exe"

# Copy TMC Core & Dependencies
Copy-Item "$workspace\src\TerrariaModCore\bin\Release\TerrariaModCore.dll" "$dist\TMC\" -Force
Copy-Item "$workspace\src\TerrariaModCore\bin\Release\TerrariaModCore.pdb" "$dist\TMC\" -Force
Copy-Item "$workspace\src\TerrariaModCore.API\bin\Release\TerrariaModCore.API.dll" "$dist\TMC\" -Force
Copy-Item "$workspace\src\TerrariaModCore.API\bin\Release\TerrariaModCore.API.pdb" "$dist\TMC\" -Force
Copy-Item "$workspace\packages\Lib.Harmony.2.4.2\lib\net48\0Harmony.dll" "$dist\TMC\" -Force
Copy-Item "$workspace\packages\Lib.Harmony.2.4.2\lib\net48\0Harmony.dll" "$dist\" -Force

# Create Default Core Config
$coreConfig = @'
{
  "LogLevel": "Info",
  "DiagnosticBannerOnStartup": true,
  "StrictCompatibilityCheck": true,
  "SafeModeOnModFailure": true,
  "ModsDirectoryName": "mods"
}
'@
Set-Content -Path "$dist\TMC\config\core.json" -Value $coreConfig -Encoding UTF8

# Copy OreCascade
Copy-Item "$workspace\src\mods\OreCascade\bin\Release\OreCascade.dll" "$dist\mods\OreCascade\" -Force
Copy-Item "$workspace\src\mods\OreCascade\bin\Release\OreCascade.pdb" "$dist\mods\OreCascade\" -Force
Copy-Item "$workspace\src\mods\OreCascade\manifest.json" "$dist\mods\OreCascade\" -Force
$oreConfig = @'
{
  "Enabled": true,
  "MaxBlocksPerActivation": 100,
  "AllowDiagonalConnections": false,
  "RequireSameOreType": true,
  "IncludeGems": true,
  "IncludeExtractables": true
}
'@
Set-Content -Path "$dist\mods\OreCascade\config.json" -Value $oreConfig -Encoding UTF8

# Copy AutoFishing
Copy-Item "$workspace\src\mods\AutoFishing\bin\Release\AutoFishing.dll" "$dist\mods\AutoFishing\" -Force
Copy-Item "$workspace\src\mods\AutoFishing\bin\Release\AutoFishing.pdb" "$dist\mods\AutoFishing\" -Force
Copy-Item "$workspace\src\mods\AutoFishing\manifest.json" "$dist\mods\AutoFishing\" -Force
$autoFishingConfig = @'
{
  "Enabled": true,
  "AutoCast": true,
  "AutoReel": true,
  "CastDelayTicks": 30,
  "ReelDelayTicks": 2,
  "RequireBait": true
}
'@
Set-Content -Path "$dist\mods\AutoFishing\config.json" -Value $autoFishingConfig -Encoding UTF8

# Copy FishingLinePlus
Copy-Item "$workspace\src\mods\FishingLinePlus\bin\Release\FishingLinePlus.dll" "$dist\mods\FishingLinePlus\" -Force
Copy-Item "$workspace\src\mods\FishingLinePlus\bin\Release\FishingLinePlus.pdb" "$dist\mods\FishingLinePlus\" -Force
Copy-Item "$workspace\src\mods\FishingLinePlus\manifest.json" "$dist\mods\FishingLinePlus\" -Force
$fishingLineConfig = @'
{
  "Enabled": true,
  "MaxActiveFishingLines": 4,
  "LinesPerCast": 4,
  "SpreadAngleDegrees": 7.0,
  "VelocitySpread": 0.08
}
'@
Set-Content -Path "$dist\mods\FishingLinePlus\config.json" -Value $fishingLineConfig -Encoding UTF8

# Copy TurboExtractinator
Copy-Item "$workspace\src\mods\TurboExtractinator\bin\Release\TurboExtractinator.dll" "$dist\mods\TurboExtractinator\" -Force
Copy-Item "$workspace\src\mods\TurboExtractinator\bin\Release\TurboExtractinator.pdb" "$dist\mods\TurboExtractinator\" -Force
Copy-Item "$workspace\src\mods\TurboExtractinator\manifest.json" "$dist\mods\TurboExtractinator\" -Force
$turboConfig = @'
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsChlorophyteExtractinator": true,
  "BatchExtractionSize": 1
}
'@
Set-Content -Path "$dist\mods\TurboExtractinator\config.json" -Value $turboConfig -Encoding UTF8

# Copy AutoBuff
Copy-Item "$workspace\src\mods\AutoBuff\bin\Release\AutoBuff.dll" "$dist\mods\AutoBuff\" -Force
Copy-Item "$workspace\src\mods\AutoBuff\bin\Release\AutoBuff.pdb" "$dist\mods\AutoBuff\" -Force
Copy-Item "$workspace\src\mods\AutoBuff\manifest.json" "$dist\mods\AutoBuff\" -Force
$autoBuffConfig = @'
{
  "Enabled": true,
  "CheckIntervalTicks": 15,
  "IncludeFood": true,
  "IncludeFlasks": true,
  "IncludeVoidBag": true,
  "IncludePiggyBank": true,
  "MinBuffTimeThresholdTicks": 0,
  "ExcludedBuffIds": [
    18,
    119,
    120
  ],
  "ExcludedItemIds": [
    1344,
    2756
  ]
}
'@
Set-Content -Path "$dist\mods\AutoBuff\config.json" -Value $autoBuffConfig -Encoding UTF8

# Copy AutoOpen
Copy-Item "$workspace\src\mods\AutoOpen\bin\Release\AutoOpen.dll" "$dist\mods\AutoOpen\" -Force
Copy-Item "$workspace\src\mods\AutoOpen\bin\Release\AutoOpen.pdb" "$dist\mods\AutoOpen\" -Force
Copy-Item "$workspace\src\mods\AutoOpen\manifest.json" "$dist\mods\AutoOpen\" -Force
$autoOpenConfig = @'
{
  "Enabled": true,
  "RapidRightClickOpen": true,
  "OpenDelayTicks": 3,
  "BatchSize": 1,
  "PlaySound": true,
  "AutoOpenInventory": false,
  "AutoOpenIntervalTicks": 10,
  "IncludeVoidBag": true,
  "ExcludedItemIds": []
}
'@
Set-Content -Path "$dist\mods\AutoOpen\config.json" -Value $autoOpenConfig -Encoding UTF8

# Copy AutoResearch
Copy-Item "$workspace\src\mods\AutoResearch\bin\Release\AutoResearch.dll" "$dist\mods\AutoResearch\" -Force
Copy-Item "$workspace\src\mods\AutoResearch\bin\Release\AutoResearch.pdb" "$dist\mods\AutoResearch\" -Force
Copy-Item "$workspace\src\mods\AutoResearch\manifest.json" "$dist\mods\AutoResearch\" -Force
$autoResearchConfig = @'
{
  "Enabled": true,
  "ScanIntervalTicks": 1,
  "IncludeVoidBag": true,
  "PlaySound": true,
  "ShowNotifications": true,
  "ExcludedItemIds": []
}
'@
Set-Content -Path "$dist\mods\AutoResearch\config.json" -Value $autoResearchConfig -Encoding UTF8

# Copy PiggyVault
Copy-Item "$workspace\src\mods\PiggyVault\bin\Release\PiggyVault.dll" "$dist\mods\PiggyVault\" -Force
Copy-Item "$workspace\src\mods\PiggyVault\bin\Release\PiggyVault.pdb" "$dist\mods\PiggyVault\" -Force
Copy-Item "$workspace\src\mods\PiggyVault\manifest.json" "$dist\mods\PiggyVault\" -Force
$piggyVaultConfig = @'
{
  "Enabled": true,
  "RequirePiggyItemInInventory": true,
  "AutoPickupToPiggyBank": true,
  "CraftFromPiggyBank": true,
  "QuickBuffFromPiggyBank": true,
  "QuickHealFromPiggyBank": true,
  "QuickManaFromPiggyBank": true,
  "ConsumeAmmoAndBaitFromPiggyBank": true,
  "InfoAccessoriesInPiggyBank": true,
  "WormholePotionFromPiggyBank": true,
  "PlayPickupSound": true,
  "ShowPickupText": true
}
'@
Set-Content -Path "$dist\mods\PiggyVault\config.json" -Value $piggyVaultConfig -Encoding UTF8

# Copy TurboBucket
Copy-Item "$workspace\src\mods\TurboBucket\bin\Release\TurboBucket.dll" "$dist\mods\TurboBucket\" -Force
Copy-Item "$workspace\src\mods\TurboBucket\bin\Release\TurboBucket.pdb" "$dist\mods\TurboBucket\" -Force
Copy-Item "$workspace\src\mods\TurboBucket\manifest.json" "$dist\mods\TurboBucket\" -Force
$turboBucketConfig = @'
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsWater": true,
  "AffectsLava": true,
  "AffectsHoney": true,
  "AffectsBottomlessBuckets": true,
  "AffectsEmptyBuckets": false,
  "AffectsSponges": false
}
'@
Set-Content -Path "$dist\mods\TurboBucket\config.json" -Value $turboBucketConfig -Encoding UTF8

# Copy BossCursor
Copy-Item "$workspace\src\mods\BossCursor\bin\Release\BossCursor.dll" "$dist\mods\BossCursor\" -Force
Copy-Item "$workspace\src\mods\BossCursor\bin\Release\BossCursor.pdb" "$dist\mods\BossCursor\" -Force
Copy-Item "$workspace\src\mods\BossCursor\manifest.json" "$dist\mods\BossCursor\" -Force
Copy-Item "$workspace\src\mods\BossCursor\UI\Cursor.png" "$dist\mods\BossCursor\UI\" -Force
$bossCursorConfig = @'
{
  "Enabled": true,
  "HideOnScreen": false,
  "CursorDistance": 150,
  "CursorSize": 1.0,
  "HeadOffset": 45.0,
  "BlacklistPillars": true,
  "ExcludedNpcIds": [],
  "IncludedNpcIds": []
}
'@
Set-Content -Path "$dist\mods\BossCursor\config.json" -Value $bossCursorConfig -Encoding UTF8

Write-Host "`nDistribution assembled successfully in: $dist" -ForegroundColor Green

# Dynamically resolve Terraria game directory
$possibleGameDirs = @(
    $env:TERRARIA_PATH,
    "D:\Jogos\Steam\steamapps\common\Terraria",
    "C:\Program Files (x86)\Steam\steamapps\common\Terraria",
    "C:\Program Files\Steam\steamapps\common\Terraria",
    "C:\GOG Games\Terraria",
    "D:\GOG Games\Terraria",
    "E:\Steam\steamapps\common\Terraria",
    "E:\Jogos\Steam\steamapps\common\Terraria"
)
$gameDir = $null
foreach ($dir in $possibleGameDirs) {
    if (![string]::IsNullOrWhiteSpace($dir) -and (Test-Path (Join-Path $dir "Terraria.exe"))) {
        $gameDir = $dir
        break
    }
}

if ($gameDir) {
    Write-Host "`nDeploying distribution to game directory: $gameDir" -ForegroundColor Cyan
    try {
        Copy-Item -Path "$dist\*" -Destination $gameDir -Recurse -Force
        Write-Host "Deployment to $gameDir completed successfully." -ForegroundColor Green
    }
    catch {
        Write-Warning "Could not overwrite some files in $gameDir (the game may be running). Please close Terraria/TerrariaModded and re-run build_dist.ps1."
    }
}

# Package Release Archive
$zipPath = Join-Path $workspace "TerrariaModCore-v1.2.0.zip"
Write-Host "`nPackaging release archive: $zipPath..." -ForegroundColor Cyan
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}
Compress-Archive -Path "$dist\*" -DestinationPath $zipPath -Force
Write-Host "Release archive packaged successfully: $zipPath" -ForegroundColor Green

Get-ChildItem -Path $dist -Recurse | Select-Object FullName

