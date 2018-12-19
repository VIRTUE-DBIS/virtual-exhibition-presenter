using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.VREM.Model {
  [Serializable]
  public class Room {

    public string text;
    public Vector3 size;
    public Vector3 position;
    public Vector3 entrypoint;
    public Wall[] walls;

    public string floor;
    public string ceiling;

    public string ambient;

    public Exhibit[] exhibits;    
    
    
    public string GetURLEncodedAudioPath() {
      if (!string.IsNullOrEmpty(ambient))
      {
        return ServerSettings.SERVER_ID+"content/get/"+ ambient.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
      }
      else
      {
        return null;
      }
      
    }
    
  }
}