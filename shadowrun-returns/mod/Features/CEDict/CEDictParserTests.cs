using System.IO;

namespace ShadowrunReturnsLanguageEngage
{
  internal static class CEDictParserTests
  {
    internal static void RunAll()
    {
      TestParseLine();
    }

    private static void TestParseLine()
    {
      var path = Path.GetTempFileName();
      try
      {
        File.WriteAllText(path,
          "動產 动产 [dong4 chan3] /movable property/personal property/\n");

        var dict = CEDictParser.ParseCEDict(path);

        Assert("has key 动产", dict.ContainsKey("动产"));

        var entry = dict["动产"];
        Assert("pinyin is 'dong4chan3'",
          entry["pinyin"] == "dong4chan3");
        Assert("english is movable property/personal property/",
          entry["english"] == "movable property/personal property/");

        ShadowrunreturnsLanguageEngage.Log.LogInfo(
          "[CEDictParserTests] ParseLine PASSED");
      }
      finally
      {
        File.Delete(path);
      }
    }

    private static void Assert(string label, bool condition)
    {
      if (!condition)
        ShadowrunreturnsLanguageEngage.Log.LogError($"[FAIL] CEDict: {label}");
    }
  }
}
