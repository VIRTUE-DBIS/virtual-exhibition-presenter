using System;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace DefaultNamespace.VREM.Model {
  [Serializable]
  public class Wall {
    
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