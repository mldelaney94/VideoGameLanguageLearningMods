using System.Collections.Generic;
using UnityEngine;

namespace ShadowrunReturnsLanguageEngage
{
  public class LabelDataObject
  {
    public BetterList<Vector3> corners;
    public string text;
    public Transform transform;
    public List<int> textIndices;
    public BetterList<Vector3> textQuads;
    public BetterList<Color> colors;

    public LabelDataObject(UILabel label, BetterList<Color> colors, BetterList<Vector3> textQuads, string text, List<int> textIndices)
    {
      transform = label.transform;
      this.text = text;
      this.colors = colors;
      this.textQuads = textQuads;
      this.textIndices = textIndices;

      corners = CalculateCorners(label);
    }

    private BetterList<Vector3> CalculateCorners(UILabel label)
    {
      // pivot offset ranges between 0 and 1
      var right = label.relativeSize.x * (label.pivotOffset.x + 1);
      var bottom = label.relativeSize.y * (label.pivotOffset.y - 1);

      var bounds = new BetterList<Vector3>();
      bounds.Add(Vector3.zero);
      bounds.Add(new Vector3(right, bottom, 0));
      bounds.Add(Vector3.zero);
      bounds.Add(Vector3.zero);

      return bounds;
    }
  }
}
