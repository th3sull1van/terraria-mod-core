<div align="center">

# BossCursor

**Real-time directional indicator arrows and boss head icons pointing toward active bosses in Vanilla Terraria with proximity scaling and zero file modification.**

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

- **Directional Arrow & Boss Head Indicator**:
  - Points directly toward active bosses and tracked enemies in real time.
  - Renders the boss's official head icon right alongside the pointer arrow.

- **Dynamic Proximity Scaling & Opacity Fading**:
  - As the boss gets closer to the player, the arrow and head icon become larger and more opaque.
  - As the boss moves farther away or off-screen, the indicator smoothly scales down and becomes translucent.

- **Gravitation Potion & Upside-Down Inversion**:
  - Automatically detects inverted gravity (`gravDir == -1f`) and adjusts all angles and coordinates so pointers remain accurate.

- **Fullscreen Map Suppression**:
  - Automatically hides the indicators whenever the fullscreen overlay map is open (`Main.mapStyle == 2`).

- **Always Active & Seamless**:
  - Runs continuously in the background whenever bosses or mini-bosses are present in the world.
  - Zero keybind clutter or accidental toggle interruptions.

- **Customizable Whitelist & Blacklist**:
  - Blacklists Celestial / Lunar Towers (Solar, Nebula, Vortex, Stardust) by default.
  - Add any custom NPC ID to the whitelist (e.g. Dreadnautilus, Mourning Wood, Pumpking, Martian Saucer) or blacklist.

- **Extensible Modder API**:
  - Programmatically add or remove NPCs from the whitelist/blacklist via `BossCursorAPI`.

---

## Configuration Reference

Located at `mods/BossCursor/config.json`:

```json
{
  "Enabled": true,
  "HideOnScreen": false,
  "CursorDistance": 150,
  "CursorSize": 1.0,
  "HeadOffset": 45.0,
  "BlacklistPillars": true,
  "ExcludedNpcIds": [],
  "IncludedNpcIds": []
}
```

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | `bool` | `true` | Master switch to enable or disable Boss Cursor (always active while true). |
| `HideOnScreen` | `bool` | `false` | When `true`, hides the cursor if the boss is currently visible within the camera view. |
| `CursorDistance` | `int` | `150` | Radial distance (in pixels) from player center to cursor (range: `0` to `500`). |
| `CursorSize` | `float` | `1.0` | Scale multiplier for the arrow and boss head icon (range: `0.1` to `2.0`). |
| `HeadOffset` | `float` | `45.0` | Radial separation in pixels between pointer arrow and boss head icon. |
| `BlacklistPillars` | `bool` | `true` | When `true`, excludes the four Celestial / Lunar Towers. |
| `ExcludedNpcIds` | `int[]` | `[]` | Custom list of NPC IDs that should never display a cursor. |
| `IncludedNpcIds` | `int[]` | `[]` | Custom list of NPC IDs that should always display a cursor (mini-bosses, events). |

---

## Developer API (`BossCursorAPI`)

Other TMC plugins can interact with Boss Cursor at runtime:

```csharp
using BossCursor;

// Add a custom NPC to the whitelist with optional custom head texture
BossCursorAPI.AddToWhitelist(npcId, customHeadTexture);

// Remove an NPC from the whitelist
BossCursorAPI.RemoveFromWhitelist(npcId);

// Add an NPC to the blacklist
BossCursorAPI.AddToBlacklist(npcId);

// Check if an NPC is currently tracked
bool isTracked = BossCursorAPI.IsBossTracked(npc);

// Toggle Boss Cursor state
BossCursorAPI.SetEnabled(true);
```

---

## Intercepted Runtime Methods

| Target Class | Target Method | Hook Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Terraria.Main` | `DrawInterface_36_Cursor()` | `Postfix` | Renders directional arrows and boss head icons over the in-game UI layer. |

---

## Plugin Structure

```text
mods/BossCursor/
├── manifest.json       # Mod identity, dependencies, and entry metadata
├── BossCursor.dll      # Compiled plugin assembly
├── BossCursor.pdb      # Debug symbols
├── README.md           # Master English documentation
├── README_pt-BR.md     # Master Brazilian Portuguese documentation
└── config.json         # Runtime configurable options
```

---

## License

MIT © [th3sull1van](https://github.com/th3sull1van)
