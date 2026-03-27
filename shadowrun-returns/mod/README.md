# Shadowrun Returns — Language Engage Mod

BepInEx mod for **Shadowrun Returns** (Unity 4.2.0f4) that adds Chinese language-learning support. Hooks into the game's NGUI text rendering pipeline to enable per-word interaction, hover detection, and (planned) dictionary popup.

## Features

- **Word boundary detection** — consumes zero-width spaces (U+200B) pre-inserted into `.po` files by the [preprocessor](../preprocessor/) to identify word boundaries at runtime.
- **Vertex hit-testing** — maps mouse position to individual character quads in the NGUI rendering buffer, identifying which word the user is hovering over.
- **Character-to-string index mapping** — bridges the gap between rendered glyph indices and source string indices (many characters like ZWS, spaces, and color codes produce no vertices).

## Prerequisites

- **Shadowrun Returns** (Steam)
- **BepInEx 6** installed into the game directory
- `Assembly-CSharp.dll` from the game's `Managed/` folder, placed in `libs/`

## Build

Open `ShadowrunReturnsLanguageEngage.sln` in Visual Studio 2022 (or build via CLI):

```bash
dotnet restore
dotnet build; if ($?) { copy "bin\Debug\net35\ShadowrunReturnsLanguageEngage.dll" "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Returns\BepInEx\plugins\" -Force }
```

NuGet restores from nuget.org, nuget.bepinex.dev, and nuget.samboy.dev.

Output: `bin/Debug/net35/ShadowrunReturnsLanguageEngage.dll`

## Install

Copy the built DLL to your BepInEx `plugins/` folder inside the Shadowrun Returns game directory.

## Tech Stack

| Component | Version |
|-----------|---------|
| BepInEx | 6.0.0-be.754 |
| HarmonyX | 2.10.2 |
| Unity | 4.2.0f4 |
| .NET Framework | 3.5 |

