// All plugins must use this abstract class as a base
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShadowrunReturnsLanguageEngage.Contract
{
  public abstract class SRLEPlugin
  {
    /// <summary>
    /// When you mouse over a word, you are actually mousing over a letter,
    /// and we need to determine where the word begins and ends. This algorithm
    /// searches left and right to find the whole word.
    /// </summary>
    /// <param name="vertexBaseIndex"></param>
    /// <param name="text"></param>
    /// <param name="textIndices"></param>
    /// <returns></returns>
    public virtual string ExtractWord(int vertexBaseIndex, string text, List<int> textIndices)
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
    public virtual bool IsBoundary(char c)
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

    /// <summary>
    /// Any extra formatting for the name label
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual string FormatNameLabel(string text)
    {
      return text;
    }

    /// <summary>
    /// Any extra formatting necessary for text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual string FormatTextLabel(string text)
    {
      return text;
    }

    /**
     * This runs before popup logic runs,
     * where `text` is the text under the mouse.
     * If false, the popup will not show
     */
    public virtual bool ShouldDisplayPopupForWordUnderMouse(string text)
    {
      if (Regex.IsMatch(text, "[0-9a-zA-Z]+")) return false;

      return true;
    }

    /// <summary>
    /// Formats the word for display in the popup
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public virtual string FormatDictionaryDefinition(string word)
    {
      return word;
    }

    public virtual void Init(string pluginPath)
    {
      return;
    }
  }
}
