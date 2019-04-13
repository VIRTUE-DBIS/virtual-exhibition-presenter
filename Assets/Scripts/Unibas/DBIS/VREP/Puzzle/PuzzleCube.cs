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
                                        Hand.AttachmentFlags.VelocityMovement /*| Hand.AttachmentFlags.TurnOffGravity*/;
            Throwable.releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;
        }

        private void Update()
        {
            
            
        }

        public void SetupThrowable()
        {
            /*Debug.Log("Setup Throwable");
            Throwable.attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand |
                                        Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.TurnOffGravity;
            Throwable.releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;*/
            
            /*
             // Throwable properties from displayal
            Throwable.attachmentFlags = Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.TurnOffGravity;
            //Hand.AttachmentFlags.VelocityMovement  Hand.AttachmentFlags.TurnOffGravity;
            Throwable.releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;*/
        }
    }
}