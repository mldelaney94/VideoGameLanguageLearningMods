using HarmonyLib;
using System.Text.RegularExpressions;

namespace ShadowrunReturnsLanguageEngage
{
  [HarmonyPatch(typeof(UILabel), nameof(UILabel.OnFill))]
  internal static class UILabelOnFillPatch
  {
    private static void Prefix(UILabel __instance)
    {
      var name = __instance.gameObject.name;

      if (name == "NameLabel")
      {
        __instance.text = FormatNameLabel(__instance.text);
      }
      else if (name == "TextLabel" && __instance.transform.parent.name == "ConversationDragPanel")
      {
        __instance.text = FormatTextLabel(__instance.text);
      }
      else
      {
        ShadowrunreturnsLanguageEngage.Log.LogWarning(
          $"[OnFill Prefix] UNEXPECTED label=\"{name}\" — not NameLabel or TextLabel"
        );
      }
    }

    // NameLabel text arrives as "Chinese\npin yin" — collapse pinyin spaces
    // and capitalize, then place it inline: "Chinese Pinyin"
    private static string FormatNameLabel(string text)
    {
      var parts = text.Split('\n');
      var chinese = parts[0];
      var pinyin = parts[1];
      pinyin = pinyin[0].ToString().ToUpper() + pinyin.Substring(1);
      pinyin = string.Join("", pinyin.Split(' '));
      return chinese + " " + pinyin;
    }

    // Emote lines erroneously look like so: {{EFD27B}}chinese\n\npinyin{{-}}.
    // This cannot be fixed in preprocessing because the game sometimes injects
    // the colours at runtime.
    // Split them into individually colored bracket-delimited lines
    private static string FormatTextLabel(string text)
    {
      bool isSingleColorEmote =
        text.StartsWith("{{EFD27B}}")
        && text.EndsWith("{{-}}")
        && Regex.Matches(text, Regex.Escape("{{EFD27B}}")).Count == 1;

      if (isSingleColorEmote)
        return string.Join("]{{-}}\n\n{{EFD27B}}[", text.Split('\n'));

      return text;
    }
  }
}
