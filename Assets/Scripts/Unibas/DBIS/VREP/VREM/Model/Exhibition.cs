using System;
using Valve.Newtonsoft.Json;

namespace Unibas.DBIS.VREP.VREM.Model
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.exhibition.Exhibition
  /// </summary>
  [Serializable]
  public class Exhibition
  {
    [JsonProperty("_id")] public string id;
    public string name;
    public string description;

    public Room[] rooms;
  }
}