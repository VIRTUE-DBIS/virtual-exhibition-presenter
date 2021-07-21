using System;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.VREM.Model
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.exhibition.Wall
  /// </summary>
  [Serializable]
  public class Wall
  {
    public Vector3 color;
    public string direction;

    public Exhibit[] exhibits;

    public string texture;

    public WallOrientation GetOrientation()
    {
      return (WallOrientation) Enum.Parse(typeof(WallOrientation), direction, true);
    }
  }
}