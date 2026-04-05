using UnityEngine;

namespace ShadowrunReturnsLanguageEngage
{
  public static class DragPanelFactory
  {
    public struct DragPanelGroup
    {
      public BoxCollider dragCollider;
      public UIPanel scrollPanel;
      public UIDraggablePanel draggablePanel;
      public UILabel label;
    }

    /// <summary>
    /// Creates the sibling trio that mirrors ConversationDragContents / ConversationDragPanel / TextLabel.
    /// All children are parented under <paramref name="parent"/>.
    /// </summary>
    public static DragPanelGroup Create(
      GameObject parent,
      string namePrefix,
      UIFont font,
      int lineWidth,
      Vector4 clipRange)
    {
      var dragContents = NGUITools.AddChild(parent);
      dragContents.name = namePrefix + "DragContents";
      var dragCollider = dragContents.AddComponent<BoxCollider>();
      dragCollider.center = Vector3.zero;
      dragCollider.size = new Vector3(clipRange.z, clipRange.w, 1f);

      var dragPanelGo = NGUITools.AddChild(parent);
      dragPanelGo.name = namePrefix + "DragPanel";
      var scrollPanel = dragPanelGo.AddComponent<UIPanel>();
      scrollPanel.clipping = UIDrawCall.Clipping.SoftClip;
      scrollPanel.clipRange = clipRange;
      var draggablePanel = dragPanelGo.AddComponent<UIDraggablePanel>();
      draggablePanel.restrictWithinPanel = true;

      var dragPanelContents = dragContents.AddComponent<UIDragPanelContents>();
      dragPanelContents.draggablePanel = draggablePanel;

      var label = NGUITools.AddWidget<UILabel>(dragPanelGo);
      label.gameObject.name = "TextLabel";
      label.font = font;
      label.lineWidth = lineWidth;
      label.pivot = UIWidget.Pivot.TopLeft;
      label.transform.localPosition = new Vector3(
        -lineWidth / 2f,
        clipRange.w / 2f,
        -1f);

      return new DragPanelGroup
      {
        dragCollider = dragCollider,
        scrollPanel = scrollPanel,
        draggablePanel = draggablePanel,
        label = label
      };
    }
  }
}

