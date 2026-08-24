# 🔍 TerrariaModCore (TMC) — Compatibility & Hook Matrix

This document provides a comprehensive compatibility matrix, runtime hook catalog, and coexistence verification for **TerrariaModCore (TMC)** on **Terraria 1.4.5.8 / 1.4.5.7**.

---

## 1. Game Version Compatibility

| Property | Value | Notes |
| :--- | :--- | :--- |
| **Target Terraria Version** | `1.4.5.8` / `1.4.5.7` | Official Steam & GOG releases supported (1.4.5.x series) |
| **Target Runtime** | `.NET Framework 4.8` | x86 (32-bit Architecture) |
| **Harmony Framework** | `Lib.Harmony 2.4.2` | Runtime IL manipulation |
| **Memory Limit** | `4 GB (LAA)` | Enabled via `IMAGE_FILE_LARGE_ADDRESS_AWARE` (0x0020) |
| **Disk Integrity** | `100% Intact` | `Terraria.exe` unmodified; zero binary patching on disk |

---

## 2. Complete Hook & Patch Catalog

The table below catalogs every vanilla method intercepted across TMC Core and the 10 production plugins (28 total runtime patches):

| Component | Target Type | Target Method | Patch Type | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **TMC Core** | `CaptureManager` | `.ctor()` | `Prefix` | Prevents `NullReferenceException` when initialized before XNA `GraphicsDevice` creation. |
| **TMC Core** | `CaptureManager` | `get_IsCapturing` | `Prefix` | Returns `false` safely if `CaptureCamera` is not yet instantiated. |
| **TMC Core** | `CaptureManager` | `Capture(...)` | `Prefix` | Ensures `CaptureCamera` instance before executing capture routines. |
| **TMC Core** | `CaptureManager` | `DrawTick()` | `Prefix` | Ensures `CaptureCamera` instance before drawing capture frames. |
| **OreCascade** | `Player` | `PickTile(int, int, int)` | `Prefix` & `Postfix` | Identifies broken ore/gem tile and triggers recursive BFS vein cascade. |
| **AutoFishing** | `Player` | `Update(int)` | `Postfix` | Evaluates local player fishing state machine, auto-cast, and auto-reel timers. |
| **AutoFishing** | `Player` | `ItemCheck_Shoot(...)` | `Postfix` | Auto-cast detection and initial bobber registration. |
| **AutoFishing** | `Player` | `ItemCheck_PullFishingBobbers(Item)` | `Prefix` | Auto-pull bite execution. |
| **FishingLinePlus** | `Player` | `ItemCheck_Shoot(...)` | `Postfix` | Spawns additional bobber projectiles with angular spread when casting. |
| **FishingLinePlus** | `Player` | `ItemCheck_PullFishingBobbers(Item)` | `Prefix` | Guarantees fishing loot table checks on all active floating bobbers before retraction. |
| **FishingLinePlus** | `Projectile` | `AI_061_FishingBobber()` | `Postfix` | Synchronizes bite states and splashing animations across all active bobbers in water. |
| **TurboExtractinator** | `Player` | `PlaceThing_ItemInExtractinator(...)` | `Postfix` | Scales down `itemTime` / `itemAnimation` by `SpeedMultiplier` and processes batch extractions. |
| **AutoBuff** | `Player` | `Update(int)` | `Postfix` | Evaluates missing buffs and consumes replenishment potions from inventory/Void Bag/Piggy Bank. |
| **AutoOpen** | `ItemSlot` | `RightClick(Item[], int, int)` | `Prefix` | Detects hold-right-click on grab bags/containers to arm continuous auto-opening. |
| **AutoOpen** | `Player` | `Update(int)` | `Postfix` | Ticks rapid continuous grab bag/container consumption. |
| **AutoResearch** | `Player` | `Update(int)` | `Postfix` | Scans inventory/Void Bag and automatically sacrifices researchable Journey Mode items. |
| **PiggyVault** | `Player` | `GetItem(int, Item, GetItemSettings)` | `Postfix` | Redirects surplus inventory item pickups directly into the Piggy Bank. |
| **PiggyVault** | `Player` | `ItemSpaceForCofveve(Item, ...)` | `Postfix` | Reports available storage space inside Piggy Bank. |
| **PiggyVault** | `Recipe` | `CollectItemsFromChests()` | `Postfix` | Adds Piggy Bank items into crafting recipe availability matrix. |
| **PiggyVault** | `Player` | `QuickHeal_GetItemToUse()` | `Postfix` | Allows Quick Heal to draw healing potions from Piggy Bank. |
| **PiggyVault** | `Player` | `QuickMana_GetItemToUse()` | `Postfix` | Allows Quick Mana to draw mana potions from Piggy Bank. |
| **PiggyVault** | `Player` | `QuickBuff_PickBestFoodItem()` | `Postfix` | Allows Quick Buff to consume food from Piggy Bank. |
| **PiggyVault** | `Player` | `QuickBuff()` | `Postfix` | Allows Quick Buff to consume buff potions from Piggy Bank. |
| **PiggyVault** | `Player` | `ConsumeItem(...)` | `Postfix` | Consumes ammo and bait from Piggy Bank when firing or fishing. |
| **PiggyVault** | `Player` | `HasUnityPotion()` | `Postfix` | Checks Piggy Bank for Wormhole Potions during multiplayer map teleports. |
| **PiggyVault** | `Player` | `TakeUnityPotion()` | `Prefix` & `Postfix` | Consumes Wormhole Potion from Piggy Bank upon teleportation. |
| **PiggyVault** | `Player` | `UpdateEquips(int)` | `Postfix` | Applies informational (GPS, Watch, DPS, Compass, Radar, etc.) and mechanical accessory data during active gameplay. |
| **PiggyVault** | `Player` | `RefreshInfoAccs()` | `Postfix` | Applies informational accessory data stored inside Piggy Bank when paused. |
| **TurboBucket** | `Player` | `ItemCheck_UseBuckets(...)` | `Postfix` | Reduces bucket `itemTime`/`itemAnimation` to 60 TPS and accelerates bottomless buckets and sponges. |
| **BossCursor** | `Main` | `DrawInterface_36_Cursor()` | `Postfix` | Renders directional pointer arrows and boss head icons with proximity scaling. |

