<div align="center">

# ⛏️ OreCascade — VeinMiner & Ore Excavator for Vanilla Terraria

**Instant chain-mining for ores and gemstones with runtime IL injection, strict vein isolation, legitimate drop preservation, and zero file modification.**

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Key Features

- **⚡ Zero tModLoader Dependency & 100% Vanilla File Integrity**:
  - Runs inside the **TerrariaModCore (TMC)** framework on official Terraria 1.4.5.7.
  - Original `Terraria.exe` remains 100% untouched.

- **🚀 Iterative Breadth-First Search (BFS)**:
  - Discovers contiguous ore veins dynamically with $O(V)$ time and spatial complexity.
  - **Strict Vein Isolation**: Adjacent veins of differing materials (e.g., Gold touching Copper) are strictly isolated when `RequireSameOreType` is enabled.
  - **Diagonal Connectivity**: Supports optional 8-way diagonal exploration for complex vein formations.

- **🛡️ Native Engine Drops & Pickaxe Tier Safety**:
  - Blocks are broken through `WorldGen.KillTile`, preserving vanilla drop tables, lucky coins, particles, achievements, and sound effects.
  - Adheres strictly to vanilla pickaxe tiers (e.g., Cobalt requires 100% pickaxe power, Chlorophyte requires 200%).
  - Thread-static reentrancy guard (`[ThreadStatic] bool _isCascading`) eliminates infinite recursion.

- **🌐 Multiplayer Synchronized**:
  - Automatically broadcasts Tile Manipulation packets (`NetMessage.SendData(17, ...)`) in client multiplayer sessions, synchronizing tile destructions in real-time.

---

## ⚙️ Configuration (`config.json`)

The configuration file is located at `mods/OreCascade/config.json`:

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

### Configuration Reference

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Enables or disables all cascade mining functionality. |
| `MaxBlocksPerActivation` | `int` | `100` | Maximum number of ore blocks mined per chain activation (clamped 1 to 500). |
| `AllowDiagonalConnections` | `bool` | `false` | When `true`, searches 8-directional neighbors instead of 4-way orthogonal neighbors. |
| `RequireSameOreType` | `bool` | `true` | When `true`, restricts mining strictly to the same tile type and frame identity. |
| `IncludeGems` | `bool` | `true` | When `true`, enables chain-mining for natural gemstones (Amethyst, Diamond, etc.) and Amber. |
| `IncludeExtractables` | `bool` | `true` | When `true`, enables chain-mining for extractable resource blocks (Silt, Slush, Desert Fossil). |

---

## 💎 Supported Ores, Gemstones & Extractables

| Tier / Category | Ores & Blocks Included |
| :--- | :--- |
| **Pre-Hardmode Ores** | Copper, Tin, Iron, Lead, Silver, Tungsten, Gold, Platinum, Meteorite, Demonite, Crimtane, Obsidian, Hellstone |
| **Extractables & Fossils** | Silt Block, Slush Block, Desert Fossil, Fossil Ore |
| **Hardmode (Tiers 1-3)** | Cobalt, Palladium, Mythril, Orichalcum, Adamantite, Titanium |
| **Endgame & Celestial** | Chlorophyte, Luminite (Lunar Ore) |
| **Gemstones (Optional)** | Amethyst, Topaz, Sapphire, Emerald, Ruby, Diamond, Amber |

---

## 🔧 Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `PickTile(int x, int y, int pickPower)` | `Prefix` & `Postfix` | Captures tile state before pickaxe impact and triggers the iterative BFS chain-mining when tile destruction is confirmed. |

---

## 📁 Plugin Structure

```text
mods/OreCascade/
├── manifest.json       # Mod identity, dependencies, and entry metadata
├── OreCascade.dll      # Compiled plugin assembly
├── OreCascade.pdb      # Debug symbols
└── config.json         # Runtime configurable options
```
