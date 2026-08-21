# 🧪 TerrariaModCore (TMC) — Testing Strategy & Verification Suite

This document describes the automated test architecture, test suites, and assertion coverage for **TerrariaModCore (TMC)**.

---

## 1. Overview & Test Runner

TMC includes a standalone test suite (`TerrariaModCore.Tests.exe`) that executes **85 comprehensive assertions** covering core infrastructure, isolated plugin behaviors, and multi-mod coexistence scenarios.

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
RESULTS: 85 PASSED, 0 FAILED
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
- **Multi-Mod Shared Targets**: Confirms two independent mods can hook the same target method.
- **Priority Execution**: Validates `PatchPriority.High` executes before `PatchPriority.Normal`.
- **Isolated Unpatching**: Unpatches `mod_2` and verifies that `mod_1`'s hooks remain active and functional.

### 2.3 Fault Isolation & SafeMode Tests
- **Simulated Catastrophic Failure**: Injects an unhandled exception inside a test mod's `Load()` method.
- **Process Protection**: Confirms the exception is caught, logged, the mod marked as `ModState.Failed`, and the host process remains healthy.

### 2.4 OreCascade Plugin Tests
- **Iterative BFS Traversal**: Tests contiguous ore discovery on simulated tile grids.
- **Vein Isolation**: Verifies that adjacent Gold and Copper tiles are never cross-mined.
- **Orthogonal vs Diagonal Search**: Tests 4-way vs 8-way neighbor exploration.
- **Pickaxe Tier Requirements**: Tests pickaxe power thresholds for Cobalt (100%), Chlorophyte (200%), etc.

### 2.5 AutoFishing Plugin Tests
- **In-Engine Bite Detection**: Tests condition `ai[1] < 0f && localAI[1] != 0f`.
- **Tick-Based Timing State Machine**: Tests cast delays and humanized reaction timers.
- **Bait Validation**: Tests inventory bait checking across slots 0–57.

### 2.6 FishingLinePlus Plugin Tests
- **Multi-Bobber Calculation**: Tests bobber count clamping and spawn counts.
- **Spread Physics**: Tests fan-angle distribution and velocity variance calculation.
- **Dual-Layer Synchronization**: Validates dynamic bite propagation and reel-in catch table rolls.

### 2.7 Multi-Mod Coexistence Matrix Tests (7 Scenarios)
1. `OreCascade` alone (2 patches)
2. `AutoFishing` alone (1 patch)
3. `FishingLinePlus` alone (3 patches)
4. `OreCascade` + `AutoFishing` (3 patches)
5. `OreCascade` + `FishingLinePlus` (5 patches)
6. `AutoFishing` + `FishingLinePlus` (4 patches)
7. `OreCascade` + `AutoFishing` + `FishingLinePlus` (6 patches simultaneously)
