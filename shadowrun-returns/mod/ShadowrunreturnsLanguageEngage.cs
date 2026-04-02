using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;

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

      harmony.PatchAll();
      UIFontPrintPatchTests.RunAll();
      CEDictParserTests.RunAll();
    }
  }
}
