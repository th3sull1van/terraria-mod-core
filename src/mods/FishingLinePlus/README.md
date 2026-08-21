<div align="center">

# 🎣 FishingLinePlus — Multi-Bobber & Multi-Hook Fishing for Vanilla Terraria

**Cast and manage multiple independent fishing lines simultaneously with velocity spread physics, dual-layer catch synchronization, and full vanilla drop legitimacy.**

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC_Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/License-MIT-10b981?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## 🌟 Key Features

- **⚡ Configurable Simultaneous Lines**:
  - Overrides the vanilla restriction of 1 bobber per player to support up to **4 (or more)** simultaneous functional lines.

- **📐 Realistic Fan Spread & Velocity Jitter**:
  - Automatically calculates angular offsets (`SpreadAngleDegrees`) and velocity variations (`VelocitySpread`) so all bobbers distribute naturally across the water surface instead of overlapping on a single point.

- **🌊 Dual-Layer Multi-Catch Synchronization**:
  - **Dynamic In-Water Synchronization (`BobberSyncPatch`)**: When any one of the player's active bobbers receives a bite (`ai[1] < 0`), sibling bobbers floating in water are instantly synchronized with `FishingCheck()` rolls. Visually, all bobbers splash and bob in unison.
  - **Reel-In Multi-Drop Guarantee (`BobberPullPatch`)**: When reeling in (manually or via `AutoFishing`), all floating bobbers in water (`ai[0] == 0f`) roll their fishing loot tables before retraction. All lines catch and retrieve items simultaneously!

- **🛡️ Legitimate Bait & Tackle Box Rules**:
  - Every caught fish/crate consumes its respective bait item from inventory, fully respecting the Tackle Box chance (`accTackleBox`).

---

## ⚙️ Configuration (`config.json`)

The configuration file is located at `mods/FishingLinePlus/config.json`:

```json
{
  "Enabled": true,
  "MaxActiveFishingLines": 4,
  "LinesPerCast": 4,
  "SpreadAngleDegrees": 7.0,
  "VelocitySpread": 0.08
}
```

### Configuration Reference

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Enables or disables multi-line fishing capabilities. |
| `MaxActiveFishingLines` | `int` | `4` | Maximum total bobbers the player can maintain simultaneously (clamped 1 to 20). |
| `LinesPerCast` | `int` | `4` | Number of bobbers to launch with a single cast action (clamped 1 to `MaxActiveFishingLines`). |
| `SpreadAngleDegrees` | `double` | `7.0` | Angular spread between bobber trajectories in degrees. |
| `VelocitySpread` | `double` | `0.08` | Randomized velocity variation percentage per bobber for natural distribution. |

---

## 🔧 Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `ItemCheck_Shoot(int i, Item sItem, int weaponDamage)` | `Postfix` | Spawns additional bobber projectiles with angular spread when a fishing rod is cast. |
| `Terraria.Player` | `ItemCheck_PullFishingBobbers(Item sItem)` | `Prefix` | Guarantees fishing loot table checks on all active floating bobbers before retraction. |
| `Terraria.Projectile` | `AI_061_FishingBobber()` | `Postfix` | Synchronizes bite states and splashing animations across all active bobbers in water. |

---

## 📁 Plugin Structure

```text
mods/FishingLinePlus/
├── manifest.json            # Mod identity, dependencies, and entry metadata
├── FishingLinePlus.dll      # Compiled plugin assembly
├── FishingLinePlus.pdb      # Debug symbols
└── config.json              # Runtime configurable options
```
