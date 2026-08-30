<div align="center">

# AutoBuff

**Automatically consumes buff potions and food from your inventory and Void Bag when buff durations expire, ensuring 100% active buff uptime with zero file modification.**

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

- **Native In-Engine Buff Expiration Detection**:
  - Automatically evaluates player active buffs and remaining durations during `Player.Update`.
  - Consumes corresponding buff potions the instant an active buff expires.

- **Smart Food Tier Consumption**:
  - Detects Well Fed status (*Well Fed / Plenty Satisfied / Exquisitely Stuffed*).
  - Automatically chooses and consumes the highest-tier food item available in inventory when nourishment expires.

- **Automatic Weapon Imbue Refresh**:
  - Keeps melee weapon flasks (Ichor, Cursed Flames, Fire, Gold, Venom, Poison, Nanites, Confetti) active continuously.

- **Deep Void Bag & Piggy Bank Integration**:
  - Seamlessly searches items stored in the player's Void Bag (`bank4`) and Piggy Bank (`bank`) when carried or open.

- **Configurable Blacklist & Safety Guards**:
  - Includes sensible default exclusions for hazardous or situational items (such as *Gravitation Potion* or *Red Potion* in standard worlds).
  - Fully customizable buff and item exclusion lists in `config.json`.

---

## Configuration Reference

Located at `mods/AutoBuff/config.json`:

```json
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
```

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Master switch enabling or disabling the AutoBuff mod. |
| `CheckIntervalTicks` | `int` | `15` | Delay in game ticks (60 ticks = 1s) between inventory scans (15 ticks = 4 scans/sec). |
| `IncludeFood` | `bool` | `true` | Automatically consumes best food item when Well-Fed runs out. |
| `IncludeFlasks` | `bool` | `true` | Automatically refreshes melee weapon imbues / flasks. |
| `IncludeVoidBag` | `bool` | `true` | Scans potions and food stored in the player's open Void Bag. |
| `IncludePiggyBank` | `bool` | `true` | Scans potions and food stored in the player's Piggy Bank when carried or open. |
| `MinBuffTimeThresholdTicks` | `int` | `0` | Re-applies buff if remaining duration is below this threshold (0 = only when expired). |
| `ExcludedBuffIds` | `int[]` | `[18, 119, 120]` | List of Buff IDs excluded from auto-consumption. |
| `ExcludedItemIds` | `int[]` | `[1344, 2756]` | List of Item IDs excluded from auto-consumption. |

---

## Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Evaluates buff durations and triggers safe potion and food consumption for the local player (`i == Main.myPlayer`). |

---

## Plugin Structure

```text
mods/AutoBuff/
├── manifest.json       # Mod identity, dependencies, and entry metadata
├── AutoBuff.dll        # Compiled plugin assembly
├── AutoBuff.pdb        # Debug symbols
├── README.md           # Master English documentation
├── README_pt-BR.md     # Master Brazilian Portuguese documentation
└── config.json         # Runtime configurable options
```

---

## License

MIT © [th3sull1van](https://github.com/th3sull1van)
