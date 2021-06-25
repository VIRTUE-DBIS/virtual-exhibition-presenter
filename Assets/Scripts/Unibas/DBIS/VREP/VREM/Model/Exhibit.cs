using System;
using System.Collections.Generic;
using Unibas.DBIS.VREP.Core;
using UnityEngine;

namespace Unibas.DBIS.VREP.VREM.Model
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.Ehibit
  /// </summary>
  [Serializable]
  public class Exhibit
  {
    public string id;
    public string name;
    public string type;
    public string path;
    public string description;

    public string audio;
    public bool light;

    public Dictionary<string, string> Metadata;

    public Vector3 position;
    public Vector3 size;

    public string GetURLEncodedPath()
    {
      return VREPController.Instance.settings.VREMAddress + "content/get/" +
             path.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
    }

    public string GetURLEncodedAudioPath()
    {
      if (!string.IsNullOrEmpty(audio))
      {
        return VREPController.Instance.settings.VREMAddress + "content/get/" +
               audio.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
      }

      return null;
    }
  }
}