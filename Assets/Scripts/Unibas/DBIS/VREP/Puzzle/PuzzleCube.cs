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
            var neighbors = Physics.OverlapSphere(transform.position, Size + (Size / 4f));
            foreach (var neighbor in neighbors)
            {
                var puzzleCube = neighbor.GetComponent<PuzzleCube>();
                if (puzzleCube.Id == Id)
                {
                    break;
                }
                if (puzzleCube != null)
                {
                    var otherPos = puzzleCube.PuzzlePos;
                    if (Math.Abs(Math.Abs(Vector2.Distance(PuzzlePos, otherPos)) - 1) < 0.01f)
                    {
                        Debug.Log("My neighbor ("+puzzleCube.Id+") and I ("+Id+") are correctly set");
                    }
                    else
                    {
                        Debug.Log("My neighbor ("+puzzleCube.Id+") and I ("+Id+") shall not be neighbors!");
                    }
                }
            }
        }

        public void SetupThrowable()
        {
           
        }
        
    }
}