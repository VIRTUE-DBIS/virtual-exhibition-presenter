using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Puzzle
{
    public class PuzzleCube : MonoBehaviour
    {
        public Vector2Int NbCubes;

        public int Id;

        public Throwable Throwable;

        private GameObject _parentThrowable;
        
        public Vector2Int PuzzlePos { get; set; }
        public float Size { get; set; }
        
        private void Awake()
        {
            
        }

        public void Start()
        {
            _parentThrowable = new GameObject("Throwable of "+this.name);
            Throwable = _parentThrowable.AddComponent<Throwable>();
            _parentThrowable.transform.SetParent(transform.parent, false);
            _parentThrowable.transform.position = transform.position;
            transform.SetParent(_parentThrowable.transform, false);
            transform.transform.position = Vector3.zero;
            Throwable.attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand |
                                        Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.TurnOffGravity;
            
            Throwable.releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;
        }

        private void Update()
        {
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
            }
        }

        public void OnSidedTriggerEnter(PuzzleSide side, PuzzleCubeSide collidedWith) {
            Debug.Log("ENTER: "+Id+" collide on "+side+" with "+collidedWith.PuzzleCube.Id+"'s "+collidedWith.Side+" side");
        }

        public void OnSidedTriggerExit(PuzzleSide side, PuzzleCubeSide collidedWith) {
            Debug.Log("ENTER: "+Id+" collide on "+side+" with "+collidedWith.PuzzleCube.Id+"'s "+collidedWith.Side+" side");
        }
        
        
        
        [Obsolete]
        public void SetupThrowable()
        {
           
        }
        
    }
}