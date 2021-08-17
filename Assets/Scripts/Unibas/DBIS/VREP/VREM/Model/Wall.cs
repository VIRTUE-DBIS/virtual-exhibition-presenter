using System;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Unibas.DBIS.VREP.VREM.Model
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.exhibition.Wall
  /// </summary>
  [Serializable]
  public class Wall
  {
    public string direction;
    public string texture;
    [JsonConverter(typeof(Vec3Conv))] public Vector3 color;
    public Exhibit[] exhibits;

    /// <summary>
    /// Obtains the WallOrientation enum/direction of this wall object.
    /// </summary>
    /// <returns></returns>
    public WallOrientation GetOrientation()
    {
      return (WallOrientation)Enum.Parse(typeof(WallOrientation), direction, true);
    }
  }
}