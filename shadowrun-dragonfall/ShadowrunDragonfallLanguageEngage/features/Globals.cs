using ShadowrunDragonfallLanguageEngage.Contract;
using System.Collections.Generic;

namespace ShadowrunDragonfallLanguageEngage
{
  public static class Globals
  {
    public static Dictionary<UILabel, LabelDataObject> LabelRegistry = [];
    public static UILabel currentRenderingLabel = null;
    public static Dictionary<string, Dictionary<string, string>> CEDict = [];
    public static SRDFPlugin plugin;
  }
}