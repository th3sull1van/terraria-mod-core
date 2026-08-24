# 🧪 TerrariaModCore (TMC) — Testing Strategy & Verification Suite

This document describes the automated test architecture, test suites, and assertion coverage for **TerrariaModCore (TMC)**.

---

## 1. Overview & Test Runner

TMC includes a standalone test suite (`TerrariaModCore.Tests.exe`) that executes **378 comprehensive assertions** covering core infrastructure, isolated plugin behaviors, and multi-mod coexistence scenarios.

### Running the Test Suite
```powershell
# Build and execute all tests
dotnet build tests/TerrariaModCore.Tests/TerrariaModCore.Tests.csproj -c Release -p:Platform="x86"
& "tests/TerrariaModCore.Tests/bin/Release/TerrariaModCore.Tests.exe"
```

---

## 2. Test Suite Breakdown

```text
==========================================
     TerrariaModCore (TMC) Test Suite     
==========================================
RESULTS: 378 PASSED, 0 FAILED
==========================================
```

### 2.1 Dependency Resolver & Topological Sort Tests
- **Valid Linear Graph**: Validates strict sequence resolution (`A -> B -> C`).
- **LoadOrder & LoadAfter**: Verifies explicit load-after constraints.
- **Cycle Detection**: Simulates circular dependencies (`A -> B -> C -> A`) and confirms resolution abort with diagnostics.
- **IncompatibleWith**: Verifies that declared incompatible mods fail resolution gracefully.
- **Missing Dependency**: Verifies error detection when mandatory dependencies are absent.
- **Optional Dependencies**: Confirms optional dependencies load when present and do not fail when absent.

### 2.2 Central Harmony Patch Manager Tests
- **Prefix & Postfix Registration**: Validates method hooking without target modification.
- **Multi-Mod Shared Targets**: Confirms multiple independent mods can hook the same target method.
- **Priority Execution**: Validates `PatchPriority.High` executes before `PatchPriority.Normal`.
- **Isolated Unpatching**: Unpatches one mod and verifies other active mods' hooks remain intact.

### 2.3 Fault Isolation & SafeMode Tests
- **Simulated Catastrophic Failure**: Injects an unhandled exception inside a test mod's `Load()` method.
- **Process Protection**: Confirms the exception is caught, logged, the mod marked as `ModState.Failed`, and the host process remains healthy.

### 2.4 Configuration & GameVersionChecker Tests
- **JSON Serialization**: Validates configuration saving, loading, and default creation.
- **Game Version Validation**: Validates detection of `1.4.5.8` and `1.4.5.7` releases.

### 2.5 Plugin Unit Tests
- **OreCascade**: Iterative BFS traversal, strict vein isolation, orthogonal/diagonal search, pickaxe tier requirements.
- **AutoFishing**: Native bite condition `ai[1] < 0f && localAI[1] != 0f`, 60 TPS state machine, bait validation.
- **FishingLinePlus**: Bobber count clamping, angular velocity spread physics, synchronized pull triggers.
- **TurboExtractinator**: Speed scaling (`itemTime` reduction), 1-tick frame floor, batch extraction.
- **AutoBuff**: Food/buff priority, non-wasteful threshold timers, exclusion lists, Void Bag/Piggy Bank fallback.
- **AutoOpen**: Rapid hold-right-click arming, batch container unpacking, sound effects.
- **AutoResearch**: Journey Mode sacrifice requirements, automatic inventory scanning, item preservation.
- **PiggyVault**: Surplus pickup routing, direct crafting table integration, quick heal/mana/buff, ammo consumption, info accessories.
- **TurboBucket**: 60 TPS liquid pouring, continuous flow, bottomless bucket acceleration, sponge absorption.
- **BossCursor**: Boss detection heuristics, 4 Celestial Pillar exclusion, angle rotation, proximity opacity/scaling, inverted gravity math, headless texture fallback.

### 2.6 Mod Coexistence Matrix Tests (16 Scenarios)
- **Scenarios 1–10**: Individual plugin lifecycle and patch stability for all 10 plugins.
- **Scenarios 11–15**: Dual-mod synergies (`OreCascade + AutoFishing`, `OreCascade + TurboExtractinator`, `AutoFishing + FishingLinePlus`, `AutoBuff + PiggyVault`, `AutoOpen + AutoResearch`).
- **Scenario 16**: **All 10 production mods active simultaneously** with 28 active patches, 0 conflicts, and 100% clean unload verified.
