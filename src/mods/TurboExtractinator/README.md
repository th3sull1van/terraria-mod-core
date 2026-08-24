<div align="center">

# ⚡ TurboExtractinator — High-Speed Extractinator for Vanilla Terraria

**Dramatically accelerates Extractinator and Chlorophyte Extractinator processing speeds by a configurable multiplier (default 5x) with zero vanilla file modification.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8_|_1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/Speed-5x_Configurable-f59e0b?style=for-the-badge" alt="5x Configurable">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Key Features

- **⚡ Configurable Speed Acceleration (Default 5x)**:
  - Accelerates the consumption and drop generation rate of all extractable items (Silt, Slush, Desert Fossil, Glowing Moss, and Chlorophyte conversions).
  - Turns thousands of extractable blocks into gems, coins, ores, and fossils in seconds without tedious waiting.

- **🌿 Dual Extractinator Support**:
  - Fully supports both the standard **Extractinator** (`TileID.Extractinator` / 219) and the Hardmode **Chlorophyte Extractinator** (`TileID.ChlorophyteExtractinator` / 642).

- **🛡️ 100% Vanilla Loot Legitimacy**:
  - Executes native `Player.ExtractinatorUse` and `ExtractinatorHelper.RollExtractinatorDrop` routines.
  - Drops coins, gems, ores, prehistoric fossils, and rare items with identical vanilla probability distributions and legitimate sound effects.

- **📦 Batch Extraction Support**:
  - Supports optional batch extraction processing per interaction cycle for ultra-fast clearing of massive inventories.

---

## ⚙️ Configuration (`config.json`)

The configuration file is located at `mods/TurboExtractinator/config.json`:

```json
{
  "Enabled": true,
  "SpeedMultiplier": 5,
  "AffectsChlorophyteExtractinator": true,
  "BatchExtractionSize": 1
}
```

### Configuration Reference

| Option | Type | Default | Range / Format | Description |
| :--- | :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | `true` / `false` | Master toggle for extraction speed acceleration. |
| `SpeedMultiplier` | `int` | `5` | `1` – `60` | Speed acceleration factor (5 means 5x faster extraction rate). |
| `AffectsChlorophyteExtractinator` | `bool` | `true` | `true` / `false` | When `true`, also applies speed acceleration to the Chlorophyte Extractinator. |
| `BatchExtractionSize` | `int` | `1` | `1` – `50` | Number of items processed per extraction tick cycle. |

---

## 🔧 Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `PlaceThing_ItemInExtractinator(Item sItem)` | `Postfix` | Scales down `player.itemTime` and `player.itemAnimation` cooldowns by `SpeedMultiplier` and processes extra batch extractions. |

---

## 📁 Plugin Structure

```text
mods/TurboExtractinator/
├── manifest.json               # Mod identity, dependencies, and entry metadata
├── TurboExtractinator.dll      # Compiled plugin assembly
├── TurboExtractinator.pdb      # Debug symbols
└── config.json                 # Runtime configurable options
```
