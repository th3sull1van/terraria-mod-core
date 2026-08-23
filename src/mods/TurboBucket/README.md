# 🪣 TurboBucket — High-Speed Liquid Bucket Acceleration Mod for Terraria

**TurboBucket** accelerates the pouring and placement speed of liquid buckets and bottomless buckets in **Vanilla Terraria 1.4.5.7**, allowing continuous liquid dumping and rapid tank filling at up to **60 TPS** without lag or engine desync.

---

## 🚀 Features

- **⚡ Configurable Speed Multiplier**: Accelerates bucket pouring from standard 10 ticks down to 2 ticks (30 pours/sec at 5x) or 1 tick (60 pours/sec at 10x).
- **🍯 Honey Bucket Support**: Instantly empties and streams honey without sluggish delays.
- **🌋 Lava Bucket Support**: Rapidly creates obsidian bridges or fills hellevators with lava.
- **💧 Water Bucket Support**: Rapid lake creation and ocean restorations.
- **✨ Bottomless Bucket Support**: Fully compatible with Bottomless Water, Lava, Honey, and Shimmer buckets.
- **🧹 Optional Empty Bucket & Sponge Acceleration**: Optional speed boost for liquid gathering with empty buckets and drying with sponges.

---

## ⚙️ Configuration (`mods/TurboBucket/config.json`)

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

| Setting | Default | Description |
| :--- | :--- | :--- |
| `Enabled` | `true` | Enables or disables the mod. |
| `SpeedMultiplier` | `5` | Speed multiplier (1 to 10). 5 = 5x faster (2 ticks/pour), 10 = 60 TPS (1 tick/pour). |
| `AffectsWater` | `true` | Accelerate Water Bucket pouring. |
| `AffectsLava` | `true` | Accelerate Lava Bucket pouring. |
| `AffectsHoney` | `true` | Accelerate Honey Bucket pouring. |
| `AffectsBottomlessBuckets` | `true` | Accelerate Bottomless buckets (Water, Lava, Honey, Shimmer). |
| `AffectsEmptyBuckets` | `false` | Accelerate draining liquids into Empty Buckets. |
| `AffectsSponges` | `false` | Accelerate liquid absorption with sponges. |
