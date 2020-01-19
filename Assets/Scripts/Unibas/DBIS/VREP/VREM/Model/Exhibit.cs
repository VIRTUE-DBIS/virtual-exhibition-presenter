using System;
using System.Collections.Generic;
using Unibas.DBIS.VREP;
using UnityEngine;

namespace DefaultNamespace.VREM.Model {
  
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.Ehibit
  /// </summary>
  [Serializable]
  public class Exhibit {
    public string id;
    public string name;
    public string type;
    public string path;
    public string description;

    public string audio;
    public bool light;

    public Dictionary<string, string> metadata;

    public Vector3 position;
    public Vector3 size;

    public string GetURLEncodedPath() {
      var uri = VREPController.Instance.Settings.Server.Address+"content/get/"+path.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
//      Debug.Log("URI="+uri);
      return uri;
    }

    public string GetURLEncodedAudioPath() {
      if (!string.IsNullOrEmpty(audio)) {
        return VREPController.Instance.Settings.Server.Address+"content/get/"+ audio.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
      }
      else
      {
        return null;
      }
    }
    

  }
}