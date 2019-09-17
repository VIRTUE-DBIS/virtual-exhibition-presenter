using UnityEngine;
using World;
using Wall = DefaultNamespace.VREM.Model.Wall;

namespace DefaultNamespace {
  public class Sandbox : MonoBehaviour {

    public bool Enabled;
    
    void Start() {
      if (Enabled) {
        TestObjectFactory();
      }
      
    }

    private void TestObjectFactory() {
      var nw = new VREM.Model.Wall();
      nw.direction = "NORTH";
      nw.texture = "NBricks";
      var ew = new VREM.Model.Wall();
      ew.direction = "EAST";
      ew.texture = "LimeBricks";
      var sw = new VREM.Model.Wall();
      sw.direction = "SOUTH";
      sw.texture = "NConcrete";
      var ww = new VREM.Model.Wall();
      ww.direction = "WEST";
      ww.texture = "MarbleBricks";

      var room = new VREM.Model.Room();
      room.floor = "MarbleTiles";
      room.size = new Vector3(10,5,10);
      room.position = new Vector3(10,-10,10);
      room.ceiling = "NFabric";
      room.walls = new[] {
        nw, ew, sw, ww
      };

      ObjectFactory.BuildRoom(room);
    }
    
  }
  
  
}