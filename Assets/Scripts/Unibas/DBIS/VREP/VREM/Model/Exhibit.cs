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
      return VREPController.Instance.Settings.VREMAddress+"content/get/"+path.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
    }

    public string GetURLEncodedAudioPath() {
      if (!string.IsNullOrEmpty(audio)) {
        //return "https://upload.wikimedia.org/wikipedia/commons/7/7b/FurElise.ogg";
        return VREPController.Instance.Settings.VREMAddress+"content/get/"+ audio.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
      }
      else
      {
        return null;
      }
    }
    

  }
}