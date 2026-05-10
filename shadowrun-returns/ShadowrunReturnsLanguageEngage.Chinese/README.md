# SRLEPlugin — Chinese satellite (`ShadowrunReturnsLanguageEngage.Chinese`)

This assembly builds **`SRLEPlugin.dll`**, a small plugin loaded at runtime by the main BepInEx mod. It subclasses **`SRLEPlugin`** from `ShadowrunReturnsLanguageEngage.Contract` and implements Chinese-specific behavior: CEDICT-backed popups, label formatting, and **word boundaries** that assume the translation strings were produced by the **[`../chinese_preprocessor/`](../chinese_preprocessor/)** pipeline.

For solution-wide context, see **[`../README.md`](../README.md)**.

## Zero-width space (U+200B) — why it exists

Written Chinese does not put spaces between words. To let the game mod treat “one word” as a learner-friendly unit, the **preprocessor** joins each run of Chinese output from the segmenter with **zero-width space** (`\u200B`):

```223:228:../chinese_preprocessor/main.py
    chinese = '\u200B'.join(chinese)
    pinyin = ' '.join(pinyin)
    if (chinese.count('\n') > 1):
      pofile[idx].msgstr = chinese + '\n\n' + pinyin
    else:
      pofile[idx].msgstr = chinese + '\n' + pinyin
```

So the **first line** of a `msgstr` looks like one continuous line of characters to the player, but encodes **implicit word boundaries** wherever the preprocessor inserted U+200B.

## What the runtime does with U+200B

1. **NGUI does not draw a glyph for U+200B** — `GetGlyph` fails; those code units contribute **no vertices**. The main mod’s print hook **skips** ZWS when building geometry so quad indices stay aligned with visible characters only.
2. **The index map** maps each **quad** back to a **string index** that may sit on a normal Han character; ZWS never occupies a quad in that map (see **`../mod/Features/Text/UIFontPrintPatchTests.cs`** — assertions that mapped indices are not ZWS).
3. **`IsBoundary`** in this plugin (and the default on the contract base) treats **`'\u200B'`** as a boundary so **`ExtractWord`** scans left/right until it hits ZWS, whitespace, punctuation, or NGUI markup characters — yielding a substring that is one “word” for lookup and highlighting.

If you change how the preprocessor inserts boundaries, **`IsBoundary`** and any assumptions in **`ExtractWord`** must stay consistent.

## Other behavior in this plugin

- **`FormatNameLabel`** — Speaker lines arrive as `Chinese\npinyin`; this collapses pinyin spaces and merges into a single visible line (`Chinese Pinyin`).
- **`FormatTextLabel`** — Some emote strings get `{{EFD27B}}…{{-}}` injected at runtime; this detects a bad combined pattern and splits into bracketed colored lines (see comments in `Plugin.cs`).
- **`ShouldDisplayPopupForWordUnderMouse`** — Skips popups for tokens that look like Latin letters or digits (English in the Chinese UI).
- **`FormatDictionaryDefinition`** — Wraps the headword in the game’s color markup and appends pinyin plus gloss lines from **CEDICT**, loaded in **`Init`** from `cedict_ts.u8` next to the plugins.

## Build

Build **`ShadowrunReturnsLanguageEngage.sln`** from `shadowrun-returns/`; output is **`bin\Debug\SRLEPlugin.dll`** under this project. Deploy it beside the main mod DLL in `BepInEx/plugins/`.

## Further reading

- **[`../chinese_preprocessor/README.md`](../chinese_preprocessor/README.md)** — full preprocessor behavior (pinyin, cleanup passes, `.mo` output).
- **`Plugin.cs`** — authoritative implementation for boundaries and formatting.
