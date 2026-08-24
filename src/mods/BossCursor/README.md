# 🎯 Boss Cursor — TerrariaModCore (TMC) Plugin

**Boss Cursor** adds real-time visual directional indicator arrows and boss head icons around your character that point toward active bosses and mini-bosses in **Vanilla Terraria 1.4.5.8 / 1.4.5.7**.

Ported from the popular tModLoader mod by **kgoyo** ([Steam Workshop #2816694149](https://steamcommunity.com/sharedfiles/filedetails/?id=2816694149)), this version runs natively inside **TerrariaModCore (TMC)** with zero tModLoader dependency, zero disk modification, and pure memory IL injection.

---

## 🌟 Key Features

1. **Directional Arrow & Boss Head Indicator**:
   - Points directly toward active bosses and tracked enemies in real time.
   - Renders the boss's official head icon right alongside the pointer arrow.

2. **Dynamic Proximity Scaling & Opacity Fading**:
   - As the boss gets closer to the player, the arrow and head icon become larger and more opaque.
   - As the boss moves farther away or off-screen, the indicator smoothly scales down and becomes translucent.

3. **Gravitation Potion & Upside-Down Inversion**:
   - Automatically detects inverted gravity (`gravDir == -1f`) and adjusts all angles and coordinates so pointers remain accurate.

4. **Fullscreen Map Suppression**:
   - Automatically hides the indicators whenever the fullscreen overlay map is open (`Main.mapStyle == 2`).

5. **Always Active & Seamless**:
   - Runs continuously in the background whenever bosses/mini-bosses are present in the world.
   - Zero keybind clutter or accidental toggle interruptions.

6. **Customizable Whitelist & Blacklist**:
   - Blacklists Celestial / Lunar Towers (Solar, Nebula, Vortex, Stardust) by default.
   - Add any custom NPC ID to the whitelist (e.g. Dreadnautilus, Mourning Wood, Pumpking, Martian Saucer) or blacklist.

7. **Extensible Modder API**:
   - Programmatically add/remove NPCs from the whitelist or blacklist via `BossCursorAPI`.

---

## ⚙️ Configuration (`config.json`)

Located at `<Terraria>/mods/BossCursor/config.json`:

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

| Setting | Type | Default | Description |
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

## 💻 Developer API (`BossCursorAPI`)

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

## 🛡️ Engineering & Compatibility

- **Target Engine**: Terraria 1.4.5.8 / 1.4.5.7 (Steam & GOG)
- **Runtime Framework**: .NET Framework 4.8 / x86
- **Vanilla File Integrity**: 100% untouched on disk.
- **Harmony Hooks**: `Main.DrawInterface_36_Cursor` (Postfix).
