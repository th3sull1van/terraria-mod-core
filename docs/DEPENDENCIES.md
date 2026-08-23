# 📦 TerrariaModCore (TMC) — Dependency & Environment Requirements

This document provides a comprehensive overview of all system prerequisites, runtime dependencies, build toolchains, third-party libraries, and mod-level dependency mechanisms required to run, build, or extend **TerrariaModCore (TMC)**.

---

## 1. Runtime Requirements (For Players & End Users)

To play Terraria with TMC and its mods installed, your machine needs:

| Component | Minimum Version | Recommended | Notes |
| :--- | :--- | :--- | :--- |
| **Operating System** | Windows 7 SP1 / 8.1 / 10 | **Windows 10 / 11 (64-bit)** | 64-bit Windows allows the 32-bit game to utilize the full **4GB LAA memory space**. |
| **Vanilla Game** | **Terraria 1.4.5.7** | Steam or GOG release | Standard Terraria installation folder (Steam / GOG) or specified via `$env:TERRARIA_PATH`. |
| **.NET Runtime** | **.NET Framework 4.8** | .NET Framework 4.8.1 | Pre-installed on Windows 10 (May 2019+) and Windows 11. |
| **Graphics Runtime** | **Microsoft XNA Framework 4.0** | XNA Redistributable 4.0 | Automatically installed with Terraria through Steam/GOG. |

---

## 2. Development & Build Requirements (For Contributors)

To compile the full TMC solution, run the automated test suite, or build custom plugins:

| Tool / SDK | Version | Purpose |
| :--- | :--- | :--- |
| **.NET SDK** | **.NET SDK 10.0+ / 8.0+** | Multi-project build orchestration and CLI commands (`dotnet build`, `dotnet test`). |
| **MSBuild** | **MSBuild 17.0+** | Included with Visual Studio 2022 or Build Tools. |
| **Targeting Pack** | **.NET Framework 4.8 Developer Pack** | Required by MSBuild to compile `net48` projects (`Microsoft.NETFramework.ReferenceAssemblies.net48`). |
| **PowerShell** | **Windows PowerShell 5.1 / pwsh 7+** | Executes `build_dist.ps1` for compilation, test execution, LAA PE patching, and packaging. |
| **C# Compiler** | **C# 7.3+** | Supported across all projects in `TerrariaModCore.sln`. |

---

## 3. Libraries & Assembly Dependencies

TMC uses a strictly controlled set of external dependencies to ensure zero runtime bloat and seamless game execution:

### 3.1 Host & Modding Libraries
- **`Lib.Harmony` (v2.4.2)**:
  - **Path**: `packages/Lib.Harmony.2.4.2/lib/net48/0Harmony.dll`
  - **Role**: Provides in-memory IL patching, method interception, prefixes, postfixes, and transpilers.
  - **Distribution**: Deployed to `<GameRoot>/0Harmony.dll` and `<GameRoot>/TMC/0Harmony.dll`.

- **`TerrariaModCore.API` (v1.0.0)**:
  - **Path**: `src/TerrariaModCore.API/bin/Release/TerrariaModCore.API.dll`
  - **Role**: Zero-dependency interface layer providing `IMod`, `IModContext`, `IPatchManager`, `IConfigManager`, `ILogger`, and lifecycle events.

### 3.2 Game Assemblies (Provided by Terraria)
- **`Terraria.exe`**:
  - The vanilla game binary. Referenced by projects with `Private=False` (do not copy to output).
- **`Microsoft.Xna.Framework.dll`**:
  - Located in the Windows Global Assembly Cache (GAC) or referenced from the Terraria directory.

### 3.3 Test Suite Libraries
- **`Mono.Cecil` (v0.11.5)**:
  - **Role**: Used exclusively within `TerrariaModCore.Tests` for static IL inspection, instruction verification, and reflection analysis without triggering runtime execution.

---

## 4. Mod-Level Dependency Declarations (`manifest.json`)

TMC features a built-in topological dependency resolver (using **Kahn's Algorithm**). When developing a mod, you can declare dependencies in `manifest.json`:

```json
{
  "Id": "my_extension_mod",
  "Name": "My Extension Mod",
  "Version": "1.0.0",
  "Dependencies": [
    "ore_cascade"
  ],
  "OptionalDependencies": [
    "auto_fishing"
  ],
  "LoadBefore": [],
  "LoadAfter": [
    "fishing_line_plus"
  ],
  "IncompatibleWith": [
    "conflicting_mod_id"
  ]
}
```

### Dependency Fields Reference

| Field | Type | Description |
| :--- | :--- | :--- |
| `Dependencies` | `string[]` | **Mandatory** mod IDs. If any listed mod is missing or failed to load, this mod will fail to load and log a descriptive error. |
| `OptionalDependencies` | `string[]` | **Optional** mod IDs. If present, TMC guarantees they are loaded **before** this mod. |
| `LoadBefore` | `string[]` | Declares that this mod must be loaded **before** the specified mod IDs. |
| `LoadAfter` | `string[]` | Declares that this mod must be loaded **after** the specified mod IDs. |
| `IncompatibleWith` | `string[]` | Conflicting mod IDs. If any of these mods are active, resolution aborts to prevent game state corruption. |

---

## 5. Verification Commands

### Check .NET SDK Installation
```powershell
dotnet --version
# Expected: 10.0.x, 8.0.x, or similar
```

### Check .NET Framework 4.8 Installation
```powershell
Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" | Select-Object Release, Version
# Release >= 528040 indicates .NET Framework 4.8 is active
```

### Compile & Verify Entire Project
```powershell
powershell -ExecutionPolicy Bypass -File ".\build_dist.ps1"
```
