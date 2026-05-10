#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified throws a lot of false positives here
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static UICamera;

namespace ShadowrunReturnsLanguageEngage
{
  [HarmonyPatch(typeof(UICamera), "ProcessMouse")]
  internal static class UICameraProcessMousePatch
  {
    private static string lastWord = "";
    // the textlabels themselves don't have colliders, instead we collide with something thats on top
    // of them, and then walk the stack of neighbours to find them
    private static readonly HashSet<string> collisionNamesToCheck = 
      [
        "ConversationDragContents",
        "ConversationResponse(Clone)",
        "ObjectivesDragPanelContents"
      ];

    // MouseOrTouch[] 0 is where what's underneath the mouse is
    private static void Postfix(MouseOrTouch[] ___mMouse, RaycastHit ___lastHit)
    {
      if (___mMouse.Length == 0 || ___mMouse[0].current == null) return;
      if (!collisionNamesToCheck.Contains(___mMouse[0].current.name))
      {
        lastWord = string.Empty;
        //WordPopup.Hide();
        return;
      }

      if (___mMouse[0].current.name == "ConversationDragContents")
      {
        ComponentDumper.Dump(___mMouse[0].current.transform.parent.gameObject);
      }

      var textLabel = FindTextLabel(___lastHit.point, out Vector3 textLabelPoint);

      if (textLabel == null)
      {
        return;
      }

      var quadIndex = PointIsInBoxes(textLabelPoint, textLabel.textQuads);
      if (quadIndex < 0)
      {
        return;
      }

      string word = Globals.plugin.ExtractWord(quadIndex, textLabel.text, textLabel.textIndices);
      if (word.Length > 0 && word != lastWord)
      {
        lastWord = word;
        ShadowrunreturnsLanguageEngage.Log.LogInfo($"{lastWord}");

        var panel = NGUITools.FindInParents<UIPanel>(textLabel.transform.gameObject);
        if (panel != null)
          WordPopup.Show(word, panel, ___lastHit.point);
      }
    }

    private static LabelDataObject FindTextLabel(Vector3 lastHit, out Vector3 textLabelPoint)
    {
      foreach (var label in Globals.LabelRegistry.Values)
      {
        if (label.transform == null) continue;
        var localPoint = label.transform.InverseTransformPoint(lastHit);
        int isInLabel = PointIsInBoxes(localPoint, label.corners);
        if (isInLabel >= 0)
        {
          textLabelPoint = localPoint;
          return label;
        }
      }

      textLabelPoint = Vector3.zero;
      return null;
    }

    private static int PointIsInBoxes(Vector3 localPoint, BetterList<Vector3> boxes)
    {
      if (boxes.size % 4 != 0)
      {
        ShadowrunreturnsLanguageEngage.Log.LogWarning($"Box collection size modulo 4 should be 0. Is ({boxes.size} % 4 == {boxes.size % 4})");
      }
      for (int i = 0; i < boxes.size; i += 4)
      {
        // [0] = topright
        // [1] = bottomright
        // [2] = bottomleft
        // [3] = topleft
        // top left corner is always (0, 0), with y decreasing (downwards) and x increasing (rightwards),
        // so you only need bottom right coords to know if you're inside the box
        var bottomRight = boxes[i+1];
        var right = bottomRight.x;
        var bottom = bottomRight.y;

        if (localPoint.y >= bottom && localPoint.x <= right
          && localPoint.y <= 0 && localPoint.x >= 0)
        {
          return i;
        }
      }

      return -1;
    }
  }
}
