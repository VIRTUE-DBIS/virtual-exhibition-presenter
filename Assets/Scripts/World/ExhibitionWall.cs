using System;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;

namespace Unibas.DBIS.VREP.World {
  
  /// <summary>
  /// A representation of a wall, attachable to a gameobject.
  /// </summary>
  public class ExhibitionWall : MonoBehaviour {

    /// <summary>
    /// The wall's data
    /// </summary>
    public DefaultNamespace.VREM.Model.Wall WallData;
    
    /// <summary>
    /// The model of the wall.
    /// </summary>
    public WallModel WallModel;

    /// <summary>
    /// The Anchor for adding exhibits.
    /// </summary>
    public GameObject Anchor;

    public void AttachExhibits()
    {
      foreach (var e in WallData.exhibits)
      {
        
      }
    }

  }
}