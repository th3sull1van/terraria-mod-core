<div align="center">

# 🎣 AutoFishing — Smart Fishing Automation for Vanilla Terraria

**Intelligent automated casting, bite detection, and reel-in execution for Vanilla Terraria with state-machine coordination and zero file modification.**

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8_|_1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Key Features

- **🎮 User-Initiated Lifecycle (Manual Start / Manual Stop)**:
  - Selecting a fishing rod in the hotbar will **not** cast automatically.
  - Automation starts only after the user selects the rod and performs the first manual cast click.
  - Automation stops immediately whenever the user manually clicks to reel in or cancel the line.
  - Switching hotbar slots cleanly resets automation.

- **⚡ Zero External Macros & Native Game Loop Sync**:
  - Executes directly inside `Player.Update` in synchronization with the engine's 60 TPS tick cycle.
  - Zero external mouse hooking or fragile screen-pixel reading.

- **🎯 Precise In-Engine Bite Detection**:
  - Monitors active bobbers owned by the local player (`bobber.ai[1] < 0f && bobber.localAI[1] != 0f`).
  - Catches fish the instant the vanilla physics engine confirms a bite.

- **🛡️ Inventory Bait Verification**:
  - Automatically scans inventory slots 0–57 for valid fishing bait before casting.
  - Pauses automation gracefully if bait is exhausted when `RequireBait` is enabled.

- **⏱️ Configurable Natural Reaction Timers**:
  - Customizable reel-in delay (`ReelDelayTicks`) to simulate humanized reaction times.
  - Configurable cooldown between catches (`CastDelayTicks`) before recasting.

- **🤝 Full Multi-Bobber Compatibility**:
  - Fully compatible with `FishingLinePlus`. Scans all active lines in the water and reels in whenever any bobber hooks a catch.

---

## ⚙️ Configuration (`config.json`)

The configuration file is located at `mods/AutoFishing/config.json`:

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

### Configuration Reference

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Enables or disables all auto-fishing automation. |
| `AutoCast` | `bool` | `true` | Automatically recasts the rod while automation is active. |
| `AutoReel` | `bool` | `true` | Automatically reels in when a fish/item bites. |
| `CastDelayTicks` | `int` | `30` | Delay in game ticks (60 ticks = 1 second) after reeling before recasting. |
| `ReelDelayTicks` | `int` | `2` | Reaction delay in game ticks between bite detection and reeling in. |
| `RequireBait` | `bool` | `true` | Prevents casting if no bait is present in the player's inventory. |

---

## 🔧 Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `Update(int i)` | `Postfix` | Executes the fishing controller state machine for the local player (`i == Main.myPlayer`). |
| `Terraria.Player` | `ItemCheck_Shoot(int i, Item sItem, int weaponDamage)` | `Postfix` | Intercepts manual casting to engage automation. |
| `Terraria.Player` | `ItemCheck_PullFishingBobbers(Item sItem)` | `Prefix` | Intercepts manual reel-in to disengage automation. |

---

## 📁 Plugin Structure

```text
mods/AutoFishing/
├── manifest.json       # Mod identity, dependencies, and entry metadata
├── AutoFishing.dll     # Compiled plugin assembly
├── AutoFishing.pdb     # Debug symbols
└── config.json         # Runtime configurable options
```
