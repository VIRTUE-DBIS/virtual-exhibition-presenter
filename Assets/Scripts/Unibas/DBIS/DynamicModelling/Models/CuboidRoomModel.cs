using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
  [System.Serializable]
  public class CuboidRoomModel : IModel
  {
    public Vector3 position;
    public float size;
    public float height;

    public Material floorMaterial;
    public Material ceilingMaterial;
    public Material northMaterial;
    public Material eastMaterial;
    public Material southMaterial;
    public Material westMaterial;

    public CuboidRoomModel(Vector3 position, float size, float height)
    {
      this.position = position;
      this.size = size;
      this.height = height;
    }

    public CuboidRoomModel(Vector3 position, float size, float height, Material floorMaterial = null,
      Material ceilingMaterial = null, Material northMaterial = null, Material eastMaterial = null,
      Material southMaterial = null, Material westMaterial = null)
    {
      this.position = position;
      this.size = size;
      this.height = height;
      this.floorMaterial = floorMaterial;
      this.ceilingMaterial = ceilingMaterial;
      this.northMaterial = northMaterial;
      this.eastMaterial = eastMaterial;
      this.southMaterial = southMaterial;
      this.westMaterial = westMaterial;
    }
  }
}