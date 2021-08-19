using System;
using System.Collections.Generic;
using System.Linq;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Unibas.DBIS.VREP.VREM.Model
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.exhibition.Room
  /// </summary>
  [Serializable]
  public class Room
  {
    [JsonProperty("_id")] public string id;
    public string text;

    public string floor;
    public string ceiling;
    public string ambient;

    [JsonConverter(typeof(Vec3Conv))] public Vector3 size;
    [JsonConverter(typeof(Vec3Conv))] public Vector3 position;
    [JsonConverter(typeof(Vec3Conv))] public Vector3 entrypoint;

    public Exhibit[] exhibits;
    public Wall[] walls;
    public Dictionary<string, string> metadata;

    public string GetURLEncodedAudioPath()
    {
      if (!string.IsNullOrEmpty(ambient))
      {
        return VrepController.Instance.settings.VremAddress + "/content/get/" +
               ambient.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
      }

      return null;
    }

    public Wall GetWall(WallOrientation orientation)
    {
      return (from wall in walls
          let wor = (WallOrientation)Enum.Parse(typeof(WallOrientation), wall.direction, true)
          where wor.Equals(orientation)
          select wall)
        .FirstOrDefault();
    }
  }
}