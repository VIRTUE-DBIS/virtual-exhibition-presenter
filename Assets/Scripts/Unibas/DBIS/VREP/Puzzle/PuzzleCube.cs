using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Puzzle
{
    public class PuzzleCube : MonoBehaviour
    {
        public Vector2Int NbCubes;

        public int Id;

        public Throwable Throwable;

        private void Awake()
        {
            
        }

        public void Start()
        {
            gameObject.AddComponent<Throwable>();
            Throwable = gameObject.GetComponent<Throwable>();
        }

        private void Update()
        {
            
            
        }

        public void SetupThrowable()
        {
            Debug.Log("Setup Throwable");
            Throwable.attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand |
                                        Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.TurnOffGravity;
            Throwable.releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;
            
            /*
             // Throwable properties from displayal
            Throwable.attachmentFlags = Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.TurnOffGravity;
            //Hand.AttachmentFlags.VelocityMovement  Hand.AttachmentFlags.TurnOffGravity;
            Throwable.releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;*/
        }
    }
}