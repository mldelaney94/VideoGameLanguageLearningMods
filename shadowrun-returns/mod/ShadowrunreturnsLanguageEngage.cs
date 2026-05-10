using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using ShadowrunReturnsLanguageEngage.Contract;
using System;
using System.IO;
using System.Reflection;

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
      // debugging - refresh component dumper dump when you open the game
      File.Delete(ComponentDumper.OutputPath);

      UIFontPrintPatchTests.RunAll();

      LoadPlugin();

      harmony.PatchAll();
    }

    private void LoadPlugin()
    {
      var srlePluginDllPath = Path.Combine(Paths.PluginPath, "SRLEPlugin.dll");
      string pluginType = "unlabelled";
      try
      {
        var plugin = Assembly.LoadFrom(srlePluginDllPath);
        foreach(var item in plugin.GetTypes())
        {
          if (item.ToString().Contains("Plugin"))
          {
            pluginType = item.ToString();
          }
        }

        Globals.plugin = (SRLEPlugin)plugin.CreateInstance(pluginType);
        Globals.plugin.Init(Paths.PluginPath);
      }
      catch (FileNotFoundException ex)
      {
        Log.LogError(
          "Optional SRLE plugin: SRLEPlugin.dll or one of its dependencies was not found by the CLR. " +
          "Expected plugin file at: " + srlePluginDllPath + ". Copy SRLEPlugin.dll into BepInEx/plugins next to this mod. Main mod continues without linked-plugin features.");
        Log.LogError(ex);
      }
      catch (DllNotFoundException ex)
      {
        Log.LogError("Optional SRLE plugin: a native or managed dependency of SRLEPlugin.dll failed to load. Check plugin README for required DLLs beside the game.");
        Log.LogError(ex);
      }
      catch (FileLoadException ex)
      {
        Log.LogError("Optional SRLE plugin: the CLR refused to load SRLEPlugin.dll (policy, I/O after open, or identity). See inner details.");
        Log.LogError(ex);
      }
      catch (BadImageFormatException ex)
      {
        Log.LogError("Optional SRLE plugin: SRLEPlugin.dll is not loadable by this runtime (wrong CPU architecture, corrupt file, or not a .NET assembly).");
        Log.LogError(ex);
      }
      catch (TypeLoadException ex)
      {
        Log.LogError("Optional SRLE plugin: type resolution failed (missing type, wrong .NET version, or loader constraint). Often a contract/game version mismatch.");
        Log.LogError(ex);
      }
      catch (MissingMethodException ex)
      {
        Log.LogError("Optional SRLE plugin: expected member missing on Plugin or contract types — rebuild the plugin against the same contract version as this mod.");
        Log.LogError(ex);
      }
      catch (InvalidCastException ex)
      {
        Log.LogError("Optional SRLE plugin: class Plugin exists but does not inherit SRLEPlugin (wrong plugin assembly or duplicate type name).");
        Log.LogError(ex);
      }
      catch (NullReferenceException ex)
      {
        Log.LogError("Optional SRLE plugin: no public parameterless Plugin type was activated, or Plugin returned/used a null reference during startup.");
        Log.LogError(ex);
      }
      catch (Exception ex)
      {
        Log.LogError(
          "Optional SRLE plugin failed during load or FormatTextLabel (unexpected error). Main mod continues; linked-plugin features are unavailable.");
        Log.LogError(ex);
      }

      // pluginType ~= ShadowrunReturnsLanguageEngage.{language}.Plugin
      Log.LogInfo($"SLRE {pluginType.Split('.')[1]} plugin loaded :)");
    }
  }
}
