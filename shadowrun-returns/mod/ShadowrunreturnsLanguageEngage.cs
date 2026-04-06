using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using System.IO;

namespace ShadowrunReturnsLanguageEngage
{
  //[Info   : Preloader] Running under Unity 4.2.0f4
  [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
  public class ShadowrunreturnsLanguageEngage : BaseUnityPlugin
  {
    internal static ManualLogSource Log { get; private set; }
    private readonly Harmony harmony = new("matthewdelaney.ShadowRunReturnsLanguageEngage");

    private void Awake()
    {
      Log = Logger;
      Globals.CEDict = CEDictParser.ParseCEDict("./BepInEx/plugins/cedict_ts.u8");
      File.Delete(ComponentDumper.OutputPath);

      SceneUnderMouseInspector.Enabled = true;
      SceneUnderMouseInspector.Verbose = true;
      // SceneUnderMouseInspector.Enabled = true; // under-cursor NGUI + camera/layer lines when hover target changes
      // SceneUnderMouseInspector.Verbose = true;  // also: ConversationDragContents compare, UICamera mask fields, extra raycasts

      harmony.PatchAll();
      UIFontPrintPatchTests.RunAll();
      CEDictParserTests.RunAll();
    }
  }
}
