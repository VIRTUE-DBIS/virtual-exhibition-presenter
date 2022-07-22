using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
  [Serializable]
  public class PolygonRoomModel : IModel
  {
    public Vector3 position;
    private List<WallModel> _walls;
    public Material floorMaterial;
    public Material ceilingMaterial;

    public PolygonRoomModel(Vector3 position, List<WallModel> walls, Material floorMaterial, Material ceilingMaterial)
    {
      this.position = position;
      _walls = walls;
      this.floorMaterial = floorMaterial;
      this.ceilingMaterial = ceilingMaterial;
    }

    public WallModel[] GetWalls()
    {
      return _walls.ToArray();
    }

    public WallModel GetWallAt(int index)
    {
      return _walls[index];
    }

    public void Add(WallModel model)
    {
      _walls.Add(model);
    }
  }
}