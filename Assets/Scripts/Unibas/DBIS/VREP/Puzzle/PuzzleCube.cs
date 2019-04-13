using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Puzzle {
  
  public class PuzzleCube : MonoBehaviour
  {


    public Vector2Int NbCubes;
    
    public int Id;

    public Throwable Throwable;
    
    public void Start()
    {

      Throwable = gameObject.AddComponent<Throwable>();
    }
  }
}