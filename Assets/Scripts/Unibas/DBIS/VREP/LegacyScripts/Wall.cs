using System;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.LegacyScripts
{
  [Obsolete("Got replaced by ExhibitionWall")]
  public class Wall : TexturedMonoBehaviour
  {
    public WallOrientation orientation;

    public float roomRadius = 5;

    /// <summary>
    ///   Calculates the position relative to the wall. Origin is lower left corner
    /// </summary>
    /// <param name="floorCenter"></param>
    /// <param name="pos">x value is to the right, y value is up</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Vector3 CalculatePosition(Vector3 floorCenter, Vector2 pos)
    {
      const float epsilon = 0.1f; // Required 0.1f with Displayal

      return orientation switch
      {
        WallOrientation.North => new Vector3(floorCenter.x + (roomRadius - epsilon), floorCenter.y + pos.y,
          floorCenter.z + roomRadius - pos.x),
        WallOrientation.East => new Vector3(floorCenter.x + roomRadius - pos.x, floorCenter.y + pos.y,
          floorCenter.z - (roomRadius - epsilon)),
        WallOrientation.South => new Vector3(floorCenter.x - (roomRadius - epsilon), floorCenter.y + pos.y,
          floorCenter.z - roomRadius + pos.x),
        WallOrientation.West => new Vector3(floorCenter.x - roomRadius + pos.x, floorCenter.y + pos.y,
          floorCenter.z + (roomRadius - epsilon)),
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    public Vector3 CalculateRotation()
    {
      return orientation switch
      {
        WallOrientation.North => new Vector3(90, 270, 0),
        WallOrientation.East => new Vector3(90, 0, 0),
        WallOrientation.South => new Vector3(90, 90, 0),
        WallOrientation.West => new Vector3(90, 180, 0),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}