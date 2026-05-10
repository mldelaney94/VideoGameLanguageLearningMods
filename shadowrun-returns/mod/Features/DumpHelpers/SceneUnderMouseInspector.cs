using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using static UICamera;

namespace ShadowrunReturnsLanguageEngage
{
  /// <summary>
  /// Optional logging for whatever NGUI reports under the cursor (and related camera / layer context).
  /// Enable from plugin Awake or at runtime; keeps <see cref="UICameraProcessMousePatch"/> free of probe code.
  /// AI.
  /// </summary>
  internal static class SceneUnderMouseInspector
  {
    /// <summary>When true, logs whenever NGUI’s hover target changes.</summary>
    public static bool Enabled = false;

    /// <summary>Extra lines: UICamera mask fields (reflection), extra Physics.Raycast comparison.</summary>
    public static bool Verbose;

    private static GameObject lastLoggedTarget;

    internal static void ResetHoverTarget()
    {
      lastLoggedTarget = null;
    }

    internal static void MaybeLogOnTargetChange(MouseOrTouch[] mMouse, RaycastHit lastHit)
    {
      if (!Enabled || mMouse == null || mMouse.Length == 0 || mMouse[0].current == null)
        return;

      var cur = mMouse[0].current;
      if (cur == lastLoggedTarget)
        return;
      lastLoggedTarget = cur;

      var log = ShadowrunreturnsLanguageEngage.Log;
      log.LogInfo("====================");
      log.LogInfo(
        $"[SceneUnderMouse] NGUI current={cur.name} layer={cur.layer} ({LayerMask.LayerToName(cur.layer)}) path={HierarchyPath(cur.transform)}");

      if (lastHit.collider != null)
      {
        var cgo = lastHit.collider.gameObject;
        log.LogInfo(
          $"[SceneUnderMouse] Physics lastHit.collider={lastHit.collider.name} go={cgo.name} layer={cgo.layer} ({LayerMask.LayerToName(cgo.layer)})");
      }
      else
        log.LogInfo("[SceneUnderMouse] Physics lastHit.collider=null");

      Camera uiCam = UICamera.currentCamera;
      log.LogInfo(
        $"[SceneUnderMouse] UICamera.currentCamera={(uiCam != null ? uiCam.name : "null")} " +
        $"cullingMask={(uiCam != null ? FormatLayerMaskWithLayers(uiCam.cullingMask) : "n/a")}");

      Camera layerCam = NGUITools.FindCameraForLayer(cur.layer);
      log.LogInfo(
        $"[SceneUnderMouse] NGUITools.FindCameraForLayer({cur.layer})={(layerCam != null ? layerCam.name : "null")} " +
        $"sameRefAsCurrentUICam={ReferenceEquals(uiCam, layerCam)}");

      if (!Verbose)
        return;

      var refGo = GameObject.Find("ConversationDragContents");
      if (refGo != null)
      {
        int refLayer = refGo.layer;
        log.LogInfo(
          $"[SceneUnderMouse] ConversationDragContents layer={refLayer} ({LayerMask.LayerToName(refLayer)}) " +
          $"FindCameraForLayer={NGUITools.FindCameraForLayer(refLayer)?.name ?? "null"}");
      }

      LogFirstUICameraMaskFields();

      if (uiCam != null)
      {
        var ray = uiCam.ScreenPointToRay(Input.mousePosition);
        int mask = uiCam.cullingMask;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Max(uiCam.farClipPlane, 5000f), mask))
        {
          var hl = hit.collider.gameObject.layer;
          log.LogInfo(
            $"[SceneUnderMouse] Physics.Raycast(UICamera cam, cullingMask) hit={hit.collider.name} " +
            $"layer={hl} ({LayerMask.LayerToName(hl)})");
        }
        else
          log.LogInfo("[SceneUnderMouse] Physics.Raycast(UICamera cam, cullingMask): no hit");

        const int allLayers = -1;
        if (mask != allLayers && Physics.Raycast(ray, out hit, Mathf.Max(uiCam.farClipPlane, 5000f), allLayers))
        {
          log.LogInfo(
            $"[SceneUnderMouse] Physics.Raycast(..., mask=-1) hit={hit.collider?.name} (cullingMask != all layers)");
        }
      }
    }

    /// <summary>Hex, decimal, and which Unity layers have bits set (index + LayerToName per bit).</summary>
    private static string FormatLayerMaskWithLayers(int mask)
    {
      return $"0x{mask:X8} ({mask}) — {DescribeLayerMaskBits(mask)}";
    }

    private static string DescribeLayerMaskBits(int mask)
    {
      if (mask == -1)
        return "all layers";
      if (mask == 0)
        return "no layers";

      var sb = new StringBuilder();
      for (int i = 0; i < 32; i++)
      {
        if ((mask & (1 << i)) == 0)
          continue;
        if (sb.Length > 0)
          sb.Append(", ");
        string nm = LayerMask.LayerToName(i);
        if (string.IsNullOrEmpty(nm))
          nm = "?";
        sb.Append(i);
        sb.Append(" (");
        sb.Append(nm);
        sb.Append(')');
      }
      return sb.Length == 0 ? "no layers" : "layers: " + sb;
    }

    private static string FormatMaskOrIntField(object v)
    {
      if (v == null) return "null";
      if (v is LayerMask lm) return FormatLayerMaskWithLayers(lm.value);
      if (v is int i) return FormatLayerMaskWithLayers(i);
      return v.ToString();
    }

    private static string HierarchyPath(Transform t)
    {
      var sb = new StringBuilder(128);
      while (t != null)
      {
        if (sb.Length > 0) sb.Insert(0, '/');
        sb.Insert(0, t.name);
        t = t.parent;
      }
      return sb.ToString();
    }

    private static void LogFirstUICameraMaskFields()
    {
      var log = ShadowrunreturnsLanguageEngage.Log;
      var ui = Object.FindObjectOfType(typeof(UICamera)) as Component;
      if (ui == null)
      {
        log.LogInfo("[SceneUnderMouse] FindObjectOfType(UICamera): null");
        return;
      }

      log.LogInfo($"[SceneUnderMouse] first UICamera on GO={ui.gameObject.name}");
      const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      foreach (var f in ui.GetType().GetFields(flags))
      {
        if (f.Name.IndexOf("Mask") < 0 && f.Name.IndexOf("mask") < 0 && f.Name.IndexOf("layer") < 0)
          continue;
        if (f.FieldType != typeof(LayerMask) && f.FieldType != typeof(int))
          continue;
        try
        {
          log.LogInfo(
            $"[SceneUnderMouse] UICamera.{f.Name} ({f.FieldType.Name}) = {FormatMaskOrIntField(f.GetValue(ui))}");
        }
        catch { }
      }
    }
  }

  /// <summary>Hooks NGUI input only to drive <see cref="SceneUnderMouseInspector"/>; word-selection logic stays elsewhere.</summary>
  [HarmonyPatch(typeof(UICamera), "ProcessMouse")]
  internal static class UICameraSceneInspectorPatch
  {
    private static void Postfix(MouseOrTouch[] ___mMouse, RaycastHit ___lastHit)
    {
      SceneUnderMouseInspector.MaybeLogOnTargetChange(___mMouse, ___lastHit);
    }
  }
}
