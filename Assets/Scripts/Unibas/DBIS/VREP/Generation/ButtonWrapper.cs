using System;
using UnityEngine;

namespace Unibas.DBIS.VREP.Generation
{
    public class ButtonWrapper : MonoBehaviour
    {
        public void Update()
        {
            var cameraPosition = Camera.allCameras[0].transform.position;
            var objectPosition = gameObject.transform.position;
      
            var dist = Vector3.Distance(cameraPosition, objectPosition);

            foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.enabled = Math.Abs(dist) < 2.0;
            }
        }
    }
}