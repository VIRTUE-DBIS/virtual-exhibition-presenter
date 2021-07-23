using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
  [System.Serializable]
  public class WallModel : IModel
  {
    public Vector3 start;
    public Vector3 end;
    public float height;
    public Material material;

    public WallModel(Vector3 start, Vector3 end, float height, Material material = null)
    {
      this.start = start;
      this.end = end;
      this.height = height;
      this.material = material;
    }

    public WallModel(Vector3 position, float width, float height, Material material = null)
    {
      start = position;
      end = position + Vector3.right * width;
      this.height = height;
      this.material = material;
    }

    public WallModel(float width, float height, Material material = null)
    {
      start = Vector3.zero;
      end = Vector3.right * width;
      this.height = height;
      this.material = material;
    }
  }
}