using System.Collections.Generic;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  /// <summary>
  /// Sandbox component to test things.
  /// </summary>
  public class Sandbox : MonoBehaviour
  {
    public bool isEnabled;

    private void Start()
    {
      if (isEnabled)
      {
        TestObjectFactory();
      }
    }

    private static void TestObjectFactory()
    {
      var nw = new Wall { Direction = Wall.DirectionEnum.NORTH, Texture = "NBricks" };
      var ew = new Wall { Direction = Wall.DirectionEnum.EAST, Texture = "LimeBricks" };
      var sw = new Wall { Direction = Wall.DirectionEnum.SOUTH, Texture = "NConcrete" };
      var ww = new Wall { Direction = Wall.DirectionEnum.WEST, Texture = "MarbleBricks" };

      var room = new Room
      {
        Floor = "MarbleTiles",
        Size = new Vector3f(10, 5, 10),
        Position = new Vector3f(10, -10, 10),
        Ceiling = "NFabric",
        Walls = new List<Wall> { nw, ew, sw, ww }
      };

      ObjectFactory.BuildRoom(room);
    }
  }
}