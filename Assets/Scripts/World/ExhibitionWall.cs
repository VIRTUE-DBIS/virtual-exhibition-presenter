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
    public DefaultNamespace.VREM.Model.Wall WallData { get; set; }

    /// <summary>
    /// The model of the wall.
    /// </summary>
    public WallModel WallModel { get; set; }

    /// <summary>
    /// The Anchor for adding exhibits.
    /// </summary>
    public GameObject Anchor{ get; set; }

    public void AttachExhibits()
    {
      // TODO Make displayal configurable
      var prefab = ObjectFactory.GetDisplayalPrefab();
      foreach (var e in WallData.exhibits)
      {
        GameObject disp = Instantiate(prefab);
        disp.name = "Displayal (" + e.name + ")";
        disp.transform.position = new Vector3(e.position.x, e.position.y, -ExhibitionBuildingSettings.Instance.WallOffset);
        
      }
    }

  }
}