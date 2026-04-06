using System.Reflection;
using UnityEngine;

namespace ShadowrunReturnsLanguageEngage
{
  /// <summary>
  /// Reference dumps from ComponentDumper (Conversation* vs SLRETextPopup) live in comments below.
  /// <see cref="MaybeLogPopupRaycastDiagnostics"/> helps compare camera and layer assumptions at runtime.
  /// </summary>
  public static class DragPanelFactory
  {
  }
}

//=== SLRETextPopup @ 10:34:59 ===
//[SLRETextPopup] pos=(334.0, -175.0, -5.0) scale=(1.0, 1.0, 1.0)
//  <UIPanel> clipping=None clipRange=(0.0, 0.0, 0.0, 0.0) showInPanelTool=True generateNormals=False depthPass=False widgetsAreStatic=False cachedTransform="SLRETextPopup"(Transform) changedLastFrame=True debugInfo=Gizmos clipping=None clipRange=(0.0, 0.0, 0.0, 0.0) clipSoftness=(40.0, 40.0) widgets=System.Collections.Generic.List`1[UIWidget] drawCalls=System.Collections.Generic.List`1[UIDrawCall]
//  [SLRETextPopupTextPanel] pos=(0.0, 0.0, 0.0) scale=(1.0, 1.0, 1.0)
//    <UIPanel> clipping=HardClip clipRange=(0.0, 0.0, 300.0, 400.0) showInPanelTool=True generateNormals=False depthPass=False widgetsAreStatic=False cachedTransform="SLRETextPopupTextPanel"(Transform) changedLastFrame=True debugInfo=Gizmos clipping=HardClip clipRange=(0.0, 0.0, 300.0, 400.0) clipSoftness=(40.0, 40.0) widgets=System.Collections.Generic.List`1[UIWidget] drawCalls=System.Collections.Generic.List`1[UIDrawCall]
//    <UIDraggablePanel> restrictWithinPanel=True disableDragIfFits=True dragEffect=Momentum scale=(1.0, 1.0, 1.0) scrollWheelFactor=1.5 momentumAmount=35 relativePositionOnReset=(0.0, 0.0) repositionClipping=False horizontalScrollBar=null verticalScrollBar="SLRETextPopupScrollBar"(UIScrollBar) showScrollBars=OnlyIfNeeded bounds=Center: (-10.0, -250.0, 0.0), Extents: (140.0, 450.0, 0.0) shouldMoveHorizontally=False shouldMoveVertically=True currentMomentum=(0.0, 0.0, 0.0)
//    [Label] pos=(-150.0, 200.0, 0.0) scale=(100.0, 100.0, 1.0)
//      <UILabel> text="{{EFD27B}}屏幕{{-}}..." font=MediumNormal fontSize=36 lineWidth=280 depth=0 pivot=TopLeft color=RGBA(1.000, 1.000, 1.000, 1.000)
//  [DragPanelContents] pos=(0.0, 0.0, 0.0) scale=(300.0, 400.0, 1.0)
//    <UIDragPanelContents> draggablePanel="SLRETextPopupTextPanel"(UIDraggablePanel)
//    <BoxCollider> center=(0.0, 0.0, 0.0) size=(1.0, 1.0, 1.0)

//[ConversationAnchor(Clone)] pos=(-456.0, 0.0, -30.0) scale=(1.0, 1.0, 1.0)
//  [ConversationDragPanel] pos = (234.0, 10.0, -5.0) scale = (1.0, 1.0, 1.0)
//    < UIPanel > clipping = SoftClip clipRange = (1.0, 181.0, 394.0, 228.0) showInPanelTool = True generateNormals = False depthPass = False widgetsAreStatic = False cachedTransform = "ConversationDragPanel"(Transform) changedLastFrame = False debugInfo = Gizmos clipping = SoftClip clipRange = (1.0, 181.0, 394.0, 228.0) clipSoftness = (1.0, 5.0) widgets = System.Collections.Generic.List`1[UIWidget] drawCalls = System.Collections.Generic.List`1[UIDrawCall]
//    < UIDraggablePanel > restrictWithinPanel = True disableDragIfFits = True dragEffect = Momentum scale = (0.0, 1.0, 0.0) scrollWheelFactor = 1.5 momentumAmount = 35 relativePositionOnReset = (0.0, -311.0) repositionClipping = False horizontalScrollBar = null verticalScrollBar = "ConversationDragScrollBar"(UIScrollBar) showScrollBars = OnlyIfNeeded bounds = Center: (0.0, 155.0, 1.0), Extents: (195.0, 135.0, 0.0) shouldMoveHorizontally = False shouldMoveVertically = True currentMomentum = (0.0, 0.0, 0.0)
//    [TextLabel] pos = (-195.0, 290.0, 1.0) scale = (18.0, 18.0, 1.0)
//      < UILabel > text = "{{EFD27B}}[​屏幕​弹出来​..." font = MediumNormal fontSize = 36 lineWidth = 390 depth = 0 pivot = TopLeft color = RGBA(0.800, 0.675, 0.294, 1.000)
//  [ConversationDragContents] pos = (234.0, 194.0, -4.0) scale = (394.0, 228.0, 1.0)
//    < UIDragPanelContents > draggablePanel = "ConversationDragPanel"(UIDraggablePanel)
//    < BoxCollider > center = (0.0, 0.0, 0.0) size = (1.0, 1.0, 1.0)