---

## 3. Production Mod Coexistence Matrix

All 16 combinations of production mods have been tested and verified for conflict-free coexistence:

| Scenario | Active Plugins | Total Active Patches | Coexistence Status |
| :---: | :--- | :---: | :---: |
| **1** | OreCascade alone | 2 | ✅ Verified Clean |
| **2** | AutoFishing alone | 3 | ✅ Verified Clean |
| **3** | FishingLinePlus alone | 3 | ✅ Verified Clean |
| **4** | TurboExtractinator alone | 1 | ✅ Verified Clean |
| **5** | AutoBuff alone | 1 | ✅ Verified Clean |
| **6** | AutoOpen alone | 2 | ✅ Verified Clean |
| **7** | AutoResearch alone | 1 | ✅ Verified Clean |
| **8** | PiggyVault alone | 13 | ✅ Verified Clean |
| **9** | TurboBucket alone | 1 | ✅ Verified Clean |
| **10** | BossCursor alone | 1 | ✅ Verified Clean |
| **11** | OreCascade + AutoFishing | 5 | ✅ Verified Clean |
| **12** | OreCascade + TurboExtractinator | 3 | ✅ Verified Clean |
| **13** | AutoFishing + FishingLinePlus | 6 | ✅ Verified Clean |
| **14** | AutoBuff + PiggyVault | 14 | ✅ Verified Clean |
| **15** | AutoOpen + AutoResearch | 3 | ✅ Verified Clean |
| **16** | **All Ten Simultaneously** | **28** | **✅ Verified Clean** |
