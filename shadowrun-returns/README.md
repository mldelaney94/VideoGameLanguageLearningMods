# Shadowrun Returns — Language Engage

This folder adds **Chinese language-learning** support to *Shadowrun Returns*: processed dialogue strings, a BepInEx runtime that understands those strings, and optional behavior delivered through a **plugin architecture** so language-specific logic stays in small satellite DLLs.

## How the pieces fit together

| Piece | Role |
|--------|------|
| **[`chinese_preprocessor/`](chinese_preprocessor/)** | Python pipeline: reads official Chinese `.po` files, runs segmentation and pinyin, writes `.mo` files the game loads as translations. |
| **[`mod/`](mod/)** | **Main BepInEx plugin** (`ShadowrunReturnsLanguageEngage.dll`): hooks NGUI text rendering, builds per-label geometry and index maps, drives hover and dictionary UI. It does **not** hard-code Chinese rules. |
| **[`ShadowrunReturnsLanguageEngage.Contract/`](ShadowrunReturnsLanguageEngage.Contract/)** | Shared **contract** assembly: abstract `SRLEPlugin` API the main mod calls (word boundaries, label formatting, popup content, `Init`). Both the main mod and satellite plugins reference this DLL. |
| **[`ShadowrunReturnsLanguageEngage.Chinese/`](ShadowrunReturnsLanguageEngage.Chinese/)** | Reference **Chinese satellite plugin** (`SRLEPlugin.dll`): implements `SRLEPlugin` (CEDICT-backed popup text, punctuation boundaries, label tweaks). Swappable for other languages or experiments. |

At startup the main mod loads `SRLEPlugin.dll` from the BepInEx **plugins** folder (same place as the main DLL). If loading fails, the game still runs; features that depend on the plugin are disabled until the DLL and its dependencies are fixed.

## Plugin architecture (why the contract exists)

- The **main mod** stays stable and game-specific (Unity 4.2 / NGUI / Harmony).
- **Language or curriculum policy** (what counts as a word, how popups look, which strings get special formatting) lives in **plugins** that subclass `SRLEPlugin` in the contract assembly.
- The main mod and every plugin must ship a **matching** `ShadowrunReturnsLanguageEngage.Contract.dll` (same API surface).

More detail for each layer:

- **Game + pipeline overview (this file)** — you are here.
- **[`mod/README.md`](mod/README.md)** — BepInEx build, install, hooks, and runtime behavior of the core mod.
- **[`ShadowrunReturnsLanguageEngage.Chinese/README.md`](ShadowrunReturnsLanguageEngage.Chinese/README.md)** — Chinese plugin, **zero-width spaces (U+200B)** and how they tie the preprocessor to `ExtractWord` / `IsBoundary`.

## Build and deploy (quick reference)

From `shadowrun-returns/`:

```powershell
dotnet build ShadowrunReturnsLanguageEngage.sln --no-incremental
if ($?) {
  $dest = "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Returns\BepInEx\plugins\"
  Copy-Item "mod\bin\Debug\net35\ShadowrunReturnsLanguageEngage.dll" $dest -Force
  Copy-Item "mod\bin\Debug\net35\ShadowrunReturnsLanguageEngage.Contract.dll" $dest -Force
  Copy-Item "ShadowrunReturnsLanguageEngage.Chinese\bin\Debug\SRLEPlugin.dll" $dest -Force
}
```

Adjust `$dest` to your install path. You also need **`cedict_ts.u8`** (shared CEDICT data) in `BepInEx/plugins/` for the Chinese plugin’s dictionary popup. Translation output from the preprocessor follows the game’s normal localization layout (see [`chinese_preprocessor/README.md`](chinese_preprocessor/README.md)).

## Requirements (high level)

- *Shadowrun Returns* with **BepInEx 6** in the game folder.
- Main mod build: **`Assembly-CSharp.dll`** from the game’s `Managed/` folder, copied into `mod/libs/` (see mod README).
