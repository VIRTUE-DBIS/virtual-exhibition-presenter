using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
  [System.Serializable]
  public class WallModel : IModel
  {
    public Vector3 Start;
    public Vector3 End;
    public float Height;
    public Material Material;

    public WallModel(Vector3 start, Vector3 end, float height, Material material = null)
    {
      Start = start;
      End = end;
      Height = height;
      Material = material;
    }

    public WallModel(Vector3 position, float width, float height, Material material = null)
    {
      Start = position;
      End = position + Vector3.right * width;
      Height = height;
      Material = material;
    }

    public WallModel(float width, float height, Material material = null)
    {
      Start = Vector3.zero;
      End = Vector3.right * width;
      Height = height;
      Material = material;
    }
  }
}