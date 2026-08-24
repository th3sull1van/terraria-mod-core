<div align="center">

# 🌌 TerrariaModCore (TMC) — High-Performance Modding Framework for Vanilla Terraria 1.4.5.8 / 1.4.5.7

**A robust, modular, zero-tModLoader plugin framework and runtime injector with Harmony patch isolation, dependency resolution, 4GB LAA memory management, and built-in mods.**

<p align="center">
  <a href="README.md"><b>English</b></a> •
  <a href="README_pt-BR.md"><b>Português (Brasil)</b></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Terraria-1.4.5.8_|_1.4.5.7-22c55e?style=for-the-badge&logo=steam&logoColor=white" alt="Terraria 1.4.5.8 / 1.4.5.7">
  <img src="https://img.shields.io/badge/Framework-Harmony_2.4.2-6366f1?style=for-the-badge&logo=csharp&logoColor=white" alt="Harmony 2.4.2">
  <img src="https://img.shields.io/badge/Vanilla-Zero_tModLoader-06b6d4?style=for-the-badge" alt="Zero tModLoader">
  <img src="https://img.shields.io/badge/Memory-4GB_LAA_Enabled-f59e0b?style=for-the-badge" alt="4GB LAA Enabled">
  <img src="https://img.shields.io/badge/Tests-392_Passing-10b981?style=for-the-badge" alt="392 Tests Passing">
  <img src="https://img.shields.io/badge/License-MIT-3b82f6?style=for-the-badge" alt="License MIT">
</p>

<br>

<img src="https://terraria.org/static/media/logo.734118ae.png" width="360" alt="Terraria Logo">

<br>
<br>

</div>

---

## 🌟 Key Architecture & Capabilities

- **⚡ Zero tModLoader Dependency & 100% Vanilla File Integrity**:
  - Operates directly on the official **Terraria 1.4.5.8 / 1.4.5.7** release (Steam & GOG).
  - Original `Terraria.exe` remains **100% untouched and unpatched on disk** (SHA256 verified).
  - Clean separation: launch `TerrariaModded.exe` to play with mods, or `Terraria.exe` for pure vanilla gameplay.

- **🛡️ Harmony Patch Isolation & Conflict Management**:
  - Centralized patch manager (`IPatchManager`) wraps Harmony 2.4.2, tracking every prefix, postfix, and transpiler by mod ID.
  - Granular runtime unpatching: disabling or unloading a mod cleanly restores the original IL without affecting other active mods.