//=== SLRETextPopup @ 21:52:45 ===
//[SLRETextPopup]
//  <UIPanel> clipping=None clipRange=(0.0, 0.0, 0.0, 0.0) showInPanelTool=True generateNormals=False depthPass=False widgetsAreStatic=False cachedTransform="SLRETextPopup"(Transform) changedLastFrame=True debugInfo=Gizmos clipping=None clipRange=(0.0, 0.0, 0.0, 0.0) clipSoftness=(40.0, 40.0) widgets=System.Collections.Generic.List`1[UIWidget] drawCalls=System.Collections.Generic.List`1[UIDrawCall]
//  [SLRETextPopupBackground]
//    <UIPanel> clipping=None clipRange=(0.0, 0.0, 0.0, 0.0) showInPanelTool=True generateNormals=False depthPass=False widgetsAreStatic=False cachedTransform="SLRETextPopupBackground"(Transform) changedLastFrame=True debugInfo=Gizmos clipping=None clipRange=(0.0, 0.0, 0.0, 0.0) clipSoftness=(40.0, 40.0) widgets=System.Collections.Generic.List`1[UIWidget] drawCalls=System.Collections.Generic.List`1[UIDrawCall]
//    [Texture]
//      <UITexture> depth=0 pivot=Center color=RGBA(0.024, 0.024, 0.024, 1.000) keepMaterial=True
//    [Texture]
//      <UITexture> depth=1 pivot=Center color=RGBA(0.384, 0.714, 0.741, 1.000) keepMaterial=True
//  [SLRETextPopupScrollBar]
//    <UIScrollBar> onChange=<delegate(1):UIDraggablePanel.OnVerticalBar> cachedTransform="SLRETextPopupScrollBar"(Transform) cachedCamera="Camera"(Camera) background="Background"(UISlicedSprite) foreground="Foreground"(UISlicedSprite) direction=Vertical inverted=False scrollValue=0 barSize=1 alpha=1
//    [Background]
//      <UISlicedSprite> pdaAtlas=PDA spriteName="scrollBarFrame" depth=1 color=RGBA(0.114, 0.816, 0.871, 1.000)
//        sprites(55)=[add_Button, arrow_name, bracket, cart_Bar, centerCameraButton, check_box, compass_needle, compass_overlay, cyberLink, icon_character, icon_frame, icon_gear, icon_menu, icon_objectives, iconFrameWithNotification, itemBG, moneySlot, objectiveCompleteIcon, objectiveFailedIcon, pda_button, pda_buttonBG, pda_help_button, pda_help_button_tempcover, primary_obj-icon, removeTab, right_handle, rosterBG, runnerBG, scrollArrow, scrollBar, ...+25 more]
//      <BoxCollider> center=(0.0, 0.0, 0.0) size=(1.0, 1.0, 1.0)
//      <UIEventListener> parameter=null onSubmit=null onClick=null onDoubleClick=null onHover=null onPress=<delegate(1):UIScrollBar.OnPressBackground> onSelect=null onScroll=null onDrag=<delegate(1):UIScrollBar.OnDragBackground> onDrop=null onInput=null
//    [Foreground]
//      <UISlicedSprite> pdaAtlas=PDA spriteName="scrollBar" depth=2 color=RGBA(0.114, 0.816, 0.871, 1.000)
//        sprites(55)=[add_Button, arrow_name, bracket, cart_Bar, centerCameraButton, check_box, compass_needle, compass_overlay, cyberLink, icon_character, icon_frame, icon_gear, icon_menu, icon_objectives, iconFrameWithNotification, itemBG, moneySlot, objectiveCompleteIcon, objectiveFailedIcon, pda_button, pda_buttonBG, pda_help_button, pda_help_button_tempcover, primary_obj-icon, removeTab, right_handle, rosterBG, runnerBG, scrollArrow, scrollBar, ...+25 more]
//      <BoxCollider> center=(0.0, 0.0, 0.0) size=(1.0, 1.0, 1.0)
//      <UIEventListener> parameter=null onSubmit=null onClick=null onDoubleClick=null onHover=null onPress=<delegate(1):UIScrollBar.OnPressForeground> onSelect=null onScroll=null onDrag=<delegate(1):UIScrollBar.OnDragForeground> onDrop=null onInput=null
//  [SLRETextPopupTextPanel]
//    <UIPanel> clipping=HardClip clipRange=(0.0, 0.0, 300.0, 400.0) showInPanelTool=True generateNormals=False depthPass=False widgetsAreStatic=False cachedTransform="SLRETextPopupTextPanel"(Transform) changedLastFrame=True debugInfo=Gizmos clipping=HardClip clipRange=(0.0, 0.0, 300.0, 400.0) clipSoftness=(40.0, 40.0) widgets=System.Collections.Generic.List`1[UIWidget] drawCalls=System.Collections.Generic.List`1[UIDrawCall]
//    <UIDraggablePanel> restrictWithinPanel=True disableDragIfFits=True dragEffect=Momentum scale=(1.0, 1.0, 1.0) scrollWheelFactor=1.5 momentumAmount=35 relativePositionOnReset=(0.0, 0.0) repositionClipping=False horizontalScrollBar=null verticalScrollBar="SLRETextPopupScrollBar"(UIScrollBar) showScrollBars=OnlyIfNeeded bounds=Center: (-10.0, -250.0, 0.0), Extents: (140.0, 450.0, 0.0) shouldMoveHorizontally=False shouldMoveVertically=True currentMomentum=(0.0, 0.0, 0.0)
//    [Label]
//      <UILabel> text="{{EFD27B}}屏幕{{-}}..." font=MediumNormal fontSize=36 lineWidth=280 depth=0 pivot=TopLeft color=RGBA(1.000, 1.000, 1.000, 1.000)
//  [DragPanelContents]
//    <UIDragPanelContents> draggablePanel="SLRETextPopupTextPanel"(UIDraggablePanel)
//    <BoxCollider> center=(0.0, 0.0, 0.0) size=(1.0, 1.0, 1.0)

