# 🧪 TerrariaModCore (TMC) — Testing Strategy & Verification Suite

This document describes the automated test architecture, test suites, and assertion coverage for **TerrariaModCore (TMC)**.

---

## 1. Overview & Test Runner

TMC includes a standalone test suite (`TerrariaModCore.Tests.exe`) that executes **97 comprehensive assertions** covering core infrastructure, isolated plugin behaviors, and multi-mod coexistence scenarios.

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
RESULTS: 97 PASSED, 0 FAILED
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
- **Angular Spread Physics**: Tests trigonometry rotation vectors across fan spreads.
- **Dual-Layer Synchronization**: Validates synchronized pull and biting triggers across active bobber pools.

### 2.7 TurboExtractinator Plugin Tests
- **Speed Multiplier Scaling**: Tests item cooldown division (`15 ticks / 5x = 3 ticks`).
- **Single-Frame Cap**: Verifies 1-tick floor at extreme speed settings (`15 ticks / 15x = 1 tick`).
- **Batch Extraction Logic**: Validates extra batch iteration loops when configured.
- **Mod Lifecycle & Patch Isolation**: Verifies full lifecycle and clean unpatching.

### 2.8 Mod Coexistence Matrix Tests (8 Scenarios)
- **Scenarios 1–4**: Individual plugin lifecycle and patch stability.
- **Scenarios 5–7**: Dual-mod synergies (`OreCascade + AutoFishing`, `OreCascade + TurboExtractinator`, `AutoFishing + FishingLinePlus`).
- **Scenario 8**: All 4 production mods active simultaneously with 7 active patches and 0 conflicts.