- **🔀 Topological Dependency Resolution (Kahn's Algorithm)**:
  - Supports mandatory dependencies (`dependencies`), optional dependencies (`optionalDependencies`), load-ordering (`loadBefore`, `loadAfter`), and explicit conflict prevention (`incompatibleWith`).
  - Automatically calculates the optimal load order and detects circular dependency deadlocks.

- **🛡️ Fault Isolation & Safe Mode Protection**:
  - If a mod throws an unhandled exception during initialization or loading, the fault is isolated, the mod is marked as `Faulted`, and its patches are unapplied without crashing the host game.

- **🧠 Large Address Aware (4GB Virtual Address Space)**:
  - Launcher is compiled with the PE `IMAGE_FILE_LARGE_ADDRESS_AWARE` (`0x0020`) flag, giving the 32-bit engine the full 4GB virtual address space required to eliminate `OutOfMemoryException`.

- **🎨 Early Graphics Initialization Guard**:
  - Includes built-in engine patches protecting early display mode setup from pre-render `GraphicsDevice` race conditions.

---

## 🎮 Included Production Plugins

| Mod | Description | Documentation |
| :--- | :--- | :--- |
| **⛏️ OreCascade** | Instant chain-mining for ores and gemstones using iterative Breadth-First Search (BFS), strict vein isolation, and legitimate vanilla drop preservation. | [OreCascade README](src/mods/OreCascade/README.md) |
| **🎣 AutoFishing** | Intelligent automated casting, in-engine bite detection (`ai[1] < 0`), and reel-in execution synchronized with the 60 TPS game loop. | [AutoFishing README](src/mods/AutoFishing/README.md) |
| **🎣 FishingLinePlus** | Multiple simultaneous functional fishing lines with angular velocity spread physics, dual-layer catch synchronization, and multi-catch mechanics. | [FishingLinePlus README](src/mods/FishingLinePlus/README.md) |
| **⚡ TurboExtractinator** | Accelerates Extractinator and Chlorophyte Extractinator processing speeds by a configurable multiplier (default 5x) with batch extraction support. | [TurboExtractinator README](src/mods/TurboExtractinator/README.md) |
| **🧪 AutoBuff** | Automatically consumes buff potions and food from inventory and Void Bag when buff durations expire, ensuring continuous uptime with zero waste. | [AutoBuff README](src/mods/AutoBuff/README.md) |
| **📦 AutoOpen** | Continuous, rapid automated opening of grab bags, fishing crates, oysters, boss bags, lockboxes, and presents on hold-right-click (Extractinator-style). | [AutoOpen README](src/mods/AutoOpen/README.md) |
| **🔬 AutoResearch** | Automated Journey Mode item sacrifice/research upon inventory entry, preserving 100% of vanilla quantity requirements with zero manual sacrifice clicks. | [AutoResearch README](src/mods/AutoResearch/README.md) |
| **🐷 PiggyVault** | Void Bag-like auto-pickup, direct crafting, quick actions, and info accessory capabilities directly for the Piggy Bank. | [PiggyVault README](src/mods/PiggyVault/README.md) |
| **🪣 TurboBucket** | Instant 60 TPS liquid bucket pouring, continuous flow, and accelerated bottomless bucket operations. | [TurboBucket README](src/mods/TurboBucket/README.md) |
| **🎯 BossCursor** | Real-time directional indicator arrows and boss head icons pointing toward active bosses and mini-bosses with proximity scaling. | [BossCursor README](src/mods/BossCursor/README.md) |

---

## 🚀 Quick Start

### 1. Build from Source
```powershell
# Compiles solution (Release|x86), executes all 378 tests, and builds distribution
powershell -ExecutionPolicy Bypass -File "build_dist.ps1"
```

### 2. Install to Terraria
```powershell
# Copy the compiled distribution package into your Terraria folder:
Copy-Item -Path "dist\*" -Destination "D:\Jogos\Steam\steamapps\common\Terraria" -Recurse -Force
```

### 3. Launch the Game
- 🎮 **Modded Experience**: Launch via `TerrariaModded.exe` (or your desktop shortcut).
- 🛡️ **Pure Vanilla**: Launch `Terraria.exe` directly.

---

## ⚙️ Configuration Reference

### TMC Host Engine (`TMC/config/core.json`)

```json
{
  "LogLevel": "Info",
  "DiagnosticBannerOnStartup": true,
  "StrictCompatibilityCheck": true,
  "SafeModeOnModFailure": true,
  "ModsDirectoryName": "mods"
}
```

| Option | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `LogLevel` | `string` | `"Info"` | Logging verbosity: `"Trace"`, `"Debug"`, `"Info"`, `"Warn"`, `"Error"`, `"Fatal"`. |
| `DiagnosticBannerOnStartup` | `bool` | `true` | Displays startup diagnostic banner with active mod counts and versions. |
| `StrictCompatibilityCheck` | `bool` | `true` | Verifies Terraria version matches target 1.4.5.8 / 1.4.5.7 before booting. |
| `SafeModeOnModFailure` | `bool` | `true` | Isolates failing mods and continues loading healthy mods. |
| `ModsDirectoryName` | `string` | `"mods"` | Folder name containing plugin subdirectories. |

---

## 📁 Repository Structure

```text
terraria_mod_core/
├── .gitignore                          # Git ignore rules for C#, Visual Studio, and binaries
├── build_dist.ps1                      # Automated build, test runner, and distribution packager
├── LICENSE                             # MIT License
├── TerrariaModCore.sln                 # Visual Studio Solution (.NET Framework 4.8 / x86)
├── README.md                           # Master English Documentation
├── README_pt-BR.md                     # Master Brazilian Portuguese Documentation
│
├── docs/                               # Extended Technical Documentation
│   ├── ARCHITECTURE.md                 # Technical design, memory layout, and patch isolation
│   ├── MODDING.md                      # Developer guide for creating TMC plugins
│   ├── COMPATIBILITY.md                # Hook matrix and runtime version verification
│   ├── CONFIGURATION.md                # Configuration reference for Core and all mods
│   ├── TESTING.md                      # 378-assertion automated test suite breakdown
│   └── TROUBLESHOOTING.md              # Diagnostics for memory, graphics, and runtime errors
│
├── src/
│   ├── TerrariaModCore.API/            # Public Modding API (Contracts, Interfaces, Types)
│   │   ├── IMod.cs                     # Lifecycle interface (Initialize, Load, Unload)
│   │   ├── IModContext.cs              # Mod sandbox context
│   │   ├── IPatchManager.cs            # Central Harmony patch registration interface
│   │   ├── IConfigManager.cs           # Generic JSON configuration manager
│   │   └── TerrariaModCore.API.csproj
│   │
│   ├── TerrariaModCore/                # Core Host Engine (Runtime Injector & Lifecycle)
│   │   ├── ModEngine.cs                # Mod discovery, dependency resolution, and execution
│   │   ├── Patching/                   # Harmony patch manager & core compatibility fixes
│   │   ├── Dependencies/               # Kahn's topological sort & cycle detection
│   │   └── TerrariaModCore.csproj
│   │
│   ├── TerrariaModCore.Launcher/       # Modded Bootstrapper (TerrariaModded.exe)
│   │   ├── Program.cs                  # Dynamic AssemblyResolver & entry point
│   │   ├── App.config                  # Server GC and 64-bit object allocation settings
│   │   └── TerrariaModCore.Launcher.csproj
│   │
│   └── mods/                           # Included Production Plugins
│       ├── OreCascade/                 # VeinMiner / Ore Excavator plugin
│       ├── AutoFishing/                # Smart fishing automation plugin
│       ├── FishingLinePlus/            # Multi-line / multi-bobber fishing plugin
│       ├── TurboExtractinator/         # High-speed Extractinator acceleration plugin
│       ├── AutoBuff/                   # Automatic buff & potion replenishment plugin
│       ├── AutoOpen/                   # Rapid container & grab bag opener plugin
│       ├── AutoResearch/               # Automated Journey Mode research plugin
│       ├── PiggyVault/                 # Piggy Bank Void Bag capabilities plugin
│       ├── TurboBucket/                # High-speed liquid bucket pouring plugin
│       └── BossCursor/                 # Directional arrow & boss head pointer plugin
│
└── tests/
    └── TerrariaModCore.Tests/          # 378-Assertion Automated Test Suite
        ├── Program.cs                  # Standalone test runner
        ├── DependencyResolverTests.cs  # Dependency ordering & cycle tests
        ├── PatchManagerTests.cs        # Harmony prefix/postfix/unpatch tests
        ├── FaultIsolationTests.cs      # Crash containment & SafeMode tests
        ├── ConfigManagerTests.cs       # Config serialization & GameVersionChecker tests
        ├── OreCascadePluginTests.cs    # BFS vein-mining & pickaxe power tests
        ├── AutoFishingPluginTests.cs   # Fishing state machine & bite detection tests
        ├── FishingLinePlusPluginTests.cs # Spread physics & multi-catch tests
        ├── TurboExtractinatorPluginTests.cs # Speed scaling & batch extraction tests
        ├── AutoBuffPluginTests.cs      # Potion selection & buff replenishment tests
        ├── AutoOpenPluginTests.cs      # Grab bag opening & extractinator-style tests
        ├── AutoResearchPluginTests.cs  # Journey Mode sacrifice & inventory scan tests
        ├── PiggyVaultPluginTests.cs    # Piggy Bank pickup, crafting, and action tests
        ├── TurboBucketPluginTests.cs   # Instant bucket pouring & sponge tests
        ├── BossCursorPluginTests.cs    # Boss detection, proximity, and pointer tests
        └── ModCoexistenceTests.cs      # 16 multi-mod coexistence scenarios
```

---

## 📖 Extended Documentation

- 📐 **[Technical Architecture & Design](docs/ARCHITECTURE.md)**: Runtime injection, memory model, and patch management.
- 📦 **[Dependency & Environment Requirements](docs/DEPENDENCIES.md)**: System prerequisites, .NET toolchains, packages, and mod manifest dependencies.
- 🛠️ **[Mod Developer Guide](docs/MODDING.md)**: Complete walkthrough for building custom TMC plugins.
- 🔍 **[Compatibility & Patch Matrix](docs/COMPATIBILITY.md)**: Intercepted IL methods and version validation.
- ⚙️ **[Configuration Reference](docs/CONFIGURATION.md)**: Settings and presets for host and all plugins.
- 🧪 **[Testing Strategy](docs/TESTING.md)**: 378-assertion automated test breakdown.
- 🔧 **[Troubleshooting Guide](docs/TROUBLESHOOTING.md)**: Resolution for memory limits, startup issues, and log telemetry.

---

## 📄 License

This project is open-source and licensed under the [MIT License](LICENSE).
