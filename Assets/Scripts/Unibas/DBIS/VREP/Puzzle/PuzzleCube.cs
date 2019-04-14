using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Puzzle {
  public class PuzzleCube : MonoBehaviour {
    private Vector2Int NbCubes;

    public int Id;

    private Throwable Throwable;

    private GameObject _parentThrowable;

    private Vector2Int PuzzlePos { get; set; }
    private float Size { get; set; }

    public bool topNeighbor = false;
    public bool bottomNeighbor = false;
    public bool leftNeighbor = false;
    public bool rightNeighbor = false;

    public Vector2Int expectedTop;
    public Vector2Int expectedLeft;
    public Vector2Int expectedBottom;
    public Vector2Int expectedRight;

    public bool DoTriggerChecks = false;
    
    private void Awake() { }

    public void Setup(int id, float size) {
      Id = id;
      Size = size;

      NbCubes = PuzzleManager.GetInstance().nbCubes;
      PuzzlePos = PuzzleManager.GetInstance().GetPosForId(Id);

      RestoreInitialNeighborChecks();

      expectedTop = PuzzlePos + Vector2Int.up;
      expectedLeft = PuzzlePos + Vector2Int.left;
      expectedBottom = PuzzlePos + Vector2Int.down;
      expectedRight = PuzzlePos + Vector2Int.right;
      
      Debug.Log("Setup "+Id+"->"+PuzzlePos);

    }

    public void RestoreInitialNeighborChecks()
    {
      Debug.Log("PuzzleCube "+Id+" .. restoring neighbors b,l,t,r "+ (PuzzlePos.y == 0)+","+(PuzzlePos.x == 0)+","+(PuzzlePos.y == NbCubes.y-1)+","+(+PuzzlePos.x == NbCubes.x-1));
      bottomNeighbor = PuzzlePos.y == 0;
      leftNeighbor = PuzzlePos.x == 0;
      topNeighbor = PuzzlePos.y == (NbCubes.y - 1);
      rightNeighbor = PuzzlePos.x == NbCubes.x - 1;
    }

    public void Start() {
      _parentThrowable = new GameObject("Throwable of " + this.name);
      Throwable = _parentThrowable.AddComponent<Throwable>();
      _parentThrowable.transform.SetParent(transform.parent, false);
      _parentThrowable.transform.position = transform.position;
      transform.SetParent(_parentThrowable.transform, false);
      transform.transform.localPosition = Vector3.zero;
      Throwable.attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand |
                                  Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.TurnOffGravity;

      Throwable.releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;
    }

    [Obsolete]
    private void Update() {
      /*
      var neighbors = Physics.OverlapSphere(transform.position+new Vector3(Size/2f, Size/2f, Size/2f), Size + .05f);
      foreach (var neighbor in neighbors)
      {
          var puzzleCube = neighbor.GetComponent<PuzzleCube>();
          if (puzzleCube == null)
          {
              break;
          }
          if (puzzleCube.Id == Id)
          {
              break;
          }
          if (puzzleCube != null)
          {
              var otherPos = puzzleCube.PuzzlePos;
              var dis = Math.Abs(Vector2.Distance(PuzzlePos, otherPos)); 
              if (Math.Abs(dis-1) < 0.01f)
              {
                  Debug.Log("My neighbor ("+puzzleCube.Id+") and I ("+Id+") are correctly set");
                  PuzzleManager.GetInstance().SetPositionCheck(PuzzlePos.x, PuzzlePos.y, true);
              }
              else
              {
                  Debug.Log("Incorrect: this("+Id+"), other("+puzzleCube.Id+") dist="+ dis);
              }
          }
      }*/
    }

    public void OnSidedTriggerEnter(PuzzleSide side, PuzzleCubeSide collidedWith) {
      if (!DoTriggerChecks)
      {
        return;
      }
      Debug.Log("ENTER: " + Id + " collide on " + side + " with " + collidedWith.PuzzleCube.Id + "'s " +
                collidedWith.Side + " side");

      var otherPos = PuzzleManager.GetInstance().GetPosForId(collidedWith.PuzzleCube.Id);
      if (otherPos == expectedTop) {
        topNeighbor = side == PuzzleSide.TOP && collidedWith.Side == PuzzleSide.BOTTOM;
      } else if (otherPos == expectedLeft) {
        leftNeighbor = side == PuzzleSide.LEFT && collidedWith.Side == PuzzleSide.RIGHT;
      } else if (otherPos == expectedBottom) {
        bottomNeighbor = side == PuzzleSide.BOTTOM && collidedWith.Side == PuzzleSide.TOP;
      }else if (otherPos == expectedRight) {
        rightNeighbor = side == PuzzleSide.RIGHT && collidedWith.Side == PuzzleSide.LEFT;
      }
      
      Debug.Log("My correct neighbors: "+Id+"("+PuzzlePos.x+","+PuzzlePos.y+") : "+correctNeighbors());
      PuzzleManager.GetInstance().SetPositionCheck(PuzzlePos.x, PuzzlePos.y, correctNeighbors() == 4);
    }
    
    

    public void OnSidedTriggerExit(PuzzleSide side, PuzzleCubeSide collidedWith) {
      if (!DoTriggerChecks)
      {
        return;
      }
      Debug.Log("ENTER: " + Id + " collide on " + side + " with " + collidedWith.PuzzleCube.Id + "'s " +
                collidedWith.Side + " side");
      
      var otherPos = PuzzleManager.GetInstance().GetPosForId(collidedWith.PuzzleCube.Id);
      if (otherPos == expectedTop) {
        topNeighbor = !(side == PuzzleSide.TOP && collidedWith.Side == PuzzleSide.BOTTOM);
      } else if (otherPos == expectedLeft) {
        leftNeighbor = !(side == PuzzleSide.LEFT && collidedWith.Side == PuzzleSide.RIGHT);
      } else if (otherPos == expectedBottom) {
        bottomNeighbor = !(side == PuzzleSide.BOTTOM && collidedWith.Side == PuzzleSide.TOP);
      }else if (otherPos == expectedRight) {
        rightNeighbor = !(side == PuzzleSide.RIGHT && collidedWith.Side == PuzzleSide.LEFT);
      }
      
      PuzzleManager.GetInstance().SetPositionCheck(PuzzlePos.x, PuzzlePos.y, correctNeighbors() == 4);
    }

    private int correctNeighbors() {
      int c = 0;
      if (topNeighbor) {
        c++;
      }

      if (bottomNeighbor) {
        c++;
      }

      if (rightNeighbor) {
        c++;
      }

      if (leftNeighbor) {
        c++;
      }

      return c;
    }

    [Obsolete]
    public void SetupThrowable() { }
  }
}