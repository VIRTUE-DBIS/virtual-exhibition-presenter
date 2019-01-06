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
    

  }
}