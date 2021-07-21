using Unibas.DBIS.VREP.VREM.Model;
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
      var nw = new Wall {direction = "NORTH", texture = "NBricks"};
      var ew = new Wall {direction = "EAST", texture = "LimeBricks"};
      var sw = new Wall {direction = "SOUTH", texture = "NConcrete"};
      var ww = new Wall {direction = "WEST", texture = "MarbleBricks"};

      var room = new Room
      {
        floor = "MarbleTiles",
        size = new Vector3(10, 5, 10),
        position = new Vector3(10, -10, 10),
        ceiling = "NFabric",
        walls = new[] {nw, ew, sw, ww}
      };

      ObjectFactory.BuildRoom(room);
    }
  }
}