using Unibas.DBIS.VREP.Puzzle;
using UnityEngine;
using World;
using Wall = DefaultNamespace.VREM.Model.Wall;

namespace DefaultNamespace {
  public class Sandbox : MonoBehaviour {

    public bool Enabled;

    public Texture2D tex;
    
    void Start() {
      if (Enabled) {
        TestObjectFactory();
        PuzzleStuff();
      }
      
    }

    private void PuzzleStuff()
    {
      GameObject[] cubes = PuzzleCubeFactory.createPuzzle(tex, 0.5f, new Vector3(1,0,0));
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