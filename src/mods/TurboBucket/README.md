<div align="center">

# TurboBucket

**Accelerates the pouring and placement speed of liquid buckets and bottomless buckets in Vanilla Terraria with 60 TPS continuous flow and zero disk modification.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8%20%7C%201.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony%202.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Type-TMC%20Plugin-06b6d4?style=for-the-badge" alt="TMC Plugin">
  <img src="https://img.shields.io/badge/Speed-10x%20%2F%2060%20TPS-f59e0b?style=for-the-badge" alt="10x / 60 TPS">
  <img src="https://img.shields.io/badge/License-MIT-3b82f6?style=for-the-badge" alt="License MIT">
</p>

</div>

---

## Key Features

- **Configurable Speed Multiplier**:
  - Accelerates bucket pouring from standard 10 ticks down to 2 ticks (30 pours/sec at 5x) or 1 tick (60 pours/sec at 10x).

- **Honey, Lava & Water Bucket Acceleration**:
  - Instantly empties and streams honey without sluggish delays.
  - Rapidly creates obsidian bridges or fills hellevators with lava.
  - Fast lake creation and ocean restorations.

- **Bottomless Bucket Support**:
  - Fully compatible with Bottomless Water, Lava, Honey, and Shimmer buckets.

- **Optional Empty Bucket & Sponge Acceleration**:
  - Optional speed boost for liquid gathering with empty buckets and drying with sponges.

---

## Configuration Reference

Located at `mods/TurboBucket/config.json`:

```json
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
```

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Enables or disables all TurboBucket acceleration features. |
| `SpeedMultiplier` | `int` | `5` | Speed multiplier (1 to 10). 5 = 5x faster (2 ticks/pour), 10 = 60 TPS (1 tick/pour). |
| `AffectsWater` | `bool` | `true` | Accelerates Water Bucket pouring. |
| `AffectsLava` | `bool` | `true` | Accelerates Lava Bucket pouring. |
| `AffectsHoney` | `bool` | `true` | Accelerates Honey Bucket pouring. |
| `AffectsBottomlessBuckets` | `bool` | `true` | Accelerates Bottomless buckets (Water, Lava, Honey, Shimmer). |
| `AffectsEmptyBuckets` | `bool` | `false` | Accelerates draining liquids into Empty Buckets. |
| `AffectsSponges` | `bool` | `false` | Accelerates liquid absorption with sponges. |

---

## Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Player` | `ItemCheck_UseBuckets(Item sItem)` | `Postfix` | Scales down `player.itemTime` and `player.itemAnimation` cooldowns by `SpeedMultiplier`. |

---

## Plugin Structure

```text
mods/TurboBucket/
├── manifest.json       # Mod identity, dependencies, and entry metadata
├── TurboBucket.dll     # Compiled plugin assembly
├── TurboBucket.pdb     # Debug symbols
├── README.md           # Master English documentation
├── README_pt-BR.md     # Master Brazilian Portuguese documentation
└── config.json         # Runtime configurable options
```

---

## License

MIT © [th3sull1van](https://github.com/th3sull1van)
