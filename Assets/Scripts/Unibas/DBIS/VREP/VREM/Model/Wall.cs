using System;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.VREM.Model
{
  [Serializable]
  public class Wall
  {
    public Vector3 color;
    public string direction;

    public Exhibit[] exhibits;

    public string texture; // NONE -> debug: colors

    public WallOrientation GetOrientation()
    {
      return (WallOrientation) Enum.Parse(typeof(WallOrientation), direction, true);
    }
  }
}