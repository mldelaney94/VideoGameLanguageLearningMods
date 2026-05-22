# Shadowrun: Dragonfall — Language Engage

BepInEx mod for *Shadowrun: Dragonfall — Director's Cut* with a contract assembly and optional Chinese satellite plugin (`SRLEPlugin.dll`).

## Build and install

From `shadowrun-dragonfall/`:

```powershell
dotnet build ShadowrunDragonfallLanguageEngage\ShadowrunDragonfallLanguageEngage.sln --no-incremental
if ($?) {
  $dest = "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Dragonfall Director's Cut\BepInEx\plugins\"
  Copy-Item "ShadowrunDragonfallLanguageEngage\bin\Debug\net35\ShadowrunDragonfallLanguageEngage.dll" $dest -Force
  Copy-Item "ShadowrunDragonfallLanguageEngage.Contract\bin\Debug\ShadowrunDragonfallLanguageEngage.Contract.dll" $dest -Force
  Copy-Item "ShadowrunDragonfallLanguageEngage.Chinese\bin\Debug\SRLEPlugin.dll" $dest -Force
}
```

Adjust `$dest` if your Steam library is elsewhere. For the Chinese plugin dictionary popup, also place **`cedict_ts.u8`** in the same `BepInEx/plugins/` folder.
