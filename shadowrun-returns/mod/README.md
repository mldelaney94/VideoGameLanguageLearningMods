# Shadowrun Returns Language Engage — core BepInEx mod

This project is the **main plugin** (`ShadowrunReturnsLanguageEngage.dll`): Harmony patches on the game’s **NGUI** text pipeline, label registry, and dictionary popup shell. It targets **Unity 4.2.0f4** and **.NET 3.5** under **BepInEx 6**.

For the full solution (preprocessor, contract, Chinese satellite DLL), see **[`../README.md`](../README.md)**.

## What this DLL does

- **Hooks `UIFont.Print` and related paths** — While the game fills vertex and color buffers for a line of text, the mod records which string indices correspond to which rendered quads, and registers data per `UILabel`.
- **Index map** — NGUI does not emit geometry for every code unit (spaces, color markup, **U+200B**, etc.). The mod builds a **quad index → source string index** map so hit-testing and word logic use the same coordinates as the visible glyphs.
- **Hit-testing** — Maps the mouse to a character quad (with collider and depth caveats documented in repo rules), then asks the loaded **`SRLEPlugin`** (if any) for the word under the cursor.
- **Popup** — Presents dictionary-style UI; exact definition string comes from the plugin (`FormatDictionaryDefinition`, etc.).

Word **boundaries** for Chinese depend on markers inserted at **data** time; see **[`../ShadowrunReturnsLanguageEngage.Chinese/README.md`](../ShadowrunReturnsLanguageEngage.Chinese/README.md)** and the **[`../chinese_preprocessor/`](../chinese_preprocessor/)** tool.

## Prerequisites

- **Shadowrun Returns** (Steam or equivalent).
- **BepInEx 6** installed in the game directory.
- **`libs/Assembly-CSharp.dll`** — copy from the game’s `Managed/` folder into `mod/libs/` before building.

## Build

Open **`ShadowrunReturnsLanguageEngage.sln`** in the parent `shadowrun-returns/` directory, or from that directory:

```powershell
dotnet restore ShadowrunReturnsLanguageEngage.sln
dotnet build ShadowrunReturnsLanguageEngage.sln
```

NuGet uses nuget.org, nuget.bepinex.dev, and nuget.samboy.dev.

**Output:** `mod/bin/Debug/net35/ShadowrunReturnsLanguageEngage.dll` (and `ShadowrunReturnsLanguageEngage.Contract.dll` from the contract project).

## Install

Copy the built **`ShadowrunReturnsLanguageEngage.dll`** and **`ShadowrunReturnsLanguageEngage.Contract.dll`** into the game’s `BepInEx/plugins/` folder, together with any **`SRLEPlugin.dll`** (or other satellite) and **`cedict_ts.u8`** if you use the Chinese plugin.

## Tech stack

| Component | Version |
|-----------|---------|
| BepInEx | 6.0.0-be.x |
| HarmonyX | 2.10.x (via BepInEx) |
| Unity | 4.2.0f4 |
| Target runtime | .NET Framework 3.5 |

The `.csproj` sets **`LangVersion`** to `latest` so you can use modern C# while still targeting 3.5.
