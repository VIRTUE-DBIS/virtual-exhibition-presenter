using System;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;

namespace Unibas.DBIS.VREP.World {
  
  /// <summary>
  /// A representation of a wall, attachable to a gameobject.
  /// </summary>
  public class Wall : MonoBehaviour {

    /// <summary>
    /// The wall's data
    /// </summary>
    public DefaultNamespace.VREM.Model.Wall WallData;
    
    /// <summary>
    /// The model of the wall.
    /// </summary>
    public WallModel WallModel;


  }
}