using System;
using System.Linq;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.VREM.Model
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.exhibition.Room
  /// </summary>
  [Serializable]
  public class Room
  {
    public string text;
    public Vector3 size;
    public Vector3 position;
    public Vector3 entrypoint;
    public Wall[] walls;

    public string floor;
    public string ceiling;

    public string ambient;

    public Exhibit[] exhibits;

    public string GetURLEncodedAudioPath()
    {
      if (!string.IsNullOrEmpty(ambient))
      {
        return VrepController.Instance.settings.VREMAddress + "/content/get/" +
               ambient.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
      }

      return null;
    }

    public Wall GetWall(WallOrientation orientation)
    {
      return (from wall in walls
          let wor = (WallOrientation) Enum.Parse(typeof(WallOrientation), wall.direction, true)
          where wor.Equals(orientation)
          select wall)
        .FirstOrDefault();
    }
  }
}