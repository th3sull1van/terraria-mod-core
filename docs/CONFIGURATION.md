# ⚙️ TerrariaModCore (TMC) — Configuration Reference

This document provides a comprehensive reference for all configuration files, options, default values, and valid ranges across **TerrariaModCore (TMC)** and its included production plugins.

---

## 1. TMC Core Host Configuration (`TMC/config/core.json`)

The core configuration controls host engine diagnostics, logging, and mod loading policies:

```json
{
  "LogLevel": "Info",
  "DiagnosticBannerOnStartup": true,
  "StrictCompatibilityCheck": true,
  "SafeModeOnModFailure": true,
  "ModsDirectoryName": "mods"
}
```

### Options Breakdown

| Setting | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `LogLevel` | `string` | `"Info"` | Controls log verbosity written to console and `TMC/logs/tmc.log`. Options: `"Trace"`, `"Debug"`, `"Info"`, `"Warn"`, `"Error"`, `"Fatal"`. |
| `DiagnosticBannerOnStartup` | `bool` | `true` | When `true`, prints a diagnostic summary banner showing game version and active mod counts during startup. |
| `StrictCompatibilityCheck` | `bool` | `true` | When `true`, verifies that the loaded `Terraria.exe` version matches target `1.4.5.7`. |
| `SafeModeOnModFailure` | `bool` | `true` | When `true`, prevents a single failing mod from crashing the game; the failing mod is isolated and healthy mods continue loading. |
| `ModsDirectoryName` | `string` | `"mods"` | The directory name relative to the game root where plugins are scanned and loaded. |

---

## 2. OreCascade Configuration (`mods/OreCascade/config.json`)

```json
{
  "Enabled": true,
  "MaxBlocksPerActivation": 100,
  "AllowDiagonalConnections": false,
  "RequireSameOreType": true,
  "IncludeGems": true,
  "IncludeExtractables": true
}
```

### Options Breakdown

| Setting | Type | Default | Range / Format | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Master toggle for all chain-mining functionality. |
| `MaxBlocksPerActivation` | `int` | `100` | `1` – `500` | Maximum number of ore tiles mined in a single chain activation. |
| `AllowDiagonalConnections` | `bool` | `false` | `true` / `false` | When `true`, searches 8-way neighbors; when `false`, restricts search to 4 orthogonal neighbors. |
| `RequireSameOreType` | `bool` | `true` | `true` / `false` | When `true`, prevents mining adjacent ores of a different material type. |
| `IncludeGems` | `bool` | `true` | `true` / `false` | When `true`, enables chain-mining for natural gemstones (Amethyst, Diamond, etc.) and Amber. |
| `IncludeExtractables` | `bool` | `true` | `true` / `false` | When `true`, enables chain-mining for extractable blocks (Silt Block, Slush Block, Desert Fossil). |

---

## 3. AutoFishing Configuration (`mods/AutoFishing/config.json`)

```json
{
  "Enabled": true,
  "AutoCast": true,
  "AutoReel": true,
  "CastDelayTicks": 30,
  "ReelDelayTicks": 2,
  "RequireBait": true
}
```

### Options Breakdown

| Setting | Type | Default | Range / Format | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Master toggle for all fishing automation. |
| `AutoCast` | `bool` | `true` | `true` / `false` | Automatically casts the rod when held and ready. |
| `AutoReel` | `bool` | `true` | `true` / `false` | Automatically reels in when a fish/item bite is detected. |
| `CastDelayTicks` | `int` | `30` | `0` – `300` | Delay in game ticks (60 ticks = 1 second) after reeling before recasting. |
| `ReelDelayTicks` | `int` | `2` | `0` – `120` | Reaction delay in game ticks between bite detection and reel-in execution. |
| `RequireBait` | `bool` | `true` | `true` / `false` | Prevents casting if no bait is present in the player's inventory. |

---

## 4. FishingLinePlus Configuration (`mods/FishingLinePlus/config.json`)

```json
{
  "Enabled": true,
  "MaxActiveFishingLines": 4,
  "LinesPerCast": 4,
  "SpreadAngleDegrees": 7.0,
  "VelocitySpread": 0.08
}
```

### Options Breakdown

