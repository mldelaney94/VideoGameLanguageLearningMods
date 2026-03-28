using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
  /* These are the quads associated with the conversations main text */
  public static BetterList<Vector3> speakerQuads = new ();
  /* This is the actual NPC text */
  public static string speakerText = "";
  /* This manages the mismatch between speakerQuads and speakerText.
   * Speaker text can look like so: {{EFD27B}}你好\u200B呀{{-}}, but
   * but speakerQuads will only have 12 vertices, mapping to the
   * characters "你好呀", because they're actually displayed on
   * screen. So to get the character under our cursor,
   * we must create a mapping - what character are the quads at indices
   * 0, 1, 2 and 3 for? {{EFD27B}} is 10 characters, so, index 10, 你 (0-indexed)
   */
  public static List<int> speakerQuadToIndexMap = []; 
}