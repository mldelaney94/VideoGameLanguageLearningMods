using System.Collections.Generic;
using System.IO;

namespace ShadowrunReturnsLanguageEngage
{
  internal static class CEDictParser
  {
    public static Dictionary<string, Dictionary<string, string>> ParseCEDict(string filePath)
    {
      var result = new Dictionary<string, Dictionary<string, string>>();

      foreach (var line in File.ReadAllLines(filePath))
      {

        if (string.IsNullOrEmpty(line) || line.Substring(0, 1) == "#")
          continue;

        var simplified = GetSimplified(line);
        var pinyin = GetPinyin(line);
        var english = GetEnglish(line);

        result[simplified] = new Dictionary<string, string>
        {
          { "english", english },
          { "pinyin", pinyin }
        };
      }

      return result;
    }

    //動產 动产 [dong4 chan3] /movable property/personal property/
    //above is a typical line: traditional simplified [pinyin] /english/
    private static string GetSimplified(string line)
    {
      var parts = line.Split([' '], 3);

      return parts[1];
    }
    private static string GetPinyin(string line)
    {
      var start = line.IndexOf('[') + 1; // skip [
      var end = line.IndexOf(']');

      return line.Substring(start, end - start).Replace(" ", "");
    }
    private static string GetEnglish(string line)
    {
      var parts = line.Split(["] /"], System.StringSplitOptions.None);

      return parts[1];
    }
  }
}
