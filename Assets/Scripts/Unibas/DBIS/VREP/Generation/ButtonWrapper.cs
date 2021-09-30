using System;
using Unibas.DBIS.VREP.LegacyObjects;
using UnityEngine;

namespace Unibas.DBIS.VREP.Generation
{
  public class ButtonWrapper : MonoBehaviour
  {
    public Displayal displayal;

    private const float ThreshDist = 3.0f;

    public void Update()
    {
      var cameraPosition = Camera.allCameras[0].transform.position;
      var objectPosition = gameObject.transform.position;

      var dist = Vector3.Distance(cameraPosition, objectPosition);

      foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
      {
        r.enabled = Math.Abs(dist) < ThreshDist;
      }
    }
  }
}