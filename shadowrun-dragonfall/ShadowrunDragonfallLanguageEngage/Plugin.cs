using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using ShadowrunDragonfallLanguageEngage.Contract;
using System;
using System.IO;
using System.Reflection;

namespace ShadowrunDragonfallLanguageEngage
{
  [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
  public class ShadowrunDragonfallLanguageEngage : BaseUnityPlugin
  {
    internal static ManualLogSource Log { get; private set; }
    private readonly Harmony harmony = new("matthewdelaney.ShadowRunDragonfallLanguageEngage");

    private void Awake()
    {
      Log = Logger;

      LoadPlugin();

      harmony.PatchAll();
    }

    private void LoadPlugin()
    {
      var srdfPluginDllPath = Path.Combine(Paths.PluginPath, "SRDFPlugin.dll");
      string pluginType = "unlabelled";
      try
      {
        var plugin = Assembly.LoadFrom(srdfPluginDllPath);
        foreach(var item in plugin.GetTypes())
        {
          if (item.ToString().Contains("Plugin"))
          {
            pluginType = item.ToString();
          }
        }

        Globals.plugin = (SRDFPlugin)plugin.CreateInstance(pluginType);
        Globals.plugin.Init(Paths.PluginPath);
      }
      catch (FileNotFoundException ex)
      {
        Log.LogError(
          "Optional SRDF plugin: SRDFPlugin.dll or one of its dependencies was not found by the CLR. " +
          "Expected plugin file at: " + srdfPluginDllPath + ". Copy SRDFPlugin.dll into BepInEx/plugins next to this mod. Main mod continues without linked-plugin features.");
        Log.LogError(ex);
      }
      catch (DllNotFoundException ex)
      {
        Log.LogError("Optional SRDF plugin: a native or managed dependency of SRDFPlugin.dll failed to load. Check plugin README for required DLLs beside the game.");
        Log.LogError(ex);
      }
      catch (FileLoadException ex)
      {
        Log.LogError("Optional SRDF plugin: the CLR refused to load SRDFPlugin.dll (policy, I/O after open, or identity). See inner details.");
        Log.LogError(ex);
      }
      catch (BadImageFormatException ex)
      {
        Log.LogError("Optional SRDF plugin: SRDFPlugin.dll is not loadable by this runtime (wrong CPU architecture, corrupt file, or not a .NET assembly).");
        Log.LogError(ex);
      }
      catch (TypeLoadException ex)
      {
        Log.LogError("Optional SRDF plugin: type resolution failed (missing type, wrong .NET version, or loader constraint). Often a contract/game version mismatch.");
        Log.LogError(ex);
      }
      catch (MissingMethodException ex)
      {
        Log.LogError("Optional SRDF plugin: expected member missing on Plugin or contract types — rebuild the plugin against the same contract version as this mod.");
        Log.LogError(ex);
      }
      catch (InvalidCastException ex)
      {
        Log.LogError("Optional SRDF plugin: class Plugin exists but does not inherit SRDFPlugin (wrong plugin assembly or duplicate type name).");
        Log.LogError(ex);
      }
      catch (NullReferenceException ex)
      {
        Log.LogError("Optional SRDF plugin: no public parameterless Plugin type was activated, or Plugin returned/used a null reference during startup.");
        Log.LogError(ex);
      }
      catch (Exception ex)
      {
        Log.LogError(
          "Optional SRDF plugin failed during load or FormatTextLabel (unexpected error). Main mod continues; linked-plugin features are unavailable.");
        Log.LogError(ex);
      }

      // pluginType ~= ShadowrunDragonfallLanguageEngage.{language}.Plugin
      Log.LogInfo($"SRDF {pluginType.Split('.')[1]} plugin loaded :)");
    }
  }
}
