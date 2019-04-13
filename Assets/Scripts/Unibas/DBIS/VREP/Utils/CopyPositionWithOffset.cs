using UnityEngine;

namespace Unibas.DBIS.VREP.Utils
{
    public class CopyPositionWithOffset : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;

        void Update()
        {
            transform.position = target.position + offset;
        }
    }
}
