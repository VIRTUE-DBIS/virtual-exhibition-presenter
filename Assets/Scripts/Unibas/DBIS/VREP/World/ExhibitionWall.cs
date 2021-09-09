using System;
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

    private async Task<GameObject> CreateDisplayalFromExhibit(Exhibit e)
    {
      await Task.Yield();

      var displayalPrefab = ObjectFactory.GetDisplayalPrefab();

      var displayal = Instantiate(displayalPrefab, Anchor.transform, true);

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

      Displayal displayalComponent = displayal.gameObject.GetComponent<Displayal>();
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

      return displayal;
    }

    private void AddButtonsToDisplayal(Exhibit e, GameObject displayal)
    {
      var genButtonPrefab = ObjectFactory.GetGenerationButtonPrefab();

      // Get IDs and potential reference.
      var json = e.Metadata[MetadataType.MemberIds.GetKey()];
      var idDoublePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IdDoublePair>>(json);
      var ids = idDoublePairs.Select(it => it.id).ToList();

      RoomReferences references = null;

      if (e.Metadata.ContainsKey(MetadataType.References.GetKey()))
      {
        var refJson = e.Metadata[MetadataType.References.GetKey()];
        references = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomReferences>(refJson);
      }

      var localScale = displayal.transform.localScale;
      var types = Enum.GetValues(typeof(GenerationRequest.GenTypeEnum));
      var offset = 2.0f;
      var shift = types.Length / 2;
      var xFactor = 0.2f / localScale.x;
      var zFactor = 0.2f / localScale.z;

      foreach (GenerationRequest.GenTypeEnum method in types)
      {
        var genButton = Instantiate(genButtonPrefab, displayal.transform, false);
        genButton.name = "Generation Button (" + method.GetName() + ")";

        genButton.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 180.0f);
        genButton.transform.localPosition =
          new Vector3(offset * ((int)method - shift - 1) * xFactor, 0.0f, 7.5f * zFactor);
        genButton.transform.localScale = new Vector3(
          0.75f * xFactor,
          0.75f * zFactor, // Y/Z inverted due to different prefab orientation...
          0.01f
        );

        // Button.
        GenerateButton genButtonComponent = genButton.GetComponent<GenerateButton>();
        genButtonComponent.type = method;
        genButtonComponent.ids = ids;
        if (references != null && references.References.ContainsKey(method.ToString()))
        {
          genButtonComponent.targetRoomId = references.References[method.ToString()];
        }

        TextMesh genButtonText = genButton.GetComponentInChildren<TextMesh>();
        genButtonText.text = method.GetName();
      }
    }

    /// <summary>
    /// Creates and attaches the exhibits defined in this wall's data to the wall (or rather it's anchor).
    /// </summary>
    public async Task AttachExhibits()
    {
      foreach (var e in WallData.Exhibits)
      {
        var displayal = await CreateDisplayalFromExhibit(e);

        // TODO Be smarter about this check and which buttons to add exactly.
        if (e.Metadata.ContainsKey(MetadataType.Generated.GetKey()))
        {
          AddButtonsToDisplayal(e, displayal);
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