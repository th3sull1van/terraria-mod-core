<div align="center">

# AutoResearch

**Zero-effort, automatic Journey Mode item researching whenever unresearched items enter your inventory, preserving 100% of vanilla quantity rules with zero disk modification.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8%20%7C%201.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony%202.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC%20Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-3b82f6?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## Key Features

- **Instant Pickup Research**:
  - Automatically researches and consumes items the instant they are collected (`Player.GetItem`), eliminating manual menu dragging and slot sacrifice clicks.
  - If a collected stack exceeds the required research count (e.g. collecting 100 Wood when only 40 is needed), exactly 40 Wood is sacrificed to unlock duplication and the remaining 60 Wood lands in your inventory.

- **Hands-Free Inventory & Void Bag Sweep**:
  - Background periodic sweep scans player inventory, active cursor item (`Main.mouseItem`), and Void Bag (`bank4`) for items acquired via crafting, vendor purchases, or chest looting.

- **Strict Vanilla Rule & Quantity Preservation**:
  - Does **not** alter vanilla sacrifice quantities or catalog thresholds (e.g. 100 Wood, 25 Iron Ore, 1 Sword).
  - Partially researched items track and contribute progress accurately until the vanilla unlocking threshold is reached.

- **Journey Mode Isolation**:
  - Automatically inert when playing Classic, Mediumcore, or Hardcore characters (`player.difficulty != 3`), leaving non-Journey gameplay completely untouched.

- **Native Audio & Visual Telemetry**:
  - Plays native research sounds (`SoundID.Research` and `SoundID.ResearchComplete`).
  - Displays color-coded chat feedback upon partial contribution and infinite item duplication unlocking.

---

## Configuration Reference

Located at `mods/AutoResearch/config.json`:

```json
{
  "Enabled": true,
  "AutoResearchOnPickup": true,
  "AutoResearchInventory": true,
  "ScanIntervalTicks": 5,
  "IncludeVoidBag": true,
  "PlaySound": true,
  "ShowNotifications": true,
  "ExcludedItemIds": []
}
```

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Master toggle enabling or disabling AutoResearch. |
| `AutoResearchOnPickup` | `bool` | `true` | Automatically researches items the moment they are picked up. |
| `AutoResearchInventory` | `bool` | `true` | Automatically scans and researches items entering inventory via crafting, shopping, or chest retrieval. |
| `ScanIntervalTicks` | `int` | `5` | Frequency in game ticks for background inventory scanning (~12 checks/sec). |
| `IncludeVoidBag` | `bool` | `true` | Scans and auto-researches items inside the player's Void Bag. |
| `PlaySound` | `bool` | `true` | Plays vanilla research and unlocking sound effects. |
| `ShowNotifications` | `bool` | `true` | Displays in-game chat notifications when items are researched or unlocked. |
| `ExcludedItemIds` | `int[]` | `[]` | List of Item IDs excluded from automated research. |

---

## Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `GetItem(Item newItem, GetItemSettings settings)` | `Prefix` | Intercepts item acquisition and instantly researches items, reducing stacks or consuming them before inventory placement. |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Sweeps inventory slots, cursor items, and void vault in the background for crafted, bought, or chest-transferred items. |

---

## Plugin Structure

```text
mods/AutoResearch/
├── manifest.json       # Mod identity, dependencies, and entry metadata
├── AutoResearch.dll    # Compiled plugin assembly
├── AutoResearch.pdb    # Debug symbols
├── README.md           # Master English documentation
├── README_pt-BR.md     # Master Brazilian Portuguese documentation
└── config.json         # Runtime configurable options
```

---

## License

MIT © [th3sull1van](https://github.com/th3sull1van)
