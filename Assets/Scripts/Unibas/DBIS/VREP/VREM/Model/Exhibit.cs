using System;
using System.Collections.Generic;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Utils;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Unibas.DBIS.VREP.VREM.Model
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.exhibition.Exhibit
  /// </summary>
  [Serializable]
  public class Exhibit
  {
    [JsonProperty("_id")] public string id;
    public string name;
    public string type;
    public string path;
    public string description;

    public string audio;
    public bool light;

    [JsonConverter(typeof(Vec3Conv))] public Vector3 position;
    [JsonConverter(typeof(Vec3Conv))] public Vector3 size;

    public Dictionary<string, string> Metadata;

    /// <summary>
    /// Composes the URL where an exhibit's image file can be found via the VREM API.
    /// Note that this relies on exhibition ID stored in the configuration of the controller.
    /// </summary>
    /// <returns>A string with the full path/URL to the image of the exhibit.</returns>
    public string GetURLEncodedPath()
    {
      return VrepController.Instance.settings.VremAddress + "content/get/" +
             VrepController.Instance.settings.ExhibitionId + "%2F" +
             path.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
    }

    /// <summary>
    /// Composes the URL where the audio for an exhibit can be found via the VREM API.
    /// Note that this relies on exhibition ID stored in the configuration of the controller.
    /// </summary>
    /// <returns>A string with the full path/URL to the audio of the exhibit.</returns>
    public string GetURLEncodedAudioPath()
    {
      if (!string.IsNullOrEmpty(audio))
      {
        return VrepController.Instance.settings.VremAddress + "content/get/" +
               VrepController.Instance.settings.ExhibitionId + "%2F" +
               audio.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
      }

      return null;
    }
  }
}