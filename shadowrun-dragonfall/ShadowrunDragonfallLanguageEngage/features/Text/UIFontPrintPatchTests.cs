using System.Collections.Generic;

namespace ShadowrunDragonfallLanguageEngage
{
  internal static class UIFontPrintPatchTests
  {
    internal static void RunAll()
    {
      TestBuildIndexMap_ChineseDialogue();
      TestBuildIndexMap_PinyinDialogue();
      TestBuildIndexMap_Simple();
      TestBuildIndexMap_PlainChinese();
      TestBuildIndexMap_PlainChineseMultiline();
      TestBuildIndexMap_NoEncoding();
    }

    private static void TestBuildIndexMap_Simple()
    {
      // 你好 = one word, 呀 = second word, ZWS between them
      string text = "{{EFD27B}}你好\u200B呀{{-}}";
      var map = UIFontPrintPatch.BuildIndexMap(text, encoding: true);
      Assert("Simple: count is 3", map.Count == 3);
      Assert("Simple: 你 at 10", map[0] == 10);
      Assert("Simple: 好 at 11", map[1] == 11);
      Assert("Simple: 呀 at 13", map[2] == 13);
      LogMap("Simple", text, map);
    }

    private static void TestBuildIndexMap_ChineseDialogue()
    {
      string text =
        "{{EFD27B}}[站\u200B在\u200B死者\u200B旁边\u200B的\u200B是\u200B一个\u200B矮人\u200B，\u200B哼\u200B着\u200B小曲\u200B。\u200B他露\u200B着\u200B牙\n"
        + "齿\u200B微笑\u200B着\u200B好像\u200B在\u200B说\u200B\u201C\u200B我\u200B非常\u200B喜爱\u200B我\u200B的\u200B工作\u200B\u201D\u200B，\u200B但\u200B这\u200B不是\u200B\n"
        + "一个\u200B工作\u200B在\u200B停尸房\u200B或\u200B器官\u200B商店\u200B的\u200B人\u200B应该\u200B有\u200B的\u200B状态\u200B。\u200B\n"
        + "当\u200B你\u200B走近\u200B，\u200B他\u200B慢慢\u200B抬起\u200B头\u200B，\u200B他\u200B的\u200B嘴角\u200B撇向\u200B一边\u200B笑\u200B着\u200B\n"
        + "。\u200B他\u200B的\u200B眼神\u200B好像\u200B在\u200B说\u200B什么\u200B......\u200B尽管\u200B你\u200B从\u200B他\u200B眼中\u200B只能\u200B看\n"
        + "到\u200B那些\u200B手术\u200B工具\u200B的\u200B反射\u200B。]{{-}}";
      var map = UIFontPrintPatch.BuildIndexMap(text, encoding: true);
      AssertNoZws(text, map);
      AssertNoSkippedChars(text, map);
    }

    private static void TestBuildIndexMap_PinyinDialogue()
    {
      string text =
        "{{EFD27B}}[zhan4 zai4 si3zhe3 pang2bian1 de5 shi4 yi1ge5\n"
        + "ai3ren2，heng1 zhao2 xiao3qu3。ta1 lou4 zhao2\n"
        + "ya2chi3 wei1xiao4 zhao2 hao3xiang4 zai4 shuo1\u201Cwo3\n"
        + "fei1chang2 xi3ai4 wo3 de5 gong1zuo4\u201D，dan4 zhei4\n"
        + "bu4shi4 yi1ge5 gong1zuo4 zai4 ting2shi1fang2 huo4\n"
        + "qi4guan1 shang1dian4 de5 ren2 ying1gai1 you3 de5\n"
        + "zhuang4tai4。dang1 ni3 zou3jin4，ta1 man4man4\n"
        + "tai2qi3 tou2，ta1 de5 zui3jiao3 pie1 xiang4 yi1bian1\n"
        + "xiao4 zhao2。ta1 de5 yan3shen2 hao3xiang4 zai4\n"
        + "shuo1 shen2me5......jin3guan3 ni3 cong2 ta1\n"
        + "yan3zhong1 zhi3neng2 kan4dao4 na4xie1 shou3shu4\n"
        + "gong1ju4 de5 fan3she4。]{{-}}";
      var map = UIFontPrintPatch.BuildIndexMap(text, encoding: true);
      AssertNoSkippedChars(text, map);
    }

    private static void TestBuildIndexMap_PlainChinese()
    {
      string text = "你好\u200B世界\u200B，\u200B欢迎\u200B来到\u200B暗影\u200B狂奔\u200B。";
      var map = UIFontPrintPatch.BuildIndexMap(text, encoding: true);
      Assert("PlainChinese: count is 14", map.Count == 14);
      AssertNoZws(text, map);
      AssertNoSkippedChars(text, map);
      LogMap("PlainChinese", text, map);
    }

    private static void TestBuildIndexMap_PlainChineseMultiline()
    {
      string text =
        "站\u200B在\u200B死者\u200B旁边\u200B的\u200B是\u200B一个\u200B矮人\u200B，\u200B哼\u200B着\u200B小曲\u200B。\n"
        + "他露\u200B着\u200B牙齿\u200B微笑\u200B着\u200B好像\u200B在\u200B说\u200B什么\u200B。";
      var map = UIFontPrintPatch.BuildIndexMap(text, encoding: true);
      AssertNoZws(text, map);
      AssertNoSkippedChars(text, map);
      LogMap("PlainChineseMultiline", text, map);
    }

    private static void TestBuildIndexMap_NoEncoding()
    {
      // With encoding=false, {{EFD27B}} should NOT be skipped — each
      // brace/letter is a potential glyph
      string text = "{{EFD27B}}你好{{-}}";
      var map = UIFontPrintPatch.BuildIndexMap(text, encoding: false);
      Assert("NoEncoding: braces are mapped", map.Count > 3);
      LogMap("NoEncoding", text, map);
    }

    private static void AssertNoZws(string text, List<int> map)
    {
      for (int i = 0; i < map.Count; i++)
      {
        char c = text[map[i]];
        Assert($"No ZWS in map (quad {i}, str[{map[i]}])", c != '\u200B');
      }
    }

    private static void AssertNoSkippedChars(string text, List<int> map)
    {
      for (int i = 0; i < map.Count; i++)
      {
        char c = text[map[i]];
        Assert($"No space in map (quad {i})", c != ' ');
        Assert($"No newline in map (quad {i})", c != '\n');
      }
    }

    private static void LogMap(string label, string text, List<int> map)
    {
      ShadowrunDragonfallLanguageEngage.Log.LogInfo(
        $"[Test:{label}] {map.Count} quads from {text.Length} chars"
      );
      for (int i = 0; i < map.Count; i++)
      {
        char c = text[map[i]];
        string display = c < ' ' || c == '\u200B'
          ? $"U+{((int)c):X4}"
          : c.ToString();
        ShadowrunDragonfallLanguageEngage.Log.LogInfo(
          $"  quad {i} → str[{map[i]}] = '{display}'"
        );
      }
    }

    private static void Assert(string label, bool condition)
    {
      if (!condition)
        ShadowrunDragonfallLanguageEngage.Log.LogError($"[FAIL] {label}");
    }
  }
}
