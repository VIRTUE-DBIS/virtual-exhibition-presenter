using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Generation;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.Multimedia;
using Unibas.DBIS.VREP.Utils;
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

    /// <summary>
    /// Holds all displayals of this wall.
    /// </summary>
    public List<Displayal> displayals = new List<Displayal>();

    /// <summary>
    /// Restores the position of every displayal on this wall.
    /// </summary>
    public void RestoreDisplayals()
    {
      displayals.ForEach(d => d.RestorePosition());
    }

    /// <summary>
    /// Creates and attaches the exhibits defined in this wall's data to the wall (or rather it's anchor).
    /// </summary>
    public async Task AttachExhibits()
    {
      var displayalPrefab = ObjectFactory.GetDisplayalPrefab();
      var genButtonPrefab = ObjectFactory.GetGenerationButtonPrefab();

      foreach (var e in WallData.Exhibits)
      {
        await Task.Yield();

        var displayal = Instantiate(displayalPrefab, Anchor.transform, true);
        displayal.name = "Displayal (" + e.Name + ")";

        var pos = new Vector3(e.Position.X, e.Position.Y, -ExhibitionBuildingSettings.Instance.WallOffset);
        displayal.transform.localPosition = pos;

        // Non-90° as they would tip over otherwise (exhibits stand on the little bar below).
        var rot = VrepController.Instance.settings.PlaygroundEnabled
          ? Quaternion.Euler(92.5f, 0, 180) // Slightly more than 90° or it would fall down.
          : Quaternion.Euler(90, 0, 180);
        displayal.transform.localRotation = rot; // Required due to prefab orientation.

        if (!VrepController.Instance.settings.SpotsEnabled || !e.Light)
        {
          displayal.transform.Find("Directional light").gameObject.SetActive(false);
        }

        var displayalComponent = displayal.gameObject.GetComponent<Displayal>();
        displayalComponent.SetExhibitModel(e);
        displayalComponent.originalPosition = pos;
        displayalComponent.originalRotation = rot;
        displayals.Add(displayalComponent);

        var image = displayal.transform.Find("Plane").gameObject.AddComponent<ImageLoader>();
        image.ReloadImage(e.Path);
        displayal.transform.localScale = ScalingUtility.ConvertMeters2PlaneScaleSize(e.Size.X, e.Size.Y);

        if (e.Audio != null)
        {
          var closenessDetector = displayal.AddComponent<ClosenessDetector>();
          closenessDetector.url = e.Audio;
          Debug.Log("Added audio to display object.");
        }

        // Check if the exhibit represents a set of images in order to allow for generation of further rooms.
        if (e.Metadata.ContainsKey(MetadataType.MemberIds.GetKey()))
        {
          var json = e.Metadata[MetadataType.MemberIds.GetKey()];
          var idDoublePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IdDoublePair>>(json);
          var ids = idDoublePairs.Select(it => it.id).ToList();

          var genButton = Instantiate(genButtonPrefab, displayal.transform, false);
          genButton.transform.localPosition = new Vector3(0.0f, 0.0f, 10.0f);
          genButton.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 180.0f);

          GenerateButton genButtonComponent = genButton.GetComponent<GenerateButton>();
          genButtonComponent.type = GenerationRequest.GenTypeEnum.VISUALSOM;
          genButtonComponent.ids = ids;
        }
      }
    }

    /// <summary>
    /// Returns the orientation of this wall.
    /// </summary>
    /// <returns>The wall's orientation as WallOrientation enum object.</returns>
    public Wall.DirectionEnum GetOrientation()
    {
      return WallData.Direction;
    }
  }
}