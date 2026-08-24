<div align="center">

# 📦 AutoOpen — Rapid & Automated Grab Bag Opener for Vanilla Terraria

**Continuous, high-speed automated opening of grab bags, crates, oysters, boss bags, lockboxes, and presents by holding right-click (Extractinator-style) with zero vanilla file modification.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8_|_1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Key Features

- **⚡ Hold-to-Open Continuous Rapid Unpacking**:
  - Eliminates vanilla's frustrating requirement of releasing and clicking the mouse button repeatedly.
  - Simply hold down Right-Click on any stack of crates or grab bags to open them continuously at high speed (just like using an Extractinator).

- **📦 Broad Container Support**:
  - **Fishing Crates**: Wooden, Iron, Golden, Hallowed, Dungeon, Ocean, Jungle, Sky, Corrupt, Crimson, and Hardmode variants.
  - **Boss Treasure Bags**: All Expert/Master mode boss treasure bags.
  - **Specialty Bags & Grab Containers**: Herb Bags, Can of Worms, Oysters, Goodie Bags, Presents, and Chillet Eggs.
  - **Lock Boxes**: Golden Lock Boxes (automatically consumes Golden Keys) and Obsidian Lock Boxes (requires Shadow Key in inventory or Void Bag).

- **🚀 Batch Processing Support**:
  - Configurable `BatchSize` to process multiple containers per tick cycle for instant stack unpacking.

- **🤖 Optional Hands-Free Auto-Open Mode**:
  - `AutoOpenInventory` mode automatically unpacks any grab bags in your inventory or Void Bag in the background without needing to click.

- **🛡️ Key Safety & Exclusion Blacklist**:
  - Halts opening gracefully if prerequisite keys (e.g. Golden Keys) run out.
  - Custom `ExcludedItemIds` blacklist in `config.json` allows reserving specific containers.

---

## ⚙️ Configuration (`config.json`)

The configuration file is located at `mods/AutoOpen/config.json`:

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

### Configuration Reference

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Master toggle enabling or disabling AutoOpen. |
| `RapidRightClickOpen` | `bool` | `true` | Enables continuous fast opening when holding right-click. |
| `OpenDelayTicks` | `int` | `3` | Delay in game ticks between opens while holding right click (3 ticks = 20 openings/sec). |
| `BatchSize` | `int` | `1` | Number of containers opened per cycle (1 to 50). |
| `PlaySound` | `bool` | `true` | Plays the vanilla container opening sound effect. |
| `AutoOpenInventory` | `bool` | `false` | Fully automatic hands-free unpacking of bags in inventory. |
| `AutoOpenIntervalTicks` | `int` | `10` | Frequency in game ticks for background inventory scanning. |
| `IncludeVoidBag` | `bool` | `true` | Scans and opens containers inside the player's Void Bag. |
| `ExcludedItemIds` | `int[]` | `[]` | List of Item IDs excluded from automated opening. |

---

## 🔧 Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.UI.ItemSlot` | `RightClick(Item[] inv, int context, int slot)` | `Prefix` | Intercepts right-click holding on openable containers for rapid continuous opening and prevents stack splitting into cursor. |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Handles hands-free inventory auto-unpacking when `AutoOpenInventory` is enabled. |

---

## 📁 Plugin Structure

```text
mods/AutoOpen/
├── manifest.json       # Mod identity, dependencies, and entry metadata
├── AutoOpen.dll        # Compiled plugin assembly
├── AutoOpen.pdb        # Debug symbols
└── config.json         # Runtime configurable options
```
