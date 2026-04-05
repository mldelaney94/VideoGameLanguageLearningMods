using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ShadowrunReturnsLanguageEngage
{
  public static class WordPopup
  {
    private static UIPanel parentPanel;
    private static UILabel label;
    private static UIAtlas pdaAtlas;

    private const int PanelWidth = 300;
    private const int PanelHeight = 400;
    private const int TextLineWidth = 280;
    private const int BorderThickness = 1;
    private const int PopupVerticalOffset = -175;
    private const int PopupRightOffset = 100;
    private const int PopupLeftOffset = -250;
    private const string BackgroundColor = "060606"; // grey-black
    private const string BorderColor = "62b6bd"; // light-blue
    private const string WordHighlightColor = "EFD27B"; // yellow
    private const string ScrollBarColour = "1DD0DE";

    public static void Show(string text, UIPanel convoPanel, Vector3 worldPos)
    {
      if (Regex.IsMatch(text, "[0-9a-zA-Z]+")) return;

      EnsureCreated(convoPanel);
      label.text = FormatDictionaryDefinition(text);
      Position(convoPanel, worldPos);
      parentPanel.gameObject.SetActive(true);

      ComponentDumper.Dump(parentPanel.gameObject);
    }

    public static void Hide()
    {
      parentPanel.gameObject?.SetActive(false);
    }

    private static void EnsureCreated(UIPanel convoPanel)
    {
      var root = NGUITools.FindInParents<UIRoot>(convoPanel.gameObject);
      if (parentPanel == null) Create(root);
    }

    private static void Position(UIPanel convoPanel, Vector3 worldPos)
    {
      var isRight = worldPos.x > 0;
      var xOffset = isRight ? PopupRightOffset : PopupLeftOffset;
      parentPanel.transform.localPosition = new Vector3(
        convoPanel.transform.localPosition.x + xOffset,
        PopupVerticalOffset,
        0);
    }

    private static void Create(UIRoot root)
    {
      parentPanel = NGUITools.AddChild<UIPanel>(root.gameObject);
      parentPanel.name = "SLRETextPopup";
      pdaAtlas = GetAtlas();

      AddBackground(parentPanel.gameObject);
      var scrollBar = AddScrollBar(parentPanel.gameObject);
      AddTextPanel(parentPanel.gameObject, scrollBar);
    }

    private static UIAtlas GetAtlas()
    {
      if (pdaAtlas != null) return pdaAtlas;

      UIAtlas[] atlasses = (UIAtlas[]) Resources.FindObjectsOfTypeAll(typeof(UIAtlas));
      foreach (var atlas in atlasses)
      {
        if (atlas.name == "PDA")
        {
          return atlas;
        }
      }

      return null;
    }

    private static void AddBackground(GameObject parent)
    {
      var panel = NGUITools.AddChild<UIPanel>(parent);
      panel.name = "SLRETextPopupBackground";

      var bg = NGUITools.AddWidget<UITexture>(panel.gameObject);
      bg.color = NGUITools.ParseColor(BackgroundColor, 0);
      bg.transform.localScale = new Vector3(PanelWidth, PanelHeight, 1f);
      bg.material = CreateFlatMaterial(renderQueue: 1);

      var border = NGUITools.AddWidget<UITexture>(panel.gameObject);
      border.color = NGUITools.ParseColor(BorderColor, 0);
      border.transform.localScale = new Vector3(
        PanelWidth + BorderThickness,
        PanelHeight + BorderThickness,
        1f);
      border.material = CreateFlatMaterial(renderQueue: bg.material.renderQueue - 1);
    }

    private static UIScrollBar AddScrollBar(GameObject parent)
    {
      var scrollBar = NGUITools.AddChild<UIScrollBar>(parent);
      scrollBar.name = "SLRETextPopupScrollBar";
      scrollBar.direction = UIScrollBar.Direction.Vertical;

      var trackSprite = NGUITools.AddWidget<UISlicedSprite>(scrollBar.gameObject);
      trackSprite.atlas = pdaAtlas;
      trackSprite.spriteName = "scrollBarFrame";
      trackSprite.color = NGUITools.ParseColor(ScrollBarColour, 0);

      var thumbSprite = NGUITools.AddWidget<UISlicedSprite>(scrollBar.gameObject);
      thumbSprite.atlas = pdaAtlas;
      thumbSprite.spriteName = "scrollBar";
      thumbSprite.color = NGUITools.ParseColor(ScrollBarColour, 0);

      scrollBar.foreground = thumbSprite;
      scrollBar.foreground.gameObject.AddComponent<BoxCollider>();
      var fgEventListener = scrollBar.foreground.gameObject.AddComponent<UIEventListener>();
      var onPressForeground = typeof(UIScrollBar).GetMethod("OnPressForeground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      fgEventListener.onPress = (UIEventListener.BoolDelegate) Delegate.CreateDelegate(typeof(UIEventListener.BoolDelegate), scrollBar, onPressForeground);
      var onDragForeground = typeof(UIScrollBar).GetMethod("OnDragForeground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      fgEventListener.onDrag = (UIEventListener.VectorDelegate) Delegate.CreateDelegate(typeof(UIEventListener.VectorDelegate), scrollBar, onDragForeground);
      scrollBar.foreground.name = "Foreground";
      scrollBar.foreground.depth = 2;
      scrollBar.background = trackSprite;
      scrollBar.background.gameObject.AddComponent<BoxCollider>();
      var bgEventListener = scrollBar.background.gameObject.AddComponent<UIEventListener>();
      var onPressBackground = typeof(UIScrollBar).GetMethod("OnPressBackground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      bgEventListener.onPress = (UIEventListener.BoolDelegate) Delegate.CreateDelegate(typeof(UIEventListener.BoolDelegate), scrollBar, onPressBackground);
      var onDragBackground = typeof(UIScrollBar).GetMethod("OnDragBackground", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      bgEventListener.onDrag = (UIEventListener.VectorDelegate) Delegate.CreateDelegate(typeof(UIEventListener.VectorDelegate), scrollBar, onDragBackground);
      scrollBar.background.name = "Background";
      scrollBar.background.depth = 1;

      return scrollBar;
    }

    private static UIPanel AddTextPanel(GameObject parent, UIScrollBar scrollBar)
    {
      var panel = NGUITools.AddChild<UIPanel>(parent);
      panel.name = "SLRETextPopupTextPanel";
      panel.clipping = UIDrawCall.Clipping.HardClip;
      panel.clipRange = new Vector4(0, 0, PanelWidth, PanelHeight);

      var dragPanel = panel.gameObject.AddComponent<UIDraggablePanel>();
      dragPanel.transform.localScale = new Vector3(PanelWidth, PanelHeight, 1f);
      dragPanel.verticalScrollBar = scrollBar;
      dragPanel.dragEffect = UIDraggablePanel.DragEffect.Momentum;
      dragPanel.scrollWheelFactor = 1.5f;
      dragPanel.disableDragIfFits = true;
      dragPanel.scale = Vector3.one;
      var onVerticalBar = typeof(UIDraggablePanel).GetMethod("OnVerticalBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      scrollBar.onChange = (UIScrollBar.OnScrollBarChange) Delegate.CreateDelegate(typeof(UIScrollBar.OnScrollBarChange), dragPanel, onVerticalBar);

      // we need this label so we can set the text on it dynamically later
      // avoiding having to recreate these static instances
      label = NGUITools.AddWidget<UILabel>(panel.gameObject);
      label.font = FindFont();
      label.lineWidth = TextLineWidth;
      label.pivot = UIWidget.Pivot.TopLeft;
      label.transform.localPosition = new Vector3(-PanelWidth / 2f, PanelHeight / 2f, 0);

      var dragPanelContents = NGUITools.AddChild<UIDragPanelContents>(parent);
      dragPanelContents.draggablePanel = dragPanel;
      dragPanelContents.gameObject.AddComponent<BoxCollider>();

      return panel;
    }

    private static Material CreateFlatMaterial(int renderQueue)
    {
      return new Material(Shader.Find("Unlit/Transparent Colored"))
      {
        renderQueue = renderQueue
      };
    }

    private static UIFont FindFont()
    {
      foreach (var key in Globals.LabelRegistry.Keys)
      {
        if (key.transform != null) return key.font;
      }
      return null;
    }

    // This belongs here because it formats the string for display
    // specifically for this popup
    private static string FormatDictionaryDefinition(string word)
    {
      return "{{" + WordHighlightColor + "}}" + word + "{{-}}" + '\n'
        + Globals.CEDict[word]["pinyin"] + '\n'
        + Globals.CEDict[word]["english"];
    }
  }
}
