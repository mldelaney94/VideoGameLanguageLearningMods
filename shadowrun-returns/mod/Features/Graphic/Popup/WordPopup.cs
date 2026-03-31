using UnityEngine;

namespace ShadowrunReturnsLanguageEngage
{
  public static class WordPopup
  {
    private static GameObject panel;
    private static UILabel label;

    public static void Show(string text, UIPanel parentPanel, Vector3 worldPos)
    {
      if (panel == null) Create(parentPanel);

      label.text = text;

      var root = NGUITools.FindInParents<UIRoot>(parentPanel.gameObject);
      panel.transform.localPosition = root.transform.InverseTransformPoint(worldPos);
      panel.SetActive(true);
    }

    public static void Hide()
    {
      if (panel != null)
        panel.SetActive(false);
    }

    private static void Create(UIPanel parentPanel)
    {
      var root = NGUITools.FindInParents<UIRoot>(parentPanel.gameObject);
      panel = NGUITools.AddChild<UIPanel>(root.gameObject).gameObject;
      label = NGUITools.AddWidget<UILabel>(panel.gameObject);

      foreach (var key in Globals.LabelRegistry.Keys)
      {
        if (key.transform != null)
        {
          label.font = key.font;
          break;
        }
      }

      panel.name = "SLRETextPopup";
    }
  }
}
