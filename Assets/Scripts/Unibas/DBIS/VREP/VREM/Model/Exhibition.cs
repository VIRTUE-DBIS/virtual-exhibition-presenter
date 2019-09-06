using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.VREM.Model {
  
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.exhibition.Exhibition
  /// </summary>
  [Serializable]
  public class Exhibition {

    public string id;
    public string name;
    public string description;

    public Room[] rooms;

    public Exhibit[] GetExhibits()
    {
      List<Exhibit> exhibits = new List<Exhibit>();
      foreach (var room in rooms)
      {
        exhibits.AddRange(room.GetWallExhibits());
      }

      return exhibits.ToArray();
    }
    
  }
}