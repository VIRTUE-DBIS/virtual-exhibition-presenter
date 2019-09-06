using System;
using UnityEngine;

namespace Unibas.DBIS.VREP.Puzzle {
  
  public class PuzzleCubeSide : MonoBehaviour {

    [SerializeField]
    public PuzzleSide Side;

    [SerializeField]
    public PuzzleCube PuzzleCube;

    private void OnTriggerEnter(Collider other) {
      var otherSide = other.gameObject.GetComponent<PuzzleCubeSide>();
      if (otherSide != null) {
        PuzzleCube.OnSidedTriggerEnter(Side, otherSide);
      }
    }

    private void OnTriggerExit(Collider other) {
      var otherSide = other.gameObject.GetComponent<PuzzleCubeSide>();
      if (otherSide != null) {
        PuzzleCube.OnSidedTriggerExit(Side, otherSide);
      }
    }
    
    
  }
}