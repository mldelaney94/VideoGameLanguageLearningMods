using ShadowrunReturnsLanguageEngage.Contract;
using System.Collections.Generic;

namespace ShadowrunReturnsLanguageEngage
{
  public static class Globals
  {
    public static Dictionary<UILabel, LabelDataObject> LabelRegistry = [];
    public static UILabel currentRenderingLabel = null;
    public static Dictionary<string, Dictionary<string, string>> CEDict = [];
    public static SRLEPlugin plugin;
  }
}