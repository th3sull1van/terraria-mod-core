# 🤖 AGENTS.md — AI & Contributor Engineering Guidelines for TerrariaModCore (TMC)

This document establishes the mandatory architecture guidelines, engineering standards, engine invariants, and known domain traps for any AI agent or contributor developing, testing, or maintaining the **TerrariaModCore (TMC)** codebase.

---

## 1. Project Mission & Architecture

**TerrariaModCore (TMC)** is a high-performance, modular runtime modding framework for **Vanilla Terraria 1.4.5.8 / 1.4.5.7** (Steam & GOG) that requires **zero tModLoader dependency** and preserves **100% vanilla disk integrity**.

### Component Hierarchy
```text
TerrariaModCore.sln
├── src/
│   ├── TerrariaModCore.API/        # Pure contracts, interfaces, and shared models (No 3rd-party deps)
│   ├── TerrariaModCore/            # Core host engine (ModLoader, DependencyResolver, PatchManager)
│   ├── TerrariaModCore.Launcher/   # Standalone bootstrapper (TerrariaModded.exe, LAA 4GB, AssemblyResolve)
│   └── mods/
│       ├── OreCascade/                 # Iterative BFS VeinMiner & Gem Excavator plugin
│       ├── AutoFishing/                # 60 TPS native state machine fishing automation plugin
│       ├── FishingLinePlus/            # Multi-line fishing with dual-layer catch synchronization
│       ├── TurboExtractinator/         # High-speed Extractinator acceleration plugin
│       ├── AutoBuff/                   # Automatic buff potion & food replenishment plugin
│       ├── AutoOpen/                   # Rapid automated grab bag & container opener plugin
│       ├── AutoResearch/               # Automated Journey Mode item research & sacrifice plugin
│       ├── PiggyVault/                 # Void Bag capabilities & storage automation for Piggy Bank
│       ├── TurboBucket/                # High-speed liquid bucket pouring acceleration plugin
│       └── BossCursor/                 # Real-time directional indicator & boss head pointer plugin
├── tests/
│   └── TerrariaModCore.Tests/      # Standalone 370+ assertion automated test suite
├── docs/                           # Comprehensive technical documentation
├── dist/                           # Assembled release distribution
└── build_dist.ps1                  # Master compilation, test execution, and packaging script
```

---

## 2. Technology Stack & Target Environment

| Property | Value | Notes |
| :--- | :--- | :--- |
| **Target Game** | `Terraria 1.4.5.8` (1.4.5.x) | Auto-detected Steam/GOG install or `$env:TERRARIA_PATH` |
| **Target Framework** | `.NET Framework 4.8` | Target across all projects |
| **Architecture** | `x86 (32-bit)` | Must target `Platform="x86"` (Terraria is a 32-bit application) |
| **Memory Model** | `4 GB LAA` | PE Header flag `IMAGE_FILE_LARGE_ADDRESS_AWARE` (`0x0020`) |
| **Patch Engine** | `Lib.Harmony 2.4.2` | Centralized via `IPatchManager` |
| **Language Standards** | `C# 7.3+ / English` | All code, docstrings, and documentation must be in English |

---

## 3. Non-Negotiable Engineering Invariants

1. **Zero Vanilla File Modification**:
   - `Terraria.exe` on disk must remain completely untouched (SHA256 verified).
   - Never write patchers or installers that modify vanilla binaries or XNB content on disk.
   - All runtime modifications must occur strictly in memory via Harmony IL injection.

2. **Centralized Harmony Patch Management**:
   - Mods must **NEVER** instantiate private `new Harmony("...")` instances.
   - All patches must be registered through `IModContext.PatchManager.RegisterAll(...)` or explicit `RegisterPrefix`/`RegisterPostfix` calls.
   - This ensures full traceability by mod ID, priority ordering, and clean unpatching via `UnpatchAll(modId)`.

3. **Large Address Aware (LAA) Invariant**:
   - The compiled `TerrariaModded.exe` must always have the PE header flag `IMAGE_FILE_LARGE_ADDRESS_AWARE` (`0x0020`) applied (`Characteristics: 0x0122`).
   - `build_dist.ps1` automatically applies this flag. If you modify build scripts, you **must** preserve LAA patching to prevent `OutOfMemoryException` at 2GB.

4. **Thread-Static Reentrancy Guards**:
   - Any patch that triggers native vanilla methods that might re-enter that same hook (e.g. `WorldGen.KillTile` during chain-mining, or `ItemCheck_Shoot` during multi-cast) **must** use a `[ThreadStatic]` reentrancy guard boolean.