//[ConversationDragContents]
//  <UIDragPanelContents> draggablePanel="ConversationDragPanel"(UIDraggablePanel)
//  <BoxCollider> center=(0.0, 0.0, 0.0) size=(1.0, 1.0, 1.0)
//[ConversationDragPanel]
//  <UIPanel> clipping=SoftClip clipRange=(1.0, 181.0, 394.0, 228.0) showInPanelTool=True generateNormals=False depthPass=False widgetsAreStatic=False cachedTransform="ConversationDragPanel"(Transform) changedLastFrame=False debugInfo=Gizmos clipping=SoftClip clipRange=(1.0, 181.0, 394.0, 228.0) clipSoftness=(1.0, 5.0) widgets=System.Collections.Generic.List`1[UIWidget] drawCalls=System.Collections.Generic.List`1[UIDrawCall]
//  <UIDraggablePanel> restrictWithinPanel=True disableDragIfFits=True dragEffect=Momentum scale=(0.0, 1.0, 0.0) scrollWheelFactor=1.5 momentumAmount=35 relativePositionOnReset=(0.0, -311.0) repositionClipping=False horizontalScrollBar=null verticalScrollBar="ConversationDragScrollBar"(UIScrollBar) showScrollBars=OnlyIfNeeded bounds=Center: (0.0, 155.0, 1.0), Extents: (195.0, 135.0, 0.0) shouldMoveHorizontally=False shouldMoveVertically=True currentMomentum=(0.0, 0.0, 0.0)
//  [TextLabel]
//    <UILabel> text="{{EFD27B}}[​屏幕​弹出..." font=MediumNormal fontSize=36 lineWidth=390 depth=0 pivot=TopLeft color=RGBA(0.800, 0.675, 0.294, 1.000)
//[ConversationDragScrollBar]
//  <UIScrollBar> onChange=<delegate(1):UIDraggablePanel.OnVerticalBar> cachedTransform="ConversationDragScrollBar"(Transform) cachedCamera="Camera"(Camera) background="Background"(UISlicedSprite) foreground="Foreground"(UISlicedSprite) direction=Vertical inverted=False scrollValue=0 barSize=0.8074071 alpha=1
//  [Scalar]
//    [Background]
//      <UISlicedSprite> pdaAtlas=PDA spriteName="scrollBarFrame" depth=1 color=RGBA(0.114, 0.816, 0.871, 1.000)
//        sprites(55)=[add_Button, arrow_name, bracket, cart_Bar, centerCameraButton, check_box, compass_needle, compass_overlay, cyberLink, icon_character, icon_frame, icon_gear, icon_menu, icon_objectives, iconFrameWithNotification, itemBG, moneySlot, objectiveCompleteIcon, objectiveFailedIcon, pda_button, pda_buttonBG, pda_help_button, pda_help_button_tempcover, primary_obj-icon, removeTab, right_handle, rosterBG, runnerBG, scrollArrow, scrollBar, ...+25 more]
//      <BoxCollider> center=(0.0, -0.5, 0.0) size=(1.0, 1.0, 1.0)
//      <UIEventListener> parameter=null onSubmit=null onClick=null onDoubleClick=null onHover=null onPress=<delegate(1):UIScrollBar.OnPressBackground> onSelect=null onScroll=null onDrag=<delegate(1):UIScrollBar.OnDragBackground> onDrop=null onInput=null
//    [Transform]
//      [Foreground]
//        <UISlicedSprite> pdaAtlas=PDA spriteName="scrollBar" depth=2 color=RGBA(0.114, 0.816, 0.871, 1.000)
//          sprites(55)=[add_Button, arrow_name, bracket, cart_Bar, centerCameraButton, check_box, compass_needle, compass_overlay, cyberLink, icon_character, icon_frame, icon_gear, icon_menu, icon_objectives, iconFrameWithNotification, itemBG, moneySlot, objectiveCompleteIcon, objectiveFailedIcon, pda_button, pda_buttonBG, pda_help_button, pda_help_button_tempcover, primary_obj-icon, removeTab, right_handle, rosterBG, runnerBG, scrollArrow, scrollBar, ...+25 more]
//        <BoxCollider> center=(0.0, -0.5, 0.0) size=(1.0, 1.0, 1.0)
//        <UIEventListener> parameter=null onSubmit=null onClick=null onDoubleClick=null onHover=null onPress=<delegate(1):UIScrollBar.OnPressForeground> onSelect=null onScroll=null onDrag=<delegate(1):UIScrollBar.OnDragForeground> onDrop=null onInput=null
