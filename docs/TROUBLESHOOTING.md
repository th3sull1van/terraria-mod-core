# 🔧 TerrariaModCore (TMC) — Troubleshooting & Diagnostic Guide

This guide provides resolutions for common startup errors, runtime memory issues, and mod configuration diagnostics.

---

## 1. Common Issues & Solutions

### 1.1 `System.OutOfMemoryException` on Startup or Main Menu
- **Symptom**: Game crashes with `System.OutOfMemoryException` inside `Main.DrawMenu` or during high-resolution texture loading.
- **Root Cause**: Terraria is a 32-bit (x86) application. Without the Large Address Aware flag, Windows restricts the process to 2GB of virtual address space.
- **Solution**:
  - TMC applies the PE header flag `IMAGE_FILE_LARGE_ADDRESS_AWARE` (`0x0020`) to `TerrariaModded.exe` during build, expanding virtual address space to **4GB**.
  - Ensure `TerrariaModded.exe.config` is present in your Terraria directory with `<gcServer enabled="true"/>` and `<gcAllowVeryLargeObjects enabled="true"/>`.

### 1.2 `System.TypeInitializationException` on `CaptureManager`
- **Symptom**: Game crashes during `Lighting.Initialize()` / `LegacyLighting.Rebuild()` with inner `NullReferenceException` in `CaptureManager..ctor()`.
- **Root Cause**: `LegacyLighting` queries `CaptureManager` before XNA creates the `GraphicsDevice`.
- **Solution**:
  - TMC includes built-in compatibility patches in `CoreFixPatches.cs` that lazily defer `CaptureCamera` instantiation until `GraphicsDevice` is fully created.

### 1.3 `System.IO.FileNotFoundException: Could not load file or assembly 'Terraria'`
- **Symptom**: `TerrariaModded.exe` fails to start immediately upon execution.
- **Root Cause**: JIT compiler attempted to resolve Terraria types before the dynamic `AssemblyResolve` handler was registered.
- **Solution**:
  - TMC isolates the `AssemblyResolve` registration in `Program`'s static constructor and separates the main logic into a non-inlined method (`[MethodImpl(MethodImplOptions.NoInlining)] RealMain`).

### 1.4 Multi-Line Fishing Only Yields 1 Fish per Cast
- **Symptom**: Multiple bobbers are cast into the water, but only 1 item is retrieved upon reeling in.
- **Root Cause**: In Terraria 1.4.5.7, active floating bobbers have `ai[0] == 0f` and independent bite timers. When one bobber bites and triggers reel-in, other bobbers are pulled out before their timers expire.
- **Solution**:
  - `FishingLinePlus` implements a dual-layer algorithm:
    1. **`BobberSyncPatch`**: Dynamically propagates `FishingCheck()` to all sibling bobbers in water the moment any line hooks a fish.
    2. **`BobberPullPatch`**: Guarantees loot table rolls on all floating bobbers (`ai[0] == 0f`) upon reel-in, ensuring all lines retrieve fish and consume bait legitimately.

---

## 2. Diagnostic Telemetry & Logs

TMC automatically writes all diagnostic logs to:
`<TerrariaDirectory>/TMC/logs/tmc.log`

### Log Levels
- **`[INFO]`**: Standard startup, version verification, and mod lifecycle events.
- **`[DEBUG]`**: Harmony patch application details and configuration saves.
- **`[WARN]`**: Optional dependency notices or non-critical configuration fallbacks.
- **`[ERROR]`**: Mod loading failures or caught exceptions with full stack traces.
