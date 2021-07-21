﻿using System.Collections.Generic;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.Multimedia;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.VREM.Model;
using UnityEngine;

namespace Unibas.DBIS.VREP.World
{
  /// <summary>
  /// A representation of a wall, attachable to a game object.
  /// </summary>
  public class ExhibitionWall : MonoBehaviour
  {
    /// <summary>
    /// The wall's data.
    /// </summary>
    public Wall WallData { get; set; }

    /// <summary>
    /// The model of the wall.
    /// </summary>
    public WallModel WallModel { get; set; }

    /// <summary>
    /// The Anchor for adding exhibits.
    /// </summary>
    public GameObject Anchor { get; set; }

    public List<Displayal> displayals = new List<Displayal>();

    public void RestoreDisplayals()
    {
      displayals.ForEach(d => d.RestorePosition());
    }

    public void AttachExhibits()
    {
      var prefab = ObjectFactory.GetDisplayalPrefab();

      foreach (var e in WallData.exhibits)
      {
        var displayal = Instantiate(prefab, Anchor.transform, true);
        displayal.name = "Displayal (" + e.name + ")";

        var pos = new Vector3(e.position.x, e.position.y, -ExhibitionBuildingSettings.Instance.WallOffset);
        displayal.transform.localPosition = pos;

        // Non-90° as they would tip over otherwise (exhibits stand on the little bar below).
        var rot = VrepController.Instance.settings.PlaygroundEnabled
          ? Quaternion.Euler(92.5f, 0, 180) // Slightly more than 90° or it would fall down.
          : Quaternion.Euler(90, 0, 180);
        displayal.transform.localRotation = rot; // Required due to prefab orientation.

        if (!VrepController.Instance.settings.SpotsEnabled || !e.light)
        {
          displayal.transform.Find("Directional light").gameObject.SetActive(false);
        }

        var disp = displayal.gameObject.GetComponent<Displayal>();
        disp.SetExhibitModel(e);
        disp.originalPosition = pos;
        disp.originalRotation = rot;
        displayals.Add(disp);

        var image = displayal.transform.Find("Plane").gameObject.AddComponent<ImageLoader>();
        image.ReloadImage(e.GetURLEncodedPath());
        displayal.transform.localScale = ScalingUtility.ConvertMeters2PlaneScaleSize(e.size.x, e.size.y);

        if (e.audio == null) continue;

        Debug.Log("Added audio to display object.");

        var closenessDetector = displayal.AddComponent<ClosenessDetector>();
        closenessDetector.url = e.GetURLEncodedAudioPath();
      }
    }

    public WallOrientation GetOrientation()
    {
      return WallData.GetOrientation();
    }
  }
}