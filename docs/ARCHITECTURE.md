# 📐 TerrariaModCore (TMC) — Technical Architecture & Design Specification

This document provides an exhaustive technical specification of the **TerrariaModCore (TMC)** framework, detailing its runtime injection mechanism, memory management model, lifecycle orchestration, centralized Harmony patch isolation, dependency resolution engine, and fault-tolerance boundaries.

---

## 1. High-Level Architecture Overview

TMC enforces a layered, decoupled architecture with strict separation of concerns between core infrastructure, runtime hooks, and mod gameplay modules:

```text
               Terraria.exe (Vanilla Release 1.4.5.7)
                              ▲
                              │ Dynamic Runtime Injection
                      TerrariaModded.exe (Bootstrapper)
                              │
                              ▼
                      TerrariaModCore (Host Engine)
     ┌────────────────────────┬────────────────────────┬────────────────────────┐
     │  Dependency Resolver   │  Central PatchManager  │    ModLoader & Mod     │
     │     (Kahn's DAG)       │    (Harmony 2.4.2)     │        Registry        │
     └────────────────────────┴────────────────────────┴────────────────────────┘
             │                                   │                         │
             ▼                                   ▼                         ▼
     ┌────────────────────────┬────────────────────────┬────────────────────────┐
     │       OreCascade       │      AutoFishing       │    FishingLinePlus     │
     │      (Production)      │      (Production)      │      (Production)      │
     └────────────────────────┴────────────────────────┴────────────────────────┘
```

---

## 2. Core Engine Components

### 2.1 TerrariaModCore.API (`TerrariaModCore.API.dll`)
A standalone contract library without third-party dependencies, declaring interfaces and standard models:
- **`IMod`**: Mod lifecycle interface (`Initialize`, `Load`, `Unload`).
- **`IModContext`**: Injected sandbox providing access to isolated mod logger, configuration manager, centralized patch manager, event bus, and game services.
- **`IPatchManager`**: Centralized registry for Harmony prefixes, postfixes, and transpilers.
- **`IConfigManager`**: Type-safe JSON configuration provider with default fallback and disk serialization.
- **`IModRegistry`**: Runtime query interface for inspecting active mod states and manifests.
- **`ModManifest`**: Schema definition representing `manifest.json`.

### 2.2 Host Engine (`TerrariaModCore.dll`)

#### Mod Discovery & Loader (`ModLoader.cs`)
- Scans `<GameRoot>/mods/` subdirectories for `manifest.json` files.
- Inspects entry assemblies and loads types using reflection.
- Wraps instantiation and execution in protected fault isolation blocks (`try-catch-finally`).

#### Dependency Resolution (`DependencyResolver.cs`)
- Constructs a Directed Acyclic Graph (DAG) of discovered mods.
- Enforces mandatory dependencies (`dependencies`), optional ordering (`optionalDependencies`), and conflict declarations (`incompatibleWith`).
- Computes topological load order using **Kahn's Algorithm** ($O(V + E)$ complexity).
- Detects circular dependency deadlocks (`A -> B -> C -> A`) prior to any assembly execution.

#### Centralized Patch Isolation (`PatchManager.cs`)
- Owns a single `Harmony("com.tmc.host.patcher")` instance.
- Tracks all patches by mod ID, method target, and patch type.
- Supports conflict resolution and priority ordering (`PatchPriority.High`, `PatchPriority.Normal`, `PatchPriority.Low`).
- Enables granular runtime unpatching: calling `UnpatchAll(modId)` removes only that mod's hooks while preserving hooks registered by other mods.

#### Dynamic Assembly Resolver & Launcher (`TerrariaModCore.Launcher`)
- Bootstraps `Terraria.exe` into the runtime `AppDomain`.
- Registers a `ResolveEventHandler` in its static constructor before JIT compilation touches any Terraria types.
- Separates runtime startup into a non-inlined method (`[MethodImpl(MethodImplOptions.NoInlining)] RealMain`), preventing early `TypeInitializationException` during JIT resolution.

---

## 3. Memory Model & Large Address Aware (LAA - 4GB)

### 3.1 32-Bit (x86) Address Space Challenge
Vanilla Terraria is an x86 (32-bit) .NET Framework application. By default, Windows limits 32-bit processes to 2GB of virtual address space. With hundreds of high-resolution textures, sound banks, and world tiles, the process rapidly exhausts contiguous virtual memory, resulting in `System.OutOfMemoryException`.

### 3.2 LAA Patching
During compilation and packaging, `build_dist.ps1` sets the `IMAGE_FILE_LARGE_ADDRESS_AWARE` (`0x0020`) bit in the PE `IMAGE_FILE_HEADER.Characteristics` field of `TerrariaModded.exe`:
- **Default 32-bit header**: `Characteristics = 0x0102` (2GB virtual address limit).
- **LAA-enabled header**: `Characteristics = 0x0122` (**4GB virtual address limit on 64-bit Windows**).

### 3.3 Garbage Collection Tuning (`App.config`)
```xml
<configuration>
  <runtime>
    <gcServer enabled="true" />
    <gcAllowVeryLargeObjects enabled="true" />
  </runtime>
</configuration>
```

---

## 4. Graphics Initialization Protection (`CoreFixPatches.cs`)

### The Pre-Render Race Condition
During vanilla Terraria startup:
1. `Main.ClientInitialize()` calls `LoadSettings()`.
2. `LoadSettings()` invokes `SetDisplayMode()` -> `Lighting.Initialize()` -> `LegacyLighting.Rebuild()`.
3. `LegacyLighting.Rebuild()` queries `CaptureManager.Instance.IsCapturing`.
4. This triggers static construction `CaptureManager..cctor()`, which attempts to instantiate `new CaptureCamera(Main.instance.GraphicsDevice)`.
5. At this moment, XNA's `GraphicsDeviceManager` has not completed device creation (`GraphicsDevice == null`), throwing a fatal `NullReferenceException`.

### CoreFix Intercept
`CoreFixPatches` installs Harmony prefixes on `CaptureManager` constructor and properties:
- If `GraphicsDevice == null` during early boot, `CaptureCamera` allocation is safely deferred.
- When `GraphicsDevice` is initialized on the first render frame, `CaptureCamera` is instantiated transparently on demand.

---

## 5. Mod Lifecycle & State Transitions

```text
       [ Discovered ]
             │
             ▼
        [ Validated ] ─────── (Dependency failure) ───────► [ Failed ]
             │
             ▼
   [ Initialize(context) ] ── (Unhandled exception) ─────► [ Faulted ]
             │
             ▼
         [ Loaded ] ───────── (Runtime unpatching) ───────► [ Unloaded ]
```

1. **`Discovered`**: `manifest.json` located on disk.
2. **`Validated`**: Dependencies resolved without cycles or missing requirements.
3. **`Initialize`**: Configuration loaded; Harmony patches registered via `IModContext.PatchManager`.
4. **`Loaded`**: Mod active; game hooks listening.
5. **`Unloaded`**: Patches unapplied; timers and event subscriptions disposed.
6. **`Faulted`**: Exception intercepted; mod isolated without crashing the host process.
