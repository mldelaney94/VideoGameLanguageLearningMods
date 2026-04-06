# Mod developer notes (summary)

Authoritative Cursor context for this plugin lives in **[`.cursor/rules/mod.mdc`](../.cursor/rules/mod.mdc)** (NGUI types, registry, pitfalls, tools). This file is a short recap of UI hit-testing lessons from investigation.

## NGUI UI hits are not “wrong layer” by default

- UI objects and big chrome (e.g. `ConversationAnchor/Background`) often share **layer 12 (UI)**. Logs that only show “layer 12” do not prove a misconfiguration; check **which GameObject** won the hit (`path=`, collider name).
- **`Camera.cullingMask`** (what gets drawn) and **`UICamera.eventReceiverMask`** (what NGUI uses for UI raycasts) are **different**. Example pattern: `eventReceiverMask = 0x1000` → single layer 12; `cullingMask` can include extra bits.

## What actually loses hits for custom popups

1. **Collider vs visuals** — `BoxCollider` size `(1,1,1)` with transform **scale `(1,1,1)`** is a tiny volume; match vanilla: scale the drag object to **panel width/height** (still `size` `(1,1,1)` if that matches the dump).
2. **No `UIWidget` on the collider object** — NGUI ordering may not treat the surface like other UI. Put a **nearly invisible `UITexture`** on the **same GameObject** as `BoxCollider` + `UIDragPanelContents` (see `WordPopup`).
3. **Hierarchy / Z** — a popup under **`UIRoot` only** can lose to **`Background`** under **`ConversationAnchor`** along the UI camera ray; **depth-only** fixes may be insufficient across branches. **Reparent** under the anchor and/or adjust **local Z** when needed.

## Tools

- **`SceneUnderMouseInspector`** (`Features/DumpHelpers/SceneUnderMouseInspector.cs`) — toggles in `ShadowrunreturnsLanguageEngage.Awake`. Logs hover target, physics hit, cameras, masks with **hex + per-layer names** (no manual mask decoding). Implemented as its own Harmony postfix so **`UICameraProcessMousePatch`** stays focused on word logic.
- **`ComponentDumper`** — hierarchy dumps for comparing to vanilla widgets.

## API caveat

This build’s **`UIPanel` may not expose `depth`**; prefer **`UIWidget.depth`** (and hierarchy/Z) for hit and draw order unless you confirm a `depth` member on `UIPanel` in `Assembly-CSharp.dll`.
