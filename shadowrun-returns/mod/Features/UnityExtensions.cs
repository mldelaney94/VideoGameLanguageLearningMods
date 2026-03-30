using UnityEngine;

namespace ShadowrunReturnsLanguageEngage
{
  public static class UnityExtensions
  {
    //gameObjects contain the items that make up a component, like a boxcollider, a UIPanel, a UILabel etc
    //but transforms hold the parent because its fundamentally a spatial relationship, child is positioned
    //relative to Y
    //calling GetComponent on a transform is the equivalent of calling it on the gameObject of the same
    //component
    public static T GetComponentInParent<T>(this Component component) where T : Component
    {
      var current = component.transform;
      while(current != null)
      {
        var c = current.GetComponent<T>();
        if (c != null) return c;
        current = current.parent;
      }
      return null;
    }
  }
}
