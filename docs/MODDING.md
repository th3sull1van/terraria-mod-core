# 🛠️ TerrariaModCore (TMC) — Plugin Developer Guide

This guide walks you through creating, configuring, testing, and deploying custom plugins for the **TerrariaModCore (TMC)** framework.

---

## 1. Prerequisites & Project Setup

### Required Tools
- **Visual Studio 2022** or **.NET SDK 10.0+** with .NET Framework 4.8 targeting pack (see [Dependency Guide](DEPENDENCIES.md)).
- Terraria 1.4.5.7 installation.

### Project Template (`.csproj`)
Create a new Class Library targeting **.NET Framework 4.8** on **x86 Platform**:

```xml
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Release</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">x86</Platform>
    <OutputType>Library</OutputType>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <AssemblyName>MyCustomMod</AssemblyName>
    <RootNamespace>MyCustomMod</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="Microsoft.Xna.Framework">
      <HintPath>C:\Windows\Microsoft.NET\assembly\GAC_32\Microsoft.Xna.Framework\v4.0_4.0.0.0__842cf8be1de50553\Microsoft.Xna.Framework.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="0Harmony">
      <HintPath>..\..\packages\Lib.Harmony.2.4.2\lib\net48\0Harmony.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="Terraria">
      <HintPath>$(TerrariaPath)\Terraria.exe</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\TerrariaModCore.API\TerrariaModCore.API.csproj">
      <Project>{A1B2C3D4-0001-4000-8000-000000000001}</Project>
      <Name>TerrariaModCore.API</Name>
      <Private>False</Private>
    </ProjectReference>
  </ItemGroup>

  <ItemGroup>
    <None Include="manifest.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
  
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>
```

---

## 2. Declaring the Plugin Manifest (`manifest.json`)

Every mod must declare a `manifest.json` file in its root directory:

```json
{
  "Id": "my_custom_mod",
  "Name": "My Custom Mod",
  "Version": "1.0.0",
  "Author": "YourName",
  "Description": "Adds custom mechanics to vanilla Terraria.",
  "EntryAssembly": "MyCustomMod.dll",
  "EntryType": "MyCustomMod.MyModEntry",
  "TargetGameVersion": "1.4.5.7",
  "Enabled": true,
  "Dependencies": [],
  "OptionalDependencies": [],
  "LoadBefore": [],
  "LoadAfter": [],
  "IncompatibleWith": []
}
```

---

## 3. Implementing the Lifecycle (`IMod`)

```csharp
using System;
using TerrariaModCore.API;

namespace MyCustomMod
{
    public class MyModEntry : IMod
    {
        public static MyModEntry Instance { get; private set; }
        public IModContext Context { get; private set; }
        public MyModConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context;

            // Load or generate default configuration
            Config = context.ConfigManager.Get<MyModConfig>();

            context.Logger.Info($"MyCustomMod initialized (Enabled: {Config.Enabled})");

            if (Config.Enabled)
            {
                // Register all Harmony patches annotated in this assembly
                context.PatchManager.RegisterAll(context.Manifest.Id, GetType().Assembly);
            }
        }

        public void Load()
        {
            if (Config.Enabled)
            {
                Context.Logger.Info("MyCustomMod loaded and active.");
            }
        }

        public void Unload()
        {
            Context.Logger.Info("MyCustomMod unloaded.");
        }
    }
}
```

---

## 4. Defining Configuration Data (`MyModConfig.cs`)

```csharp
namespace MyCustomMod
{
    public class MyModConfig
    {
        public bool Enabled { get; set; } = true;
        public int Multiplier { get; set; } = 2;
        public double Probability { get; set; } = 0.75;
    }
}
```

---

## 5. Creating Harmony Patches

Create dedicated patch classes annotated with `[HarmonyPatch]`:

```csharp
using HarmonyLib;
using Terraria;

namespace MyCustomMod.Patches
{
    [HarmonyPatch(typeof(Player), "Update")]
    public static class PlayerUpdatePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i)
        {
            var mod = MyModEntry.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled) return;

            // Only run for the local active player
            if (i != Main.myPlayer || __instance.dead) return;

            // Custom gameplay logic here
        }
    }
}
```

---

## 6. Testing & Deployment

1. Compile the project: `dotnet build -c Release -p:Platform="x86"`.
2. Place the resulting folder inside `<TerrariaDirectory>/mods/my_custom_mod/`.
3. Launch `TerrariaModded.exe`.
4. Inspect `<TerrariaDirectory>/TMC/logs/tmc.log` to confirm initialization.
