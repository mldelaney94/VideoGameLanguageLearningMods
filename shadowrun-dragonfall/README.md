# Shadowrun: Dragonfall — Language Engage

BepInEx mod for *Shadowrun: Dragonfall — Director's Cut* with a contract assembly and optional Chinese satellite plugin (`SRDFPlugin.dll`).

Special thanks to https://github.com/fmwizard/shadowrun-dragonfall-zh for hosting the Chinese community translation used in this plugin

## Build and install

From `shadowrun-dragonfall/`:

```powershell
dotnet build ShadowrunDragonfallLanguageEngage\ShadowrunDragonfallLanguageEngage.sln --no-incremental
if ($?) {
  $dest = "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Dragonfall Director's Cut\BepInEx\plugins\"
  Copy-Item "ShadowrunDragonfallLanguageEngage\bin\Debug\net35\ShadowrunDragonfallLanguageEngage.dll" $dest -Force
  Copy-Item "ShadowrunDragonfallLanguageEngage.Contract\bin\Debug\net35\ShadowrunDragonfallLanguageEngage.Contract.dll" $dest -Force
  Copy-Item "ShadowrunDragonfallLanguageEngage.Chinese\bin\Debug\net35\SRDFPlugin.dll" $dest -Force
}
```

Adjust `$dest` if your Steam library is elsewhere. The Chinese plugin loads **`cedict_ts.u8`** from `BepInEx/plugins/` at startup (the copy step above uses the bundled `CEDictText.txt`). For a newer dictionary, download from [MDBG CEDICT](https://www.mdbg.net/chinese/dictionary?page=cedict) and replace that file.
