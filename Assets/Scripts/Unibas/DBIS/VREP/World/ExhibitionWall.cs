using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP;
using UnityEngine;

namespace World {
  
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

    public List<Displayal> Displayals = new List<Displayal>();

    public void RestoreDisplayals() {
      Displayals.ForEach(d => d.RestorePosition());
    }

    public void AttachExhibits()
    {
      // TODO Make displayal configurable
      var prefab = ObjectFactory.GetDisplayalPrefab();
      foreach (var e in WallData.exhibits)
      {
        GameObject displayal = Instantiate(prefab);
        displayal.name = "Displayal (" + e.name + ")";
        displayal.transform.parent = Anchor.transform;
        var pos = new Vector3(e.position.x, e.position.y, -ExhibitionBuildingSettings.Instance.WallOffset);
        displayal.transform.localPosition = pos;
        //displayal.transform.rotation = Quaternion.Euler(ObjectFactory.CalculateRotation(WallData.direction));
        var rot =  Quaternion.Euler(90,0,180);
        displayal.transform.localRotation = rot;// Because prefab is messed up
        
        
        if(!VREPController.Instance.Settings.SpotsEnabled || !e.light){	
          displayal.transform.Find("Directional light").gameObject.SetActive(false);
        }
		
        Displayal disp = displayal.gameObject.GetComponent<Displayal>();
        disp.SetExhibitModel(e);
        disp.OriginalPosition = pos;
        disp.OriginalRotation = rot;
        Displayals.Add(disp);
		
        ImageLoader image = displayal.transform.Find("Plane").gameObject.AddComponent<ImageLoader>(); // Displayal
        //ImageLoader image = displayal.AddComponent<ImageLoader>();// ImageDisplayPlane
        image.ReloadImage(e.GetURLEncodedPath());
        displayal.transform.localScale = ScalingUtility.convertMeters2PlaneScaleSize(e.size.x, e.size.y);

        if (e.audio != null)
        {
          Debug.Log("added audio to display object");
          var closenessDetector = displayal.AddComponent<ClosenessDetector>();
          closenessDetector.url = e.audio;
        }
      }
    }

    public WallOrientation GetOrientation()
    {
      return WallData.GetOrientation();
    }

  }
}