using System;
using System.Text.RegularExpressions;
using UnityEngine;

// This class essentially just tries to replicate 'ConversationDragContents'
// from the game, which is the primary text component in the conversation UI.
// That's why its a bit of a slog, because
// ultimately, we're trying to code a UI component that would have been
// created in a visual editor.

// We want to create it from scratch, rather than say, deep cloning
// 'ConversationDragContents', because it needs to appear from
// multiple different places in the game, and that component won't
// always be available.

// It was made by using component dumper on both this component,
// and 'ConversationDragContents', and then laboriously changing
// things until the dump output matched.
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
      if (parentPanel == null)
      {
        Create(root);
      }
    }

    private static void Position(UIPanel convoPanel, Vector3 worldPos)
    {
      var isRight = worldPos.x > 0;
      var xOffset = isRight ? PopupRightOffset : PopupLeftOffset;
      parentPanel.transform.localPosition = new Vector3(
        convoPanel.transform.localPosition.x + xOffset,
        PopupVerticalOffset,
        convoPanel.transform.localPosition.z);
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
      scrollBar.transform.localPosition = new Vector3(PanelWidth / 2f + 10f, PanelHeight / 2f, 0);

      var scalar = NGUITools.AddChild(scrollBar.gameObject);
      scalar.name = "Scalar";
      scalar.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

      var trackSprite = NGUITools.AddWidget<UISlicedSprite>(scalar);
      trackSprite.atlas = pdaAtlas;
      trackSprite.spriteName = "scrollBarFrame";
      trackSprite.color = NGUITools.ParseColor(ScrollBarColour, 0);
      trackSprite.transform.localScale = new Vector3(28f, PanelHeight * 2f, 1f);
      trackSprite.name = "Background";
      trackSprite.depth = 1;
      var bgCollider = trackSprite.gameObject.AddComponent<BoxCollider>();
      bgCollider.center = new Vector3(0, -0.5f, 0);
      var bgEventListener = trackSprite.gameObject.AddComponent<UIEventListener>();

      var transformNode = NGUITools.AddChild(scalar);
      transformNode.name = "Transform";
      transformNode.transform.localPosition = new Vector3(1f, 0, 0);

      var thumbSprite = NGUITools.AddWidget<UISlicedSprite>(transformNode);
      thumbSprite.atlas = pdaAtlas;
      thumbSprite.spriteName = "scrollBar";
      thumbSprite.color = NGUITools.ParseColor(ScrollBarColour, 0);
      thumbSprite.transform.localScale = new Vector3(32f, PanelHeight * 2f, 1f);
      thumbSprite.name = "Foreground";
      thumbSprite.depth = 2;
      var fgCollider = thumbSprite.gameObject.AddComponent<BoxCollider>();
      fgCollider.center = new Vector3(0, -0.5f, 0);
      var fgEventListener = thumbSprite.gameObject.AddComponent<UIEventListener>();

      scrollBar.background = trackSprite;
      scrollBar.foreground = thumbSprite;

      var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
      fgEventListener.onPress = (UIEventListener.BoolDelegate) Delegate.CreateDelegate(typeof(UIEventListener.BoolDelegate), scrollBar, typeof(UIScrollBar).GetMethod("OnPressForeground", flags));
      fgEventListener.onDrag = (UIEventListener.VectorDelegate) Delegate.CreateDelegate(typeof(UIEventListener.VectorDelegate), scrollBar, typeof(UIScrollBar).GetMethod("OnDragForeground", flags));
      bgEventListener.onPress = (UIEventListener.BoolDelegate) Delegate.CreateDelegate(typeof(UIEventListener.BoolDelegate), scrollBar, typeof(UIScrollBar).GetMethod("OnPressBackground", flags));
      bgEventListener.onDrag = (UIEventListener.VectorDelegate) Delegate.CreateDelegate(typeof(UIEventListener.VectorDelegate), scrollBar, typeof(UIScrollBar).GetMethod("OnDragBackground", flags));

      return scrollBar;
    }

    private static UIPanel AddTextPanel(GameObject parent, UIScrollBar scrollBar)
    {
      var panel = NGUITools.AddChild<UIPanel>(parent);
      panel.name = "SLRETextPopupTextPanel";
      panel.clipping = UIDrawCall.Clipping.HardClip;
      panel.clipRange = new Vector4(0, 0, PanelWidth, PanelHeight);

      var dragPanel = panel.gameObject.AddComponent<UIDraggablePanel>();
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
      var contentCollider = dragPanelContents.gameObject.AddComponent<BoxCollider>();
      contentCollider.size = Vector3.one;

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
