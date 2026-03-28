using HarmonyLib;
using UnityEngine;
using static UICamera;

namespace ShadowrunReturnsLanguageEngage
{
  [HarmonyPatch(typeof(UICamera), "ProcessMouse")]
  internal static class UICameraProcessMousePatch
  {
    private static string lastWord = "";

    // MouseOrTouch[] 0 is first button, 1 is right button, 2 is middle button
    private static void Postfix(MouseOrTouch[] ___mMouse, RaycastHit ___lastHit)
    {
      if (___mMouse.Length == 0 || ___mMouse[0].current == null) return;
      if (___mMouse[0].current.name != "ConversationDragContents") return;

      // the mouse collides with ConversationDragPanel, which does not contain a TextLabel
      // however, as visually it contains the text, it must be stacked somewhere underneath the drag panels parent
      var parent = ___mMouse[0].current.transform.parent;
      // two children, 0: NameLabel and 1: TextLabel
      var textLabel = parent.GetComponentsInChildren<UILabel>()[1];

      var textLabelPoint = textLabel.transform.InverseTransformPoint(___lastHit.point);

      var quadIndex = FindQuad(textLabelPoint);
      if (quadIndex < 0) return;

      string word = ExtractWord(quadIndex);
      if (word.Length > 0 && word != lastWord)
      {
        lastWord = word;
        ShadowrunreturnsLanguageEngage.Log.LogInfo(lastWord);
      }
    }

    private static int FindQuad(Vector3 localPoint)
    {
      for (int i = 0; i < Globals.speakerQuads.size; i += 4)
      {
        // in speakerQuads
        // [0] = topright
        // [1] = topbottom
        // [2] = bottomleft
        // [3] = topleft
        // so you only need two corners to know if we're inside the quad
        var topRight = Globals.speakerQuads[i];
        var top = topRight.y;
        var right = topRight.x;
        var bottomLeft = Globals.speakerQuads[i + 2];
        var bottom = bottomLeft.y;
        var left = bottomLeft.x;

        if (localPoint.y <= top && localPoint.y >= bottom
          && localPoint.x <= right && localPoint.x >= left)
        {
          return i;
        }
      }

      return -1;
    }


    private static string ExtractWord(int vertexBaseIndex)
    {
      int quadNumber = vertexBaseIndex / 4;
      if (quadNumber >= Globals.speakerQuadToIndexMap.Count) return "";

      int strIdx = Globals.speakerQuadToIndexMap[quadNumber];
      var text = Globals.speakerText;

      if (IsBoundary(text[strIdx])) return "";

      int left = strIdx;
      while (left > 0 && !IsBoundary(text[left - 1]))
        left--;

      int right = strIdx;
      while (right < text.Length - 1 && !IsBoundary(text[right + 1]))
        right++;

      return text.Substring(left, right - left + 1);
    }

    private static bool IsBoundary(char c)
    {
      return c == '\u200B' || c == ' ' || c == '\n'
        || c == '[' || c == ']' || c == '{' || c == '}' || c == '\\'
        || c == '？' || c == '，' || c == '！' || c == '。' || c == '；'
        || c == '"' || c == '：' || c == '–' || c == '—'
        || c == '＊' || c == '…' || c == '、' || c == '～' || c == '－'
        || c == '（' || c == '）' || c == '─' || c == '＜' || c == '＞'
        || c == '．' || c == '《' || c == '》' || c == '％' || c == '·'
        || c == '\'' || c == '【' || c == '】';
    }
  }
}
