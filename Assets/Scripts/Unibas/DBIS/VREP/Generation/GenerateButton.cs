using System.Collections.Generic;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.Core;
using UnityEngine;

namespace Unibas.DBIS.VREP.Generation
{
  public class GenerateButton : MonoBehaviour
  {
    public GenerationRequest.GenTypeEnum type;
    public List<string> ids;

    public Vector3 destination; // Use room here?

    public void ButtonPress()
    {
      Debug.Log("Pressed.");
      VrepController.Instance.GenerateAndLoadExhibition(type, ids);
    }
  }
}