| Setting | Type | Default | Range / Format | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Master toggle for multi-line fishing. |
| `MaxActiveFishingLines` | `int` | `4` | `1` – `20` | Maximum total bobbers the player can maintain simultaneously. |
| `LinesPerCast` | `int` | `4` | `1` – `20` | Number of bobbers launched per cast action. |
| `SpreadAngleDegrees` | `double` | `7.0` | `0.0` – `45.0` | Angular spread between bobber trajectories in degrees. |
| `VelocitySpread` | `double` | `0.08` | `0.0` – `0.5` | Randomized velocity variation percentage per bobber for natural distribution. |

---

## 5. TurboExtractinator Configuration (`mods/TurboExtractinator/config.json`)

```json
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsChlorophyteExtractinator": true,
  "BatchExtractionSize": 1
}
```

### Options Breakdown

| Setting | Type | Default | Range / Format | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Master toggle for extraction speed acceleration. |
| `SpeedMultiplier` | `int` | `5` | `1` – `60` | Speed acceleration multiplier (default 5 means 5x faster). |
| `AffectsChlorophyteExtractinator` | `bool` | `true` | `true` / `false` | Also applies speed acceleration to the Chlorophyte Extractinator. |
| `BatchExtractionSize` | `int` | `1` | `1` – `50` | Number of items processed per extraction tick cycle. |

---

## 6. AutoBuff Configuration (`mods/AutoBuff/config.json`)

```json
{
  "Enabled": true,
  "CheckIntervalTicks": 15,
  "IncludeFood": true,
  "IncludeFlasks": true,
  "IncludeVoidBag": true,
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
```

### Options Breakdown

| Setting | Type | Default | Range / Format | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Master toggle for all auto-buff replenishment logic. |
| `CheckIntervalTicks` | `int` | `15` | `1` – `300` | Delay in game ticks between inventory evaluation scans (15 ticks = 4 scans/sec). |
| `IncludeFood` | `bool` | `true` | `true` / `false` | Automatically consumes the best food item available when Well-Fed runs out. |
| `IncludeFlasks` | `bool` | `true` | `true` / `false` | Automatically refreshes melee weapon flasks / imbues. |
| `IncludeVoidBag` | `bool` | `true` | `true` / `false` | Scans items located in the player's unlocked and open Void Bag. |
| `MinBuffTimeThresholdTicks` | `int` | `0` | `0` – `3600` | Re-applies buff if remaining duration is below threshold (0 = only when expired). |
| `ExcludedBuffIds` | `List<int>` | `[18, 119, 120]` | Array of integers | List of Buff IDs excluded from auto-consumption (default excludes Gravitation, Lovestruck, Stinky). |
| `ExcludedItemIds` | `List<int>` | `[1344, 2756]` | Array of integers | List of Item IDs excluded from auto-consumption (default excludes Red Potion, Gender Change). |

---

## 6. AutoOpen (`mods/AutoOpen/config.json`)

Controls continuous rapid right-click opening and automated grab bag and container unpacking.

```json
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
```

### Options Breakdown

| Setting | Type | Default | Range / Format | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Master toggle enabling or disabling AutoOpen. |
| `RapidRightClickOpen` | `bool` | `true` | `true` / `false` | Enables continuous rapid opening when holding down Right-Click. |
| `OpenDelayTicks` | `int` | `3` | `0` – `60` | Delay in game ticks between opens while holding right click (3 ticks = 20 openings/sec). |
| `BatchSize` | `int` | `1` | `1` – `50` | Number of containers opened per tick cycle. |
| `PlaySound` | `bool` | `true` | `true` / `false` | Plays the native container opening sound effect. |
| `AutoOpenInventory` | `bool` | `false` | `true` / `false` | Hands-free background mode that automatically unpacks grab bags in inventory. |
| `AutoOpenIntervalTicks` | `int` | `10` | `1` – `120` | Interval in game ticks between background inventory scans. |
| `IncludeVoidBag` | `bool` | `true` | `true` / `false` | Scans and opens containers inside the player's Void Bag. |
| `ExcludedItemIds` | `List<int>` | `[]` | Array of integers | List of Item IDs excluded from automated opening. |