5. **Strict English Standards**:
   - All source code comments, XML docstrings, logs, commit messages, and markdown documentation must be written in clear, professional English (with optional `_pt-BR.md` localized counterparts).

6. **Mandatory Post-Build Game Directory Deployment**:
   - The assembled release distribution (`dist/`) **must always be copied/deployed** to the target game directory (`$env:TERRARIA_PATH` or auto-detected Steam/GOG directory) immediately after compilation.
   - `build_dist.ps1` automatically performs this synchronization upon assembling `dist/`. When performing manual builds or isolated edits, contributors and AI agents **must** ensure the latest binaries, configs, and manifests are copied to the game folder so runtime testing is always accurate.

---

## 4. Known Domain Traps & Engine Quirks

### 4.1 JIT Assembly Resolution Boundary
- **Trap**: Referencing any `Terraria.*` type inside `Program.Main` causes the .NET JIT compiler to attempt loading `Terraria.exe` before the method body runs. If `AssemblyResolve` is registered inside `Main`, the application crashes with `System.IO.FileNotFoundException`.
- **Rule**: Always register `AppDomain.CurrentDomain.AssemblyResolve` in the static constructor `static Program()` and place all logic that touches `Terraria` inside a separate non-inlined method marked `[MethodImpl(MethodImplOptions.NoInlining)] RealMain()`.

### 4.2 Early Pre-Render GraphicsDevice Race (`CaptureManager`)
- **Trap**: During `Main.ClientInitialize()`, `LoadSettings()` invokes `Lighting.Initialize()` -> `LegacyLighting.Rebuild()`. This queries `CaptureManager.Instance.IsCapturing`, forcing `CaptureManager..cctor()` to run before XNA creates `Main.instance.GraphicsDevice`, crashing with `NullReferenceException`.
- **Rule**: TMC Core includes `CoreFixPatches.cs` to lazily defer `CaptureCamera` allocation until `GraphicsDevice` is valid. Never remove or bypass this core hook.

### 4.3 Fishing Bobber AI State Meanings (Terraria 1.4.5.7)
- **`bobber.ai[0] == 0f`**: Bobber is actively **floating in water** waiting for fish.
- **`bobber.ai[0] == 1f`**: Bobber is **being reeled in** (retraction animation).
- **`bobber.ai[1] > 0f`**: Countdown timer until next bite roll.
- **`bobber.ai[1] < 0f && bobber.localAI[1] != 0f`**: **Active bite state** with hooked item ID.
- **Rule**: When querying active lines in water, always check `p.ai[0] == 0f && p.wet`.

---

## 5. Standard Development & Verification Workflows

### 5.1 Build Full Solution, Run Tests & Auto-Deploy
```powershell
# Compiles solution (Release|x86), runs test suite, builds dist/, and auto-deploys to game directory
powershell -ExecutionPolicy Bypass -File ".\build_dist.ps1"
```

### 5.2 Direct Test Suite Execution
```powershell
dotnet build tests/TerrariaModCore.Tests/TerrariaModCore.Tests.csproj -c Release -p:Platform="x86"
& "tests/TerrariaModCore.Tests/bin/Release/TerrariaModCore.Tests.exe"
```

### 5.3 Deploying to Game Directory
```powershell
$source = ".\dist"
$target = if ($env:TERRARIA_PATH) { $env:TERRARIA_PATH } else { "D:\Jogos\Steam\steamapps\common\Terraria" }
Copy-Item -Path "$source\*" -Destination $target -Recurse -Force
```

### 5.4 Inspecting Diagnostic Logs
Check `<TerrariaDirectory>/TMC/logs/tmc.log` for initialization output and mod lifecycle telemetry.

---

## 6. Mod Creation Checklist

When implementing or editing a plugin:
- [ ] Create `manifest.json` with unique lowercase `Id`, `EntryAssembly`, and `EntryType`.
- [ ] Implement `IMod` (`Initialize`, `Load`, `Unload`).
- [ ] Bind configuration via `context.ConfigManager.Get<TConfig>()`.
- [ ] Register all patches with `context.PatchManager.RegisterAll(context.Manifest.Id, assembly)`.
- [ ] Include a dedicated, polished `README.md` inside the mod's folder (e.g. `src/mods/MyMod/README.md`).
- [ ] Add corresponding unit and coexistence test cases to `TerrariaModCore.Tests`.
- [ ] Verify that all 300+ tests pass with zero failures.
- [ ] Confirm distribution is deployed to the target game directory.
