using HarmonyLib;
using System;
using System.Collections.Generic;

namespace ShadowrunReturnsLanguageEngage
{
  [HarmonyPatch(typeof(UILabel), nameof(UILabel.OnFill))]
  internal static class UILabelOnFillPatch
  {
    // tried to add the level loading text, but it has no collider
    private static readonly Dictionary<string, Func<string, string>> Actions = new()
    {
      { "NameLabel", Globals.plugin.FormatNameLabel },
      { "TextLabel", Globals.plugin.FormatTextLabel },
      { "ChapterSummaryLabel", Noop },
    };

    private static void Prefix(UILabel __instance)
    {
      var name = __instance.gameObject.name;

      if (Actions.ContainsKey(name))
      {
        __instance.text = Actions[name].Invoke(__instance.text);
        Globals.currentRenderingLabel = __instance;
      }
    }

    private static void Postfix()
    {
      Globals.currentRenderingLabel = null;
    }

    private static string Noop(string text)
    {
      return text;
    }
  }
}
