using ShadowrunReturnsLanguageEngage.Contract;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ShadowrunReturnsLanguageEngage.Chinese
{
  public class Plugin : SRLEPlugin
  {
    public static Dictionary<string, Dictionary<string, string>> CEDict = [];
    public const string WordHighlightColor = "EFD27B"; // yellow

    /// <summary>
    /// When you mouse over a word, you are actually mousing over a letter,
    /// and we need to determine where the word begins and ends. This algorithm
    /// searches left and right to find the whole word.
    /// </summary>
    /// <param name="vertexBaseIndex"></param>
    /// <param name="text"></param>
    /// <param name="textIndices"></param>
    /// <returns></returns>
    public override string ExtractWord(int vertexBaseIndex, string text, List<int> textIndices)
    {
      int quadNumber = vertexBaseIndex / 4;
      if (quadNumber >= textIndices.Count) return "";

      int strIdx = textIndices[quadNumber];

      if (IsBoundary(text[strIdx])) return "";

      int left = strIdx;
      while (left > 0 && !IsBoundary(text[left - 1]))
        left--;

      int right = strIdx;
      while (right < text.Length - 1 && !IsBoundary(text[right + 1]))
        right++;

      return text.Substring(left, right - left + 1);
    }

    /// <summary>
    /// Defines how the ExtractWord algorithm will determine when it has
    /// finished searching left and right for the beginning and end of a word
    /// under the mouse
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public override bool IsBoundary(char c)
    {
      return c == '\u200B' || c == ' ' || c == '\n'
        || c == '[' || c == ']' || c == '{' || c == '}' || c == '\\'
        || c == '？' || c == '，' || c == '！' || c == '。' || c == '；'
        || c == '"' || c == '：' || c == '–' || c == '—'
        || c == '＊' || c == '…' || c == '、' || c == '～' || c == '－'
        || c == '（' || c == '）' || c == '─' || c == '＜' || c == '＞'
        || c == '．' || c == '《' || c == '》' || c == '％' || c == '·'
        || c == '\'' || c == '【' || c == '】';
    }

    // NameLabel text arrives as "Chinese\npin yin" — collapse pinyin spaces
    // and capitalize, then place it inline: "Chinese Pinyin"
    public override string FormatNameLabel(string text)
    {
      var parts = text.Split('\n');

      if (parts.Length < 2) return text;

      var chinese = parts[0];
      var pinyin = parts[1];
      pinyin = char.ToUpper(pinyin[0]) + pinyin.Substring(1);
      pinyin = pinyin.Replace(" ", "");
      return chinese + " " + pinyin;
    }

    // Emote lines sometimes erroneously looks like so:
    // {{EFD27B}}chinese\n\npinyin{{-}}.
    // This cannot be fixed in preprocessing because the game sometimes injects
    // the colours at runtime.
    // Split them into individually colored bracket-delimited lines
    public override string FormatTextLabel(string text)
    {
      bool isIncorrectlyFormattedEmote =
        text.StartsWith("{{EFD27B}}")
        && text.EndsWith("{{-}}")
        && Regex.Matches(text, Regex.Escape("{{EFD27B}}")).Count == 1;

      if (isIncorrectlyFormattedEmote)
        return string.Join("]{{-}}\n\n{{EFD27B}}[", text.Split('\n'));

      return text;
    }

    /**
     * This runs before popup logic runs,
     * where `text` is the text under the mouse.
     * If false, the popup will not show
     */
    public override bool ShouldDisplayPopupForWordUnderMouse(string text)
    {
      if (Regex.IsMatch(text, "[0-9a-zA-Z]+")) return false;

      return true;
    }

    /// <summary>
    /// Formats the word for display in the popup
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public override string FormatDictionaryDefinition(string word)
    {
      return "{{" + WordHighlightColor + "}}" + word + "{{-}}" + "\n\n"
        + CEDict[word]["pinyin"] + "\n----------" + "\n\n"
        + string.Join("\n\n", CEDict[word]["english"].Split('/'));
    }

    public override void Init(string pluginPath)
    {
      CEDict = CEDictParser.ParseCEDict(Path.Combine(pluginPath, "cedict_ts.u8"));

      CEDictParserTests.RunAll();
    }
  }
}
