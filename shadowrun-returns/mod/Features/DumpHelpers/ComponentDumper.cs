using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ShadowrunReturnsLanguageEngage
{
  /// <summary>
  /// AI slop to dump the component hierarchy of a game object
  /// </summary>
  internal static class ComponentDumper
  {
    private static readonly HashSet<string> dumped = new HashSet<string>();
    public const string OutputPath = "./BepInEx/plugins/component-dump.txt";
    private const bool enabled = false;

    internal static void Dump(GameObject go, int maxDepth = 8)
    {
      if (!enabled) return;
      if (dumped.Contains(go.name)) return;
      dumped.Add(go.name);

      var sb = new StringBuilder();
      sb.AppendLine($"=== {go.name} @ {DateTime.Now:HH:mm:ss} ===");
      DumpRecursive(go, maxDepth, sb);
      sb.AppendLine();

      File.AppendAllText(OutputPath, sb.ToString());
      ShadowrunreturnsLanguageEngage.Log.LogInfo($"{sb.ToString()}");
    }

    internal static void Reset() => dumped.Clear();

    private static void DumpRecursive(GameObject go, int maxDepth, StringBuilder sb, int currentDepth = 0)
    {
      string indent = new string(' ', currentDepth * 2);
      var t = go.transform;
      sb.AppendLine($"{indent}[{go.name}] pos={t.localPosition} scale={t.localScale}");

      foreach (var comp in go.GetComponents<Component>())
      {
        if (comp is Transform) continue;

        string typeName = comp.GetType().Name;
        sb.Append($"{indent}  <{typeName}>");
        AppendComponentDetails(comp, sb, indent);
        sb.AppendLine();
      }

      if (currentDepth < maxDepth)
      {
        var children = new List<Transform>();
        for (int i = 0; i < t.childCount; i++)
          children.Add(t.GetChild(i));
        children.Sort((a, b) => a.localPosition.z.CompareTo(b.localPosition.z));
        foreach (var child in children)
          DumpRecursive(child.gameObject, maxDepth, sb, currentDepth + 1);
      }
    }

    private static void AppendComponentDetails(Component comp, StringBuilder sb, string indent)
    {
      if (comp is UILabel lbl)
      {
        sb.Append($" text=\"{Truncate(lbl.text, 60)}\" font={lbl.font?.name} fontSize={lbl.font?.size}");
        sb.Append($" lineWidth={lbl.lineWidth} depth={lbl.depth} pivot={lbl.pivot} color={lbl.color}");
        return;
      }
      if (comp is UISprite spr)
      {
        sb.Append($" pdaAtlas={spr.atlas?.name} spriteName=\"{spr.spriteName}\" depth={spr.depth} color={spr.color}");
        AppendAtlasSprites(spr.atlas, sb, indent);
        return;
      }
      if (comp is UIPanel panel)
      {
        sb.Append($" clipping={panel.clipping} clipRange={panel.clipRange}");
        AppendReflectedFields(comp, sb);
        return;
      }
      if (comp is UIWidget widget)
      {
        sb.Append($" depth={widget.depth} pivot={widget.pivot} color={widget.color}");
        AppendReflectedFields(comp, sb);
        return;
      }
      if (comp is BoxCollider box)
      {
        sb.Append($" center={box.center} size={box.size}");
        return;
      }

      AppendReflectedFields(comp, sb);
    }

    private static void AppendAtlasSprites(UIAtlas atlas, StringBuilder sb, string indent)
    {
      if (atlas == null) return;
      try
      {
        var sprites = atlas.GetListOfSprites();
        if (sprites == null || sprites.Count == 0) return;

        sb.AppendLine();
        sb.Append($"{indent}    sprites({sprites.Count})=[");
        int max = Math.Min(sprites.Count, 30);
        for (int i = 0; i < max; i++)
        {
          if (i > 0) sb.Append(", ");
          sb.Append(sprites[i]);
        }
        if (sprites.Count > max) sb.Append($", ...+{sprites.Count - max} more");
        sb.Append("]");
      }
      catch { }
    }

    private static void AppendReflectedFields(Component comp, StringBuilder sb)
    {
      var type = comp.GetType();
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      foreach (var field in fields)
      {
        try
        {
          sb.Append($" {field.Name}={FormatValue(field.GetValue(comp))}");
        }
        catch
        {
          sb.Append($" {field.Name}=<error>");
        }
      }

      var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      foreach (var prop in props)
      {
        if (!prop.CanRead || prop.GetIndexParameters().Length > 0) continue;
        try
        {
          sb.Append($" {prop.Name}={FormatValue(prop.GetValue(comp, null))}");
        }
        catch
        {
          sb.Append($" {prop.Name}=<error>");
        }
      }
    }

    private static string FormatValue(object value)
    {
      if (value == null) return "null";

      if (value is Delegate del)
      {
        var list = del.GetInvocationList();
        return list.Length > 0 ? $"<delegate({list.Length}):{del.Method.DeclaringType?.Name}.{del.Method.Name}>" : "null";
      }

      if (value is UnityEngine.Object uobj)
        return uobj != null ? $"\"{uobj.name}\"({uobj.GetType().Name})" : "null(destroyed)";

      if (value is string s)
        return $"\"{Truncate(s, 40)}\"";

      return value.ToString();
    }

    internal static void DumpMethods(Type type, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
    {
      var sb = new StringBuilder();
      sb.AppendLine($"=== Methods on {type.FullName} ({flags}) ===");
      foreach (var method in type.GetMethods(flags))
      {
        var parms = method.GetParameters();
        var parmStr = new string[parms.Length];
        for (int i = 0; i < parms.Length; i++)
          parmStr[i] = $"{parms[i].ParameterType.Name} {parms[i].Name}";
        sb.AppendLine($"  {method.ReturnType.Name} {method.Name}({string.Join(", ", parmStr)})");
      }
      ShadowrunreturnsLanguageEngage.Log.LogInfo(sb.ToString());
    }

    private static string Truncate(string s, int max)
    {
      if (string.IsNullOrEmpty(s)) return "";
      return s.Length <= max ? s : s.Substring(0, max) + "...";
    }
  }
}
