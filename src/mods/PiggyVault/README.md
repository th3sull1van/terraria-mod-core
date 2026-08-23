<div align="center">

# 🐷 PiggyVault — Void Bag Storage Automation for the Piggy Bank

**Empowers the Piggy Bank (`player.bank`) with all the modern capabilities of the Void Bag (`player.bank4`) — including overflow auto-pickup, direct crafting, Quick Buff/Heal/Mana, ammo/bait consumption, and informational accessories — with 100% vanilla disk integrity.**

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Key Features

- **📦 Overflow Auto-Pickup / Vacuum**:
  - Automatically captures items and coins from the world when player inventory (slots 0..49) is full and safely routes them directly into your Piggy Bank (`player.bank.item`).
  - Plays visual pickup text and audio feedback matching vanilla container vacuuming.

- **🔨 Direct Crafting from Piggy Bank**:
  - Hooks into `Recipe.CollectItemsFromChests` so crafting stations recognize all materials stored inside your Piggy Bank without having to place it down or manually withdraw items.

- **🧪 Quick Buff, Quick Heal & Quick Mana**:
  - **Quick Heal (`H`)**: Automatically drinks healing potions from your Piggy Bank if missing from inventory.
  - **Quick Mana (`M`)**: Automatically drinks mana potions from your Piggy Bank during intense magic casting.
  - **Quick Buff (`B`)**: Evaluates all missing buffs and consumes buff potions & best food items directly from your Piggy Bank.

- **🏹 Direct Ammo, Wire & Bait Consumption**:
  - Automatically fires arrows, bullets, rockets, and consumes wires, actuators, and fishing bait stored in the Piggy Bank when not present in the main inventory.

- **🧭 Informational Accessories Activation**:
  - Cell Phone, PDA, Compass, Depth Meter, Watch, GPS, DPS Meter, Metal Detector, Radar, and other information accessories active while stored inside your Piggy Bank.

- **🌀 Wormhole / Unity Potions**:
  - Teleport to teammates on the fullscreen map using Wormhole Potions stored in your Piggy Bank.

- **🛡️ 100% Non-Destructive & In-Memory**:
  - Preserves all standard Piggy Bank functions (saving coins, buying items, Money Trough, Chester) with zero disk modification.

---

## ⚙️ Configuration (`config.json`)

The configuration file is located at `mods/PiggyVault/config.json`:

```json
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
```

### Configuration Reference

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Master switch enabling or disabling the PiggyVault mod. |
| `RequirePiggyItemInInventory` | `bool` | `true` | Requires carrying a Piggy Bank, Money Trough, or Eye Bone/Chester in inventory to activate features. When `false`, features are always active. |
| `AutoPickupToPiggyBank` | `bool` | `true` | Routes world item pickups to Piggy Bank when inventory is full. |
| `CraftFromPiggyBank` | `bool` | `true` | Allows crafting recipes to use ingredients in the Piggy Bank. |
| `QuickBuffFromPiggyBank` | `bool` | `true` | Enables Quick Buff and food consumption from Piggy Bank. |
| `QuickHealFromPiggyBank` | `bool` | `true` | Enables Quick Heal from Piggy Bank. |
| `QuickManaFromPiggyBank` | `bool` | `true` | Enables Quick Mana from Piggy Bank. |
| `ConsumeAmmoAndBaitFromPiggyBank` | `bool` | `true` | Enables consuming ammo, wire, and bait from Piggy Bank. |
| `InfoAccessoriesInPiggyBank` | `bool` | `true` | Enables informational accessories inside Piggy Bank. |
| `WormholePotionFromPiggyBank` | `bool` | `true` | Allows using Wormhole Potions stored in Piggy Bank. |
| `PlayPickupSound` | `bool` | `true` | Plays audio chime when items are stored in Piggy Bank. |
| `ShowPickupText` | `bool` | `true` | Shows popup notification text for vacuumed items. |

---

## 🔧 Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `GetItem` | `Postfix` | Routes overflow items and coins to Piggy Bank. |
| `Terraria.Player` | `ItemSpaceForCofveve` | `Postfix` | Signals world pickup eligibility when Piggy Bank has space. |
| `Terraria.Recipe` | `CollectItemsFromChests` | `Postfix` | Adds Piggy Bank to available crafting material sources. |
| `Terraria.Player` | `QuickHeal_GetItemToUse` | `Postfix` | Fallback search for healing potions in Piggy Bank. |
| `Terraria.Player` | `QuickMana_GetItemToUse` | `Postfix` | Fallback search for mana potions in Piggy Bank. |
| `Terraria.Player` | `QuickBuff_PickBestFoodItem`| `Postfix` | Fallback search for best food item in Piggy Bank. |
| `Terraria.Player` | `QuickBuff` | `Postfix` | Applies missing active potion buffs from Piggy Bank. |
| `Terraria.Player` | `ConsumeItem` | `Postfix` | Consumes ammo, wire, and bait from Piggy Bank. |
| `Terraria.Player` | `HasUnityPotion` | `Postfix` | Checks Wormhole Potions in Piggy Bank. |
| `Terraria.Player` | `TakeUnityPotion` | `Prefix/Postfix` | Consumes Wormhole Potions from Piggy Bank. |
| `Terraria.Player` | `RefreshInfoAccs` | `Postfix` | Updates UI info for accessories stored in Piggy Bank. |

---

## 📁 Plugin Structure

```text
mods/PiggyVault/
├── manifest.json       # Mod identity and entry metadata
├── PiggyVault.dll      # Compiled plugin assembly
├── PiggyVault.pdb      # Debug symbols
└── config.json         # Runtime configurable options
```
