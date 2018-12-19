using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.VREM.Model {
  [Serializable]
  public class Wall {
    
    public Vector3 color;
    public string direction;

    public Exhibit[] exhibits;

    public string texture; // NONE -> debug: colors
    
  }
}