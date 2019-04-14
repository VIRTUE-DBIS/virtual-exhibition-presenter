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
    public float Size { get; set; }

    public bool topNeighbor = false;
    public bool bottomNeighbor = false;
    public bool leftNeighbor = false;
    public bool rightNeighbor = false;


    private bool doTestTop = true;
    private bool doTestRight = true;
    private bool doTestBottom = true;
    private bool doTestLeft = true;
    
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

      doTestTop = !topNeighbor;
      doTestLeft = !leftNeighbor;
      doTestBottom = !bottomNeighbor;
      doTestRight = !rightNeighbor;
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
      switch (side)
      {
        case PuzzleSide.LEFT:
          if (doTestLeft)
          {
            leftNeighbor = side == PuzzleSide.LEFT && collidedWith.Side == PuzzleSide.RIGHT && expectedLeft == collidedWith.PuzzleCube.PuzzlePos;
            Debug.Log("Left: "+leftNeighbor);
          }
          break;
        case PuzzleSide.RIGHT:
          if (doTestRight)
          {
            rightNeighbor = side == PuzzleSide.RIGHT && collidedWith.Side == PuzzleSide.LEFT && expectedRight == collidedWith.PuzzleCube.PuzzlePos;
            Debug.Log("Right: "+rightNeighbor);
          }
          break;
        case PuzzleSide.TOP:
          if (doTestTop)
          {
            topNeighbor = side == PuzzleSide.TOP && collidedWith.Side == PuzzleSide.BOTTOM && expectedTop == collidedWith.PuzzleCube.PuzzlePos;
            Debug.Log("Top: "+topNeighbor);
          }
          break;
        case PuzzleSide.BOTTOM:
          if (doTestBottom)
          {
            bottomNeighbor = side == PuzzleSide.BOTTOM && collidedWith.Side == PuzzleSide.TOP && expectedBottom == collidedWith.PuzzleCube.PuzzlePos;
            Debug.Log("Bottom: "+bottomNeighbor);
          }
          break;
        default:
          // Ignore;
          break;
      }
      Debug.Log("My nb correct neighbors: "+Id+"("+PuzzlePos.x+","+PuzzlePos.y+") : "+correctNeighbors()+" of "+neighborsToCheck());
      PuzzleManager.GetInstance().SetPositionCheck(PuzzlePos.x, PuzzlePos.y, correctNeighbors() >= neighborsToCheck());
    }
    
    

    public void OnSidedTriggerExit(PuzzleSide side, PuzzleCubeSide collidedWith) {
      if (!DoTriggerChecks)
      {
        return;
      }
      Debug.Log("EXIT: " + Id + " collide on " + side + " with " + collidedWith.PuzzleCube.Id + "'s " +
                collidedWith.Side + " side");

      
      switch (side)
      {
        case PuzzleSide.LEFT:
          if (doTestLeft)
          {
            leftNeighbor = !(side == PuzzleSide.LEFT && collidedWith.Side == PuzzleSide.RIGHT && expectedLeft == collidedWith.PuzzleCube.PuzzlePos);
            Debug.Log("Left: "+leftNeighbor);
          }
          break;
        case PuzzleSide.RIGHT:
          if (doTestRight)
          {
            rightNeighbor = !(side == PuzzleSide.RIGHT && collidedWith.Side == PuzzleSide.LEFT && expectedRight == collidedWith.PuzzleCube.PuzzlePos);
            Debug.Log("Right: "+rightNeighbor);
          }
          break;
        case PuzzleSide.TOP:
          if (doTestTop)
          {
            topNeighbor = !(side == PuzzleSide.TOP && collidedWith.Side == PuzzleSide.BOTTOM && expectedTop == collidedWith.PuzzleCube.PuzzlePos);
            Debug.Log("Top: "+topNeighbor);
          }
          break;
        case PuzzleSide.BOTTOM:
          if (doTestBottom)
          {
            bottomNeighbor = !(side == PuzzleSide.BOTTOM && collidedWith.Side == PuzzleSide.TOP && expectedBottom == collidedWith.PuzzleCube.PuzzlePos);
            Debug.Log("Bottom: "+bottomNeighbor);
          }
          break;
        default:
          // Ignore;
          break;
      }
      
      
      PuzzleManager.GetInstance().SetPositionCheck(PuzzlePos.x, PuzzlePos.y, correctNeighbors() >= neighborsToCheck()); // TODO more sophistcated?
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

    private int neighborsToCheck()
    {
      int c = 0;
      if (doTestBottom)
      {
        c++;
      }

      if (doTestRight)
      {
        c++;
      }

      if (doTestTop)
      {
        c++;
      }

      if (doTestLeft)
      {
        c++;
      }

      return c;
    }

    [Obsolete]
    public void SetupThrowable() { }
  }
